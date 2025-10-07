// CommuterAI_Min.cs
using UnityEngine;
using UnityEngine.AI;

public class CommuterAI_Min : MonoBehaviour
{
    [Header("Refs")]
    public Transform trainInterior;   // 열차 내부 트리거 (BoxCollider, IsTrigger = On)
    public Transform trainRoot;       // 열차 루트(애니 루트)

    [Header("On Board Behavior")]
    public bool disableAgentOnBoard = true;
    public bool disableColliderOnBoard = true;  // 재충돌 방지
    public bool makeChildKinematic = true;      // 물리 안정화
    public bool followWithoutParent = true;     // 부모로 붙이지 않고 오프셋 추적

    [Header("Respawn (Interior에 닿으면)")]
    public bool despawnOnInterior = true;       // ✅ true면 사라졌다가 원위치 리스폰
    public float respawnDelay = 0.4f;
    public Transform respawnPointOverride;      // 비우면 시작 위치로 리스폰
    public string interiorTag = "Interior";     // (선택) 트리거에 이 태그 부여

    private NavMeshAgent agent;
    private Rigidbody rb;
    private Collider col;
    private bool boarded;

    // 시작 위치 기억
    private Vector3 spawnPos;
    private Quaternion spawnRot;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        spawnPos = transform.position;
        spawnRot = transform.rotation;
    }

    void OnEnable()
    {
        boarded = false;
        if (trainInterior && agent && agent.enabled)
            agent.SetDestination(trainInterior.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (boarded) return;

        bool hitInterior =
            (trainInterior && (other.transform == trainInterior || other.transform.IsChildOf(trainInterior))) ||
            (!string.IsNullOrEmpty(interiorTag) && other.CompareTag(interiorTag));

        if (!hitInterior) return;

        if (despawnOnInterior)
        {
            // ▶ 인테리어 닿으면 사라졌다가 원위치 리스폰
            StartCoroutine(RespawnRoutine());
            return;
        }

        // ▶ 기존 탑승/추적 로직
        boarded = true;

        if (disableAgentOnBoard && agent && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (makeChildKinematic && rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (disableColliderOnBoard && col) col.enabled = false;

        if (trainRoot)
        {
            if (followWithoutParent)
            {
                var follow = GetComponent<FollowTrain>();
                if (!follow) follow = gameObject.AddComponent<FollowTrain>();
                follow.train = trainRoot;
                follow.offset = trainRoot.InverseTransformPoint(transform.position);
                follow.enabled = true;
            }
            else
            {
                transform.SetParent(trainRoot, true);
            }
        }
    }

    System.Collections.IEnumerator RespawnRoutine()
    {
        boarded = true; // 중복 처리 방지

        // 비활성 처리(보여지는 것/충돌/네비)
        ToggleVisuals(false);
        if (col) col.enabled = false;
        if (agent && agent.enabled) { agent.isStopped = true; agent.enabled = false; }
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(respawnDelay);

        // 위치 초기화
        var p = respawnPointOverride ? respawnPointOverride.position : spawnPos;
        var r = respawnPointOverride ? respawnPointOverride.rotation : spawnRot;
        transform.SetPositionAndRotation(p, r);

        // 재활성
        ToggleVisuals(true);
        if (col) col.enabled = true;

        if (agent)
        {
            agent.enabled = true;
            agent.Warp(transform.position);
            agent.isStopped = false;
            if (trainInterior) agent.SetDestination(trainInterior.position);
        }

        boarded = false;
    }

    void ToggleVisuals(bool on)
    {
        foreach (var rd in GetComponentsInChildren<Renderer>(true)) rd.enabled = on;
    }
}

/// <summary>부모-자식으로 묶지 않고 기차의 로컬 오프셋을 유지하며 따라감</summary>
public class FollowTrain : MonoBehaviour
{
    public Transform train;
    public Vector3 offset;
    void LateUpdate()
    {
        if (!train) return;
        transform.position = train.TransformPoint(offset);
        // 회전까지 맞추려면 아래 주석 해제
        // transform.rotation = train.rotation;
    }
}
