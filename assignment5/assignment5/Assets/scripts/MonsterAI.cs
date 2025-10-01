using UnityEngine;
using UnityEngine.AI;

public enum MonsterState { Idle, Investigate, Hunt }

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("引用")]
    public Transform player;
    public LayerMask sightMask; 

    [Header("感知/追击")]
    public float sightRange = 7f;
    public float catchDistance = 1.4f;
    public float hearingRadius = 25f;
    public float investigateDuration = 6f;

    [Header("爆表瞬移（Jumpscare）")]
    public bool flashOnCritical = true;
    public float flashDistance = 1.6f;
    public float flashCooldown = 3f;
    public AudioSource screamAudio; 

    NavMeshAgent agent;
    MonsterState state = MonsterState.Idle;
    Vector3 lastHeardPos;
    float stateTimer;
    float flashTimer = 0f;

    void OnEnable()
    {
        if (NoiseSystem.I != null)
            NoiseSystem.I.OnNoisePulse += OnNoisePulse;
    }

    void OnDisable()
    {
        if (NoiseSystem.I != null)
            NoiseSystem.I.OnNoisePulse -= OnNoisePulse;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        state = MonsterState.Idle;
    }

    void Update()
    {
        if (!player) return;

        if (flashTimer > 0f) flashTimer -= Time.deltaTime;

        if (flashOnCritical && NoiseSystem.I != null && NoiseSystem.I.IsCritical() && flashTimer <= 0f)
        {
            TryFlashToPlayerFront();
            flashTimer = flashCooldown;
            state = MonsterState.Hunt;
        }
        if ((NoiseSystem.I != null && NoiseSystem.I.IsCritical()) || CanSeePlayer())
            state = MonsterState.Hunt;

        switch (state)
        {
            case MonsterState.Idle:
                if (agent.hasPath && agent.remainingDistance < 0.5f) agent.ResetPath();
                break;

            case MonsterState.Investigate:
                stateTimer -= Time.deltaTime;
                if (agent.remainingDistance < 0.6f && stateTimer <= 0f)
                    state = MonsterState.Idle;
                break;

            case MonsterState.Hunt:
                HuntUpdate();
                break;
        }
        if (Vector3.Distance(transform.position, player.position) <= catchDistance)
        {
            if (GameFlow.I != null) GameFlow.I.GameOver();
        }
    }

    void HuntUpdate()
    {
        var hide = player.GetComponent<PlayerHideState>();
        if (hide && hide.IsHidden && !CanSeePlayer())
        {
            if (agent.remainingDistance < 0.5f)
                agent.SetDestination(player.position + Random.insideUnitSphere * 2f);
            return;
        }
        agent.SetDestination(player.position);
    }

    void OnNoisePulse(Vector3 pos, float strength)
    {
        float dist = Vector3.Distance(transform.position, pos);
        if (dist <= hearingRadius * Mathf.Lerp(0.5f, 1.5f, strength))
        {
            lastHeardPos = pos;
            state = MonsterState.Investigate;
            stateTimer = investigateDuration;
            agent.SetDestination(lastHeardPos);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 origin = transform.position + Vector3.up * 0.6f;
        Vector3 target = player.position + Vector3.up * 0.6f;
        Vector3 dir = target - origin;
        if (dir.magnitude > sightRange) return false;

        if (Physics.Raycast(origin, dir.normalized, out var hit, sightRange, ~sightMask))
            return hit.collider.CompareTag("Player");

        return false;
    }

    void TryFlashToPlayerFront()
    {
        Vector3 fwd = player.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 wanted = player.position + fwd * flashDistance;

        if (NavMesh.SamplePosition(wanted, out var hit, 2f, NavMesh.AllAreas))
            agent.Warp(hit.position);
        else if (NavMesh.SamplePosition(player.position, out var hit2, 2f, NavMesh.AllAreas))
            agent.Warp(hit2.position);

        if (screamAudio) screamAudio.Play();

        var camShake = Camera.main ? Camera.main.GetComponent<CameraShake>() : null;
        if (camShake) camShake.Shake(0.4f, 0.3f);

        if (GameFlow.I != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= catchDistance + 0.2f) 
            {
                GameFlow.I.GameOver();
                return;
            }
        }

    }
}
