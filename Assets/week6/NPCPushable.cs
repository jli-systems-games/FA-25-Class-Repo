// NPCPushable.cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class NPCPushable : MonoBehaviour
{
    [Header("Knockback")]
    public float knockoutTime = 0.22f;    // 물리에 맡길 시간
    public float frictionMultiplier = 0.6f; // 감속 계수
    public float maxPushSpeed = 6f;       // 넉백 중 최고 속도 제한

    Rigidbody rb;
    NavMeshAgent agent;
    bool beingPushed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        // 기본은 네비가 위치 제어, 물리는 비활성
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    public void ApplyKnockback(Vector3 dir, float force)
    {
        if (!isActiveAndEnabled) return;
        if (beingPushed) return;
        StartCoroutine(KnockbackRoutine(dir.normalized, force));
    }

    IEnumerator KnockbackRoutine(Vector3 dir, float force)
    {
        beingPushed = true;

        // 네비 잠시 off
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // 물리 on
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(dir * force, ForceMode.Impulse);
        if (rb.linearVelocity.magnitude > maxPushSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxPushSpeed;

        float t = 0f;
        while (t < knockoutTime)
        {
            // 마찰로 감속
            rb.linearVelocity *= Mathf.Pow(frictionMultiplier, Time.deltaTime * 60f);
            t += Time.deltaTime;
            yield return null;
        }

        // 물리 off, 네비 복귀(현재 위치로 워프)
        rb.isKinematic = true;
        if (!agent.enabled) agent.enabled = true;
        agent.Warp(rb.position);
        agent.isStopped = false;

        beingPushed = false;
    }
}
