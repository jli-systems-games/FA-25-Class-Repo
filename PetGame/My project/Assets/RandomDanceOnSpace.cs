using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class CrewController_DanceMoveAttack_ReloadAndSfx : MonoBehaviour
{
    public enum MoveMode { CharacterController, NavMeshAgent, TransformAdditive }

    [Header("References")]
    public Animator animator;
    public Camera cam;
    public LayerMask groundMask = ~0;

    [Header("Movement")]
    public MoveMode moveMode = MoveMode.CharacterController;
    public CharacterController characterController;
    public NavMeshAgent agent;
    public float moveSpeed = 3.2f;
    public float turnSpeed = 9f;
    public float stopDistance = 0.12f;
    public float gravity = -18f;
    public bool faceOnlyY = true;

    [Header("Keys")]
    public KeyCode danceKey = KeyCode.Space; // 切舞
    public KeyCode reloadKey = KeyCode.R;    // 重载场景
    public int moveMouseButton = 0;          // 左键=0

    // —— 每个动作都能配置 SFX、音量、偏移、是否循环 ——
    [Serializable]
    public struct ClipSfx
    {
        public AnimationClip clip;
        public AudioClip sfx;
        [Range(0f, 1f)] public float volume;
        [Tooltip("音频起始偏移（秒），用于对齐动作起手/击打点等。")]
        public float startOffset;
        [Tooltip("当该动作仍在播放（未切走）时，音频是否循环。建议：Dance 勾选，Attack 不勾。")]
        public bool loopWhileActive;

        public ClipSfx(AnimationClip c, AudioClip a, float vol = 1f, float offset = 0f, bool loop = false)
        { clip = c; sfx = a; volume = vol; startOffset = offset; loopWhileActive = loop; }
    }

    [Header("Manual Lists (Drag here)")]
    public ClipSfx[] danceClips;   // 建议：loopWhileActive = true
    public ClipSfx[] attackClips;  // 建议：loopWhileActive = false

    [Header("Optional Auto-Scan (Resources)")]
    public bool alsoScanResources = false;
    public string danceKeyword = "Dance";
    public string attackKeyword = "Attack";

    [Header("Blend & Switch")]
    public float crossFadeDuration = 0.18f;
    public bool danceSequential = true;
    public bool avoidRepeatRandom = true;

    [Header("Animator Param (Optional)")]
    public string speedParamName = "Speed"; // 改：始终驱动Speed，保证停下就回Idle

    [Header("Attack Gating")]
    public string enemyTag = "Enemy";
    public float attackCooldown = 0.8f;
    public float attackLockMin = 0.4f;
    public float attackLockExtra = 0.0f;
    public bool stopMoveOnAttack = true;

    // ---------- internal ----------
    struct Entry
    {
        public AnimationClip clip;
        public AudioClip sfx;
        public float volume;
        public float startOffset;
        public bool loopWhileActive;
        public string name;
        public float length;
    }

    List<Entry> _dances = new();
    List<Entry> _attacks = new();
    int _danceIndex = -1, _danceLastRand = -1;
    int _hashSpeed;

    // Playables
    PlayableGraph _graph;
    AnimationPlayableOutput _output;
    AnimationMixerPlayable _mixer;
    AnimationClipPlayable _clipA, _clipB;
    int _activePort = 0;
    float _fadeTimer = 0f, _fadeDur = 0f;

    // Audio（互斥）
    AudioSource _audio;
    AnimationClip _currentPlayingClip; // 用于判断“是否仍在播放同一动作”

    // Move state
    Vector3 _targetPos;
    bool _hasTarget = false;
    float _verticalVel = 0f;

    // Attack state
    readonly HashSet<Collider> _enemiesInRange = new();
    float _nextAttackTime = 0f;
    float _lockUntil = 0f;

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

        _audio = GetComponent<AudioSource>();
        if (!_audio) _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;

        _hashSpeed = Animator.StringToHash(string.IsNullOrEmpty(speedParamName) ? "Speed" : speedParamName);
        animator.SetFloat(_hashSpeed, 0f); // 确保场景初始是 Idle

        BuildLists();
        SetupGraph();

        if (moveMode == MoveMode.NavMeshAgent && agent)
        {
            agent.updateRotation = false;
            agent.updatePosition = true;
        }
    }

    void OnEnable() { if (_graph.IsValid()) _graph.Play(); }
    void OnDisable() { if (_graph.IsValid()) _graph.Stop(); }
    void OnDestroy() { if (_graph.IsValid()) _graph.Destroy(); }

    void Update()
    {
        // 左键：只移动
        if (Input.GetMouseButtonDown(moveMouseButton))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit, 400f, groundMask))
                SetDestination(hit.point);
        }

        // 空格：切舞（攻击锁期间忽略）
        if (Input.GetKeyDown(danceKey) && Time.time >= _lockUntil)
            SwitchDance();

        // R：重载当前场景（100% 回Idle）
        if (Input.GetKeyDown(reloadKey))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // —— 移动推进：始终用速度驱动 Animator.Speed（关键修复）——
        Vector3 horizVel = Vector3.zero;
        switch (moveMode)
        {
            case MoveMode.CharacterController: horizVel = TickCharacterController(); break;
            case MoveMode.TransformAdditive: horizVel = TickTransform(); break;
            case MoveMode.NavMeshAgent: horizVel = TickNavMesh(); break;
        }
        if (animator && _hashSpeed != 0)
            animator.SetFloat(_hashSpeed, horizVel.magnitude, 0.12f, Time.deltaTime);

        // CrossFade 梯度
        if (_fadeDur > 0f && _fadeTimer < _fadeDur)
        {
            _fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_fadeTimer / _fadeDur);
            if (_activePort == 0) { _mixer.SetInputWeight(0, 1f - t); _mixer.SetInputWeight(1, t); }
            else { _mixer.SetInputWeight(0, t); _mixer.SetInputWeight(1, 1f - t); }
        }
    }

    // ---------- Trigger 攻击 ----------
    void OnTriggerEnter(Collider other)
    {
        if (!other || !other.CompareTag(enemyTag)) return;
        _enemiesInRange.Add(other);
        TryAttack();
    }
    void OnTriggerStay(Collider other)
    {
        if (!other || !other.CompareTag(enemyTag)) return;
        TryAttack();
    }
    void OnTriggerExit(Collider other)
    {
        if (!other || !other.CompareTag(enemyTag)) return;
        _enemiesInRange.Remove(other);
    }

    void TryAttack()
    {
        if (_attacks.Count == 0) return;
        if (_enemiesInRange.Count == 0) return;
        if (Time.time < _nextAttackTime) return;

        PlayRandomAttack();
    }

    // ---------- 列表 ----------
    void BuildLists()
    {
        _dances.Clear(); _attacks.Clear();

        // 手动（带音频槽）
        if (danceClips != null)
        {
            foreach (var cs in danceClips)
                if (cs.clip)
                    _dances.Add(new Entry
                    {
                        clip = cs.clip,
                        sfx = cs.sfx,
                        volume = (cs.volume <= 0 ? 1f : cs.volume),
                        startOffset = Mathf.Max(0, cs.startOffset),
                        loopWhileActive = cs.loopWhileActive,
                        name = cs.clip.name,
                        length = Mathf.Max(cs.clip.length, 0.01f)
                    });
        }
        if (attackClips != null)
        {
            foreach (var cs in attackClips)
                if (cs.clip)
                    _attacks.Add(new Entry
                    {
                        clip = cs.clip,
                        sfx = cs.sfx,
                        volume = (cs.volume <= 0 ? 1f : cs.volume),
                        startOffset = Mathf.Max(0, cs.startOffset),
                        loopWhileActive = cs.loopWhileActive,
                        name = cs.clip.name,
                        length = Mathf.Max(cs.clip.length, 0.01f)
                    });
        }

        // 可选：扫描 Resources（这些没有音频绑定）
        if (alsoScanResources)
        {
            var res = Resources.LoadAll<AnimationClip>("");
            if (res != null)
            {
                foreach (var c in res)
                {
                    if (!c || c.legacy) continue;
                    if (danceKeyword.Length > 0 && c.name.IndexOf(danceKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        _dances.Add(new Entry { clip = c, sfx = null, volume = 1f, startOffset = 0f, loopWhileActive = true, name = c.name, length = Mathf.Max(c.length, 0.01f) });
                    if (attackKeyword.Length > 0 && c.name.IndexOf(attackKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        _attacks.Add(new Entry { clip = c, sfx = null, volume = 1f, startOffset = 0f, loopWhileActive = false, name = c.name, length = Mathf.Max(c.length, 0.01f) });
                }
            }
        }

        if (_dances.Count == 0) Debug.LogWarning("[Crew] 没找到舞蹈 Clip。");
        if (_attacks.Count == 0) Debug.LogWarning("[Crew] 没找到攻击 Clip。");
    }

    // ---------- Playables ----------
    void SetupGraph()
    {
        if (_graph.IsValid()) _graph.Destroy();

        _graph = PlayableGraph.Create("CrewGraph");
        _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        _output = AnimationPlayableOutput.Create(_graph, "AnimOutput", animator);
        _output.SetWeight(0f); // 初始让 Animator 控制（Idle）

        _mixer = AnimationMixerPlayable.Create(_graph, 2, true);
        _output.SetSourcePlayable(_mixer);

        _clipA = AnimationClipPlayable.Create(_graph, null);
        _clipB = AnimationClipPlayable.Create(_graph, null);
        _clipA.SetApplyFootIK(true);
        _clipB.SetApplyFootIK(true);

        _graph.Connect(_clipA, 0, _mixer, 0);
        _graph.Connect(_clipB, 0, _mixer, 1);
        _mixer.SetInputWeight(0, 1f);
        _mixer.SetInputWeight(1, 0f);
    }

    void PlayEntry(Entry e, bool instant = false)
    {
        if (e.clip == null) return;

        // 覆盖 Animator
        _output.SetWeight(1f);

        // 切动画
        if (_activePort == 0)
        {
            _clipB.Destroy();
            _clipB = AnimationClipPlayable.Create(_graph, e.clip);
            _clipB.SetApplyFootIK(true);
            _clipB.SetTime(0);
            _clipB.SetSpeed(1);

            _mixer.DisconnectInput(1);
            _graph.Connect(_clipB, 0, _mixer, 1);

            if (instant) { _mixer.SetInputWeight(0, 0f); _mixer.SetInputWeight(1, 1f); _fadeDur = 0f; _fadeTimer = 0f; }
            else { _mixer.SetInputWeight(0, 1f); _mixer.SetInputWeight(1, 0f); _fadeDur = Mathf.Max(0.01f, crossFadeDuration); _fadeTimer = 0f; }

            _activePort = 1;
        }
        else
        {
            _clipA.Destroy();
            _clipA = AnimationClipPlayable.Create(_graph, e.clip);
            _clipA.SetApplyFootIK(true);
            _clipA.SetTime(0);
            _clipA.SetSpeed(1);

            _mixer.DisconnectInput(0);
            _graph.Connect(_clipA, 0, _mixer, 0);

            if (instant) { _mixer.SetInputWeight(0, 1f); _mixer.SetInputWeight(1, 0f); _fadeDur = 0f; _fadeTimer = 0f; }
            else { _mixer.SetInputWeight(0, 0f); _mixer.SetInputWeight(1, 1f); _fadeDur = Mathf.Max(0.01f, crossFadeDuration); _fadeTimer = 0f; }

            _activePort = 0;
        }

        // —— 音频互斥 + 可选循环 + 起始偏移 ——
        if (_audio)
        {
            _audio.Stop();                 // 切掉上一个音频
            _audio.loop = e.loopWhileActive;
            _audio.volume = e.volume <= 0 ? 1f : e.volume;
            _audio.clip = e.sfx;

            if (_audio.clip)
            {
                if (e.startOffset > 0f && e.startOffset < _audio.clip.length)
                    _audio.time = e.startOffset; // 轻松手动对齐
                else
                    _audio.time = 0f;

                _audio.Play(); // 同帧起播，与动画一起启
            }
        }

        _currentPlayingClip = e.clip; // 用于判断“是否仍在播放同一动作”
    }

    // —— 切舞 / 随机攻击 ——
    void SwitchDance()
    {
        if (_dances.Count == 0) return;

        int idx;
        if (danceSequential)
        {
            idx = (_danceIndex + 1 + _dances.Count) % _dances.Count;
        }
        else
        {
            idx = UnityEngine.Random.Range(0, _dances.Count);
            if (avoidRepeatRandom && _dances.Count > 1 && idx == _danceLastRand)
                idx = (idx + 1) % _dances.Count;
            _danceLastRand = idx;
        }
        _danceIndex = idx;

        PlayEntry(_dances[idx], instant: false);
    }

    void PlayRandomAttack()
    {
        if (_attacks.Count == 0) return;
        if (Time.time < _nextAttackTime) return;

        if (stopMoveOnAttack)
        {
            _hasTarget = false;
            if (moveMode == MoveMode.NavMeshAgent && agent) { agent.ResetPath(); agent.velocity = Vector3.zero; }
        }

        int idx = UnityEngine.Random.Range(0, _attacks.Count);
        var e = _attacks[idx];
        PlayEntry(e, instant: false);

        _nextAttackTime = Time.time + attackCooldown;
        float lockLen = Mathf.Max(e.length + attackLockExtra, attackLockMin);
        _lockUntil = Time.time + lockLen;
    }

    // ---------- Movement ----------
    void SetDestination(Vector3 worldPoint)
    {
        _targetPos = worldPoint;
        if (moveMode != MoveMode.NavMeshAgent) _targetPos.y = transform.position.y;
        _hasTarget = true;

        if (moveMode == MoveMode.NavMeshAgent && agent)
            agent.SetDestination(worldPoint);
    }

    Vector3 TickCharacterController()
    {
        if (!characterController) return Vector3.zero;

        Vector3 pos = transform.position;
        Vector3 to = _targetPos - pos; to.y = 0f;
        float dist = to.magnitude;
        bool moving = _hasTarget && dist > stopDistance;

        if (moving && to.sqrMagnitude > 1e-6f)
        {
            var rot = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }

        Vector3 horiz = moving ? transform.forward * moveSpeed : Vector3.zero;

        if (characterController.isGrounded && _verticalVel < 0f) _verticalVel = -1f;
        _verticalVel += gravity * Time.deltaTime;

        characterController.Move(new Vector3(horiz.x, _verticalVel, horiz.z) * Time.deltaTime);

        if (!moving) _hasTarget = false;
        return horiz;
    }

    Vector3 TickTransform()
    {
        Vector3 pos = transform.position;
        Vector3 to = _targetPos - pos; to.y = 0f;
        float dist = to.magnitude;
        bool moving = _hasTarget && dist > stopDistance;

        if (moving && to.sqrMagnitude > 1e-6f)
        {
            var rot = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);

            Vector3 step = transform.forward * moveSpeed * Time.deltaTime;
            if (step.magnitude > dist) step = to.normalized * dist;
            transform.position += step;

            return step / Time.deltaTime;
        }
        else
        {
            _hasTarget = false;
            return Vector3.zero;
        }
    }

    Vector3 TickNavMesh()
    {
        if (!agent) return Vector3.zero;

        bool moving = agent.hasPath && !agent.pathPending && agent.remainingDistance > stopDistance;

        if (moving)
        {
            Vector3 dir = agent.steeringTarget - transform.position; dir.y = 0f;
            if (dir.sqrMagnitude < 1e-6f) dir = agent.desiredVelocity;
            dir.y = 0f;

            if (dir.sqrMagnitude > 1e-6f)
            {
                var rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
            }
        }
        else if (_hasTarget && agent.remainingDistance <= stopDistance)
        {
            _hasTarget = false;
        }

        return new Vector3(agent.velocity.x, 0f, agent.velocity.z);
    }
}
