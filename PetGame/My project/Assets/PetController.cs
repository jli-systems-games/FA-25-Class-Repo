using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Playables;

[DisallowMultipleComponent]
public class CrewController : MonoBehaviour
{
    public enum MoveMode { CharacterController, NavMeshAgent, TransformAdditive }

    public Animator animator;
    public Camera cam;
    public LayerMask groundMask = ~0;

    public MoveMode moveMode = MoveMode.CharacterController;
    public CharacterController characterController;
    public NavMeshAgent agent;
    public float moveSpeed = 3.2f;
    public float turnSpeed = 9f;
    public float stopDistance = 0.12f;
    public float gravity = -18f;

    public KeyCode danceToggleKey = KeyCode.Space;
    public KeyCode reloadKey = KeyCode.R;
    public int moveMouseButton = 0;

    public AudioClip walkLoop; public float walkVolume = 0.8f;
    public AudioClip bottleMusic; public float bottleMusicVolume = 1f;

    public AnimationClip dieClip;
    public AudioClip dieSfx; public float dieVolume = 1f;
    public float deathReloadDelay = 2f;

    public AnimationClip sleepClip;
    public AudioClip sleepSfx; public float sleepVolume = 1f;

    public AnimationClip bottleGiveClip;
    public AudioClip danceMusic; public float danceMusicVolume = 1f;
    public AnimationClip danceClip;
    public float crossFadeDuration = 0.18f;

    PlayableGraph graph;
    AnimationPlayableOutput output;
    AnimationMixerPlayable mixer;
    AnimationClipPlayable clipA, clipB;
    int activePort; float fadeTimer, fadeDur;

    AudioSource sfxSource;
    AudioSource footSource;
    AudioSource musicSource;

    public string speedParamName = "Speed";
    int hashSpeed;

    Vector3 targetPos; bool hasTarget; float verticalVel;

