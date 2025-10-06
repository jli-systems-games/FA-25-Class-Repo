// CommuterAI_Min.cs
using UnityEngine;
using UnityEngine.AI;

public class CommuterAI_Min : MonoBehaviour
{
    [Header("Refs")]
    public Transform trainInterior;   // 열차 내부 트리거(BoxCollider isTrigger = On)
    public Transform trainRoot;       // 열차 루트(달리고 움직이는 부모)

    [Header("On Board Behavior")]
    public bool disableAgentOnBoard = true;
    public bool disableColliderOnBoard = true;  // 재충돌 방지
    public bool makeChildKinematic = true;      // 물리 안정화

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

            // 네비 꺼서 위치 제어 중단
            if (disableAgentOnBoard && agent && agent.enabled)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }

            // 물리 안정화(선택)
            var rb = GetComponent<Rigidbody>();
            if (makeChildKinematic && rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // 재충돌 방지(선택)
            var col = GetComponent<Collider>();
            if (disableColliderOnBoard && col) col.enabled = false;

            // 열차의 자식으로 붙이기 (월드 좌표 유지)
            if (trainRoot) transform.SetParent(trainRoot, true);

            // 필요시: 좌석 포인트로 스냅하고 싶다면 여기에 위치 지정 추가
            // transform.position = seatPoint.position;

            // 탑승 성공 후 추가 로직(점수/카운트 등) 여기에
            // GameManager.Instance.AddBoardedCount(1);
        }
    }
}
