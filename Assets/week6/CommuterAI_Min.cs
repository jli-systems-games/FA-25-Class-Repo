// CommuterAI_Min.cs
using UnityEngine;
using UnityEngine.AI;

public class CommuterAI_Min : MonoBehaviour
{
    [Header("Refs")]
    public Transform trainInterior;   // 열차 내부 트리거(BoxCollider isTrigger=On)
    public Transform trainRoot;       // 열차 비주얼/애니메이션의 루트(따라갈 기준)

    [Header("On Board Behavior")]
    public bool disableAgentOnBoard = true;
    public bool disableColliderOnBoard = true;  // 재충돌 방지
    public bool makeChildKinematic = true;      // 물리 안정화
    public bool followWithoutParent = true;     // ✅ 부모로 붙이지 않고 오프셋 유지하며 따라가기

    private NavMeshAgent agent;
    private bool boarded;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        boarded = false;
        if (trainInterior && agent) agent.SetDestination(trainInterior.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (boarded) return;

        // trainInterior 트리거에 들어왔는지 확인
        if (other.transform == trainInterior)
        {
            boarded = true;

            // 네비 끄기
            if (disableAgentOnBoard && agent && agent.enabled)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }

            // 물리 안정화
            var rb = GetComponent<Rigidbody>();
            if (makeChildKinematic && rb)
            {
                if (!rb.isKinematic) // kinematic일 땐 velocity 건드리지 않음
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                rb.isKinematic = true;
            }

            // 재충돌 방지
            var col = GetComponent<Collider>();
            if (disableColliderOnBoard && col) col.enabled = false;

            // ✅ 기차 따라가기 처리
            if (trainRoot)
            {
                if (followWithoutParent)
                {
                    // 부모로 붙이지 않고 오프셋을 유지하며 따라가게 함
                    var follow = GetComponent<FollowTrain>();
                    if (follow == null) follow = gameObject.AddComponent<FollowTrain>();
                    follow.train = trainRoot;
                    follow.offset = trainRoot.InverseTransformPoint(transform.position);
                    follow.enabled = true;
                }
                else
                {
                    // (옵션) 진짜로 부모로 붙이기
                    transform.SetParent(trainRoot, true);
                }
            }

            // TODO: 점수/카운트 등 추가 연출
        }
    }
}

/// <summary>
/// 기차(Transform train)의 로컬 오프셋을 유지하며 매 프레임 따라가는 컴포넌트
/// 부모-자식 관계를 쓰지 않아 Animator/Timeline RootMotion과 충돌이 적음
/// </summary>
public class FollowTrain : MonoBehaviour
{
    public Transform train;
    public Vector3 offset;

    void LateUpdate()
    {
        if (!train) return;
        transform.position = train.TransformPoint(offset);
        // 회전까지 맞추고 싶으면 아래 줄 주석 해제
        // transform.rotation = train.rotation;
    }
}