    public bool IsDancing { get; private set; }
    public bool IsDead { get; private set; }
    bool actionBusy;
    bool danceVisualActive;

    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        cam = Camera.main;
        characterController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!cam) cam = Camera.main;

        sfxSource = gameObject.AddComponent<AudioSource>(); sfxSource.playOnAwake = false;
        footSource = gameObject.AddComponent<AudioSource>(); footSource.playOnAwake = false; footSource.loop = true; footSource.volume = walkVolume;
        musicSource = gameObject.AddComponent<AudioSource>(); musicSource.playOnAwake = false; musicSource.loop = true;

        hashSpeed = Animator.StringToHash(string.IsNullOrEmpty(speedParamName) ? "Speed" : speedParamName);
        animator.SetFloat(hashSpeed, 0f);

        SetupGraph();

        if (moveMode == MoveMode.NavMeshAgent && agent) { agent.updateRotation = false; agent.updatePosition = true; }
    }

    void OnEnable() { if (graph.IsValid()) graph.Play(); }
    void OnDisable() { if (graph.IsValid()) graph.Stop(); }
    void OnDestroy() { if (graph.IsValid()) graph.Destroy(); }

    void Update()
    {
        if (Input.GetMouseButtonDown(moveMouseButton))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit, 400f, groundMask))
                SetDestination(hit.point);
        }

        if (Input.GetKeyDown(danceToggleKey)) ToggleDance();
        if (Input.GetKeyDown(reloadKey)) SceneReloadFader.Reload(0f, 2f);
        if (Input.GetMouseButtonDown(1)) ForceIdle();

        Vector3 horizVel = Vector3.zero;
        switch (moveMode)
        {
            case MoveMode.CharacterController: horizVel = TickCharacterController(); break;
            case MoveMode.TransformAdditive: horizVel = TickTransform(); break;
            case MoveMode.NavMeshAgent: horizVel = TickNavMesh(); break;
        }
        if (animator && hashSpeed != 0) animator.SetFloat(hashSpeed, horizVel.magnitude, 0.12f, Time.deltaTime);

        bool shouldWalk = horizVel.magnitude > 0.1f && (characterController ? characterController.isGrounded : true) && !IsDead && !actionBusy;
        if (shouldWalk && walkLoop)
        {
            if (!footSource.isPlaying || footSource.clip != walkLoop)
            { footSource.clip = walkLoop; footSource.volume = walkVolume; footSource.Play(); }
        }
        else if (footSource.isPlaying) footSource.Stop();

        MaintainDanceLayer();

        if (fadeDur > 0f && fadeTimer < fadeDur)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDur);
            if (activePort == 0) { mixer.SetInputWeight(0, 1f - t); mixer.SetInputWeight(1, t); }
            else { mixer.SetInputWeight(0, t); mixer.SetInputWeight(1, 1f - t); }
        }
    }

    public void ForceIdle()
    {
        IsDancing = false;
        actionBusy = false;
        hasTarget = false;
        if (moveMode == MoveMode.NavMeshAgent && agent) { agent.ResetPath(); agent.velocity = Vector3.zero; }
        sfxSource.Stop();
        musicSource.Stop();
        footSource.Stop();
        StopAllCoroutines();
        StopDanceVisuals();
        output.SetWeight(0f);
    }

    public void GiveItem(GiveItemType type)
    {
        if (type == GiveItemType.Money)
        {
            var hud = FindFirstObjectByType<HUDInventoryUI>();
            hud?.RefillMoneyFull();
        }
        else if (type == GiveItemType.Bottle)
        {
            var hud = FindFirstObjectByType<HUDInventoryUI>();
            hud?.AddWater(25);
            if (!actionBusy) StartCoroutine(CoDrink());
        }
    }

    public void SetDancing(bool on)
    {
        IsDancing = on;
        if (!on) StopDanceVisuals();
    }

    public void ToggleDance()
    {
        IsDancing = !IsDancing;
        if (!IsDancing) StopDanceVisuals();
    }

    IEnumerator CoDrink()
    {
        actionBusy = true;
        StopDanceVisuals();
        if (bottleGiveClip) PlayActionClip(bottleGiveClip, crossFadeDuration);
        PlayOnce(sfxSource, bottleMusic, 1f);
        float wait = 0f;
        if (bottleGiveClip) wait = Mathf.Max(wait, bottleGiveClip.length);
        if (bottleMusic) wait = Mathf.Max(wait, bottleMusic.length);
        if (wait <= 0f) wait = 0.5f;
        float t = 0f;
        while (t < wait) { t += Time.deltaTime; yield return null; }
        actionBusy = false;
    }

    public void TriggerSleep()
    {
        if (actionBusy) return;
        StopDanceVisuals();
        if (sleepClip) PlayActionClip(sleepClip, 0.18f);
        PlayOnce(sfxSource, sleepSfx, sleepVolume);
    }

    public void TriggerDeath()
    {
        if (IsDead) return;
        StartCoroutine(CoDieAndReload());
    }

    IEnumerator CoDieAndReload()
    {
        IsDead = true;
        hasTarget = false;
        if (moveMode == MoveMode.NavMeshAgent && agent) { agent.ResetPath(); agent.velocity = Vector3.zero; }
        footSource.Stop();
        StopDanceVisuals();
        if (dieClip) PlayActionClip(dieClip, 0.12f);
        PlayOnce(sfxSource, dieSfx, dieVolume);
        SceneReloadFader.Reload(deathReloadDelay, 2f);
        yield break;
    }

    void MaintainDanceLayer()
    {
        if (IsDead || actionBusy) { StopDanceVisuals(); return; }
        if (IsDancing)
        {
            if (!danceVisualActive)
            {
                if (danceClip) PlayLoopClip(danceClip, crossFadeDuration);
                if (danceMusic)
                {
                    musicSource.Stop();
                    musicSource.clip = danceMusic;
                    musicSource.volume = Mathf.Clamp01(danceMusicVolume);
                    musicSource.loop = true;
                    musicSource.time = 0f;
                    musicSource.Play();
                }
                danceVisualActive = true;
            }
        }
        else
        {
            StopDanceVisuals();
        }
    }

    void StopDanceVisuals()
    {
        if (danceVisualActive)
        {
            musicSource.Stop();
            if (!actionBusy) output.SetWeight(0f);
            danceVisualActive = false;
        }
    }

    void PlayActionClip(AnimationClip clip, float crossFade)
    {
        PlayClip(clip, crossFade);
        output.SetWeight(1f);
        StopCoroutine(nameof(CoAutoReleaseOutput));
        StartCoroutine(CoAutoReleaseOutput(clip ? clip.length : 0.5f));
    }

    IEnumerator CoAutoReleaseOutput(float after)
    {
        float t = 0f;
        while (t < after) { t += Time.deltaTime; yield return null; }
        if (!danceVisualActive) output.SetWeight(0f);
    }

    void SetupGraph()
    {
        if (graph.IsValid()) graph.Destroy();
        graph = PlayableGraph.Create("CrewGraph");
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        output = AnimationPlayableOutput.Create(graph, "AnimOutput", animator);
        output.SetWeight(0f);
        mixer = AnimationMixerPlayable.Create(graph, 2);
        output.SetSourcePlayable(mixer);
        clipA = AnimationClipPlayable.Create(graph, null);
        clipB = AnimationClipPlayable.Create(graph, null);
        clipA.SetApplyFootIK(true); clipB.SetApplyFootIK(true);
        graph.Connect(clipA, 0, mixer, 0);
        graph.Connect(clipB, 0, mixer, 1);
        mixer.SetInputWeight(0, 1f); mixer.SetInputWeight(1, 0f);
        activePort = 0; fadeTimer = fadeDur = 0f;
    }

    void PlayLoopClip(AnimationClip clip, float crossFade = 0.18f)
    {
        if (!clip) return;
        if (activePort == 0)
        {
            clipB.Destroy();
            clipB = AnimationClipPlayable.Create(graph, clip);
            clipB.SetApplyFootIK(true); clipB.SetTime(0); clipB.SetSpeed(1); clipB.SetDuration(double.PositiveInfinity);
            mixer.DisconnectInput(1); graph.Connect(clipB, 0, mixer, 1);
            mixer.SetInputWeight(0, 1f); mixer.SetInputWeight(1, 0f);
            fadeDur = Mathf.Max(0.01f, crossFade); fadeTimer = 0f; activePort = 1;
        }
        else
        {
            clipA.Destroy();
            clipA = AnimationClipPlayable.Create(graph, clip);
            clipA.SetApplyFootIK(true); clipA.SetTime(0); clipA.SetSpeed(1); clipA.SetDuration(double.PositiveInfinity);
            mixer.DisconnectInput(0); graph.Connect(clipA, 0, mixer, 0);
            mixer.SetInputWeight(0, 0f); mixer.SetInputWeight(1, 1f);
            fadeDur = Mathf.Max(0.01f, crossFade); fadeTimer = 0f; activePort = 0;
        }
        output.SetWeight(1f);
    }

    void PlayClip(AnimationClip clip, float crossFade = 0.18f)
    {
        if (!clip) return;
        if (activePort == 0)
        {
            clipB.Destroy();
            clipB = AnimationClipPlayable.Create(graph, clip);
            clipB.SetApplyFootIK(true); clipB.SetTime(0); clipB.SetSpeed(1);
            mixer.DisconnectInput(1); graph.Connect(clipB, 0, mixer, 1);
            mixer.SetInputWeight(0, 1f); mixer.SetInputWeight(1, 0f);
            fadeDur = Mathf.Max(0.01f, crossFade); fadeTimer = 0f; activePort = 1;
        }
        else
        {
            clipA.Destroy();
            clipA = AnimationClipPlayable.Create(graph, clip);
            clipA.SetApplyFootIK(true); clipA.SetTime(0); clipA.SetSpeed(1);
            mixer.DisconnectInput(0); graph.Connect(clipA, 0, mixer, 0);
            mixer.SetInputWeight(0, 0f); mixer.SetInputWeight(1, 1f);
            fadeDur = Mathf.Max(0.01f, crossFade); fadeTimer = 0f; activePort = 0;
        }
    }

    void PlayOnce(AudioSource src, AudioClip clip, float volume = 1f)
    {
        if (!src) return;
        src.Stop();
        if (!clip) { src.clip = null; return; }
        src.clip = clip; src.volume = Mathf.Clamp01(volume); src.loop = false; src.time = 0f; src.Play();
    }

    void SetDestination(Vector3 worldPoint)
    {
        if (IsDead || actionBusy) return;
        targetPos = worldPoint;
        if (moveMode != MoveMode.NavMeshAgent) targetPos.y = transform.position.y;
        hasTarget = true;
        if (moveMode == MoveMode.NavMeshAgent && agent) agent.SetDestination(worldPoint);
    }

    Vector3 TickCharacterController()
    {
        if (!characterController) return Vector3.zero;
        Vector3 pos = transform.position;
        Vector3 to = targetPos - pos; to.y = 0f;
        float dist = to.magnitude;
        bool moving = !IsDead && !actionBusy && hasTarget && dist > stopDistance;
        if (moving && to.sqrMagnitude > 1e-6f)
        {
            var rot = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }
        Vector3 horiz = moving ? transform.forward * moveSpeed : Vector3.zero;
        if (characterController.isGrounded && verticalVel < 0f) verticalVel = -1f;
        verticalVel += gravity * Time.deltaTime;
        characterController.Move(new Vector3(horiz.x, verticalVel, horiz.z) * Time.deltaTime);
        if (!moving) hasTarget = false;
        return horiz;
    }

    Vector3 TickTransform()
    {
        Vector3 pos = transform.position;
        Vector3 to = targetPos - pos; to.y = 0f;
        float dist = to.magnitude;
        bool moving = !IsDead && !actionBusy && hasTarget && dist > stopDistance;
        if (moving && to.sqrMagnitude > 1e-6f)
        {
            var rot = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
            Vector3 step = transform.forward * moveSpeed * Time.deltaTime;
            if (step.magnitude > dist) step = to.normalized * dist;
            transform.position += step;
            return step / Time.deltaTime;
        }
        else { hasTarget = false; return Vector3.zero; }
    }

    Vector3 TickNavMesh()
    {
        if (!agent) return Vector3.zero;
        bool moving = !IsDead && !actionBusy && agent.hasPath && !agent.pathPending && agent.remainingDistance > stopDistance;
        if (moving)
        {
            Vector3 dir = agent.steeringTarget - transform.position; dir.y = 0f;
            if (dir.sqrMagnitude < 1e-6f) dir = agent.desiredVelocity; dir.y = 0f;
            if (dir.sqrMagnitude > 1e-6f)
            {
                var rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
            }
        }
        else if (hasTarget && agent.remainingDistance <= stopDistance) { hasTarget = false; }
        return new Vector3(agent.velocity.x, 0f, agent.velocity.z);
    }
}
