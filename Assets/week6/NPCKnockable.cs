// NPCKnockable.cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class NPCKnockable : MonoBehaviour
{
    public float recoverTime = 0.25f;   // 날아간 뒤 복구까지 시간
    public float maxKnockSpeed = 12f;   // 넉백 중 최고 속도 클램프

    Rigidbody rb;
    NavMeshAgent agent;
    bool knocking;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        // 보통 NavMeshAgent와 함께 kinematic=true로 둠
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    public void Knock(Vector3 dir, float power)
    {
        if (!isActiveAndEnabled || knocking) return;
        StartCoroutine(KnockRoutine(dir.normalized, power));
    }

    IEnumerator KnockRoutine(Vector3 dir, float power)
    {
        knocking = true;

        // 네비 잠시 끄기
        if (agent && agent.enabled) { agent.isStopped = true; agent.enabled = false; }

        // 물리 켜고 힘 주기
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(dir * power, ForceMode.Impulse);

        // 속도 상한
        if (rb.linearVelocity.magnitude > maxKnockSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxKnockSpeed;

        yield return new WaitForSeconds(recoverTime);

        // 복구: 다시 네비로
        rb.isKinematic = true;
        if (agent)
        {
            agent.enabled = true;
            agent.Warp(rb.position);   // 현재 위치로 네비 동기화
            agent.isStopped = false;
        }

        knocking = false;
    }
}
