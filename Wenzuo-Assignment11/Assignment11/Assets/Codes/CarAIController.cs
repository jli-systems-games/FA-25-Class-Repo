using System.Collections.Generic;
using UnityEngine;

public class CarAIController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform arenaCenter;
    public float seekSpeed = 24f;
    public float flankSpeed = 30f;
    public float chargeSpeed = 55f;
    public float turnSpeed = 12f;
    public float maxSpeed = 60f;
    public float chargeDuration = 1.0f;
    public float recoverDuration = 0.5f;
    public float flankDistance = 8f;
    public float centerAvoidRadius = 3f;
    public float impactForce = 26f;
    public float targetSearchRadius = 80f;

    enum AIState { Seek, Flank, Charge, Recover }
    AIState state = AIState.Seek;

    static List<CarAIController> allCars = new List<CarAIController>();

    Transform target;
    float stateTimer;
    Vector3 flankPoint;

    void OnEnable()
    {
        if (!allCars.Contains(this)) allCars.Add(this);
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    void OnDisable()
    {
        allCars.Remove(this);
    }

    void FixedUpdate()
    {
        if (!rb) return;

        if (!target || Vector3.Distance(transform.position, target.position) > targetSearchRadius)
            target = FindTarget();

        switch (state)
        {
            case AIState.Seek: UpdateSeek(); break;
            case AIState.Flank: UpdateFlank(); break;
            case AIState.Charge: UpdateCharge(); break;
            case AIState.Recover: UpdateRecover(); break;
        }

        LimitSpeed();
    }

    Transform FindTarget()
    {
        float best = Mathf.Infinity;
        Transform bestT = null;

        for (int i = 0; i < allCars.Count; i++)
        {
            var c = allCars[i];
            if (c == this) continue;
            if (!c || !c.transform) continue;

            float d = Vector3.Distance(transform.position, c.transform.position);
            if (d < best && d <= targetSearchRadius)
            {
                best = d;
                bestT = c.transform;
            }
        }

        return bestT;
    }

    void UpdateSeek()
    {
        if (!target)
        {
            if (arenaCenter)
                MoveInDirection((arenaCenter.position - transform.position).normalized, seekSpeed);
            else
                MoveInDirection(transform.forward, seekSpeed);
            return;
        }

        Vector3 toTarget = (target.position - transform.position).normalized;
        Vector3 dir = toTarget;

        if (arenaCenter)
        {
            float centerDist = Vector3.Distance(transform.position, arenaCenter.position);
            if (centerDist < centerAvoidRadius)
            {
                Vector3 away = (transform.position - arenaCenter.position).normalized;
                dir = (toTarget + away * 1.8f).normalized;
            }
        }

        MoveInDirection(dir, seekSpeed);

        float angle = Vector3.Angle(transform.forward, toTarget);
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > flankDistance * 1.2f)
        {
            if (angle < 20f)
                StartCharge();
            else
                StartFlank();
        }
        else
        {
            StartCharge();
        }
    }

    void StartFlank()
    {
        if (!target)
        {
            state = AIState.Seek;
            return;
        }

        Vector3 toTarget = (target.position - transform.position).normalized;
        Vector3 side = Vector3.Cross(Vector3.up, toTarget).normalized;
        float sideSign = Random.value < 0.5f ? 1f : -1f;
        side *= sideSign;

        flankPoint = target.position + side * flankDistance;
        state = AIState.Flank;
        stateTimer = 0.7f;
    }

    void UpdateFlank()
    {
        stateTimer -= Time.fixedDeltaTime;
        if (!target)
        {
            state = AIState.Seek;
            return;
        }

        Vector3 toPoint = flankPoint - transform.position;
        if (toPoint.sqrMagnitude < 4f || stateTimer <= 0f)
        {
            StartCharge();
            return;
        }

        MoveInDirection(toPoint.normalized, flankSpeed);
    }

    void StartCharge()
    {
        state = AIState.Charge;
        stateTimer = chargeDuration;
    }

    void UpdateCharge()
    {
        stateTimer -= Time.fixedDeltaTime;

        Vector3 dir;
        if (target)
            dir = (target.position - transform.position).normalized;
        else if (arenaCenter)
            dir = (arenaCenter.position - transform.position).normalized;
        else
            dir = transform.forward;

        MoveInDirection(dir, chargeSpeed * 1.5f);

        if (stateTimer <= 0f)
            state = AIState.Seek;
    }

    void StartRecover()
    {
        state = AIState.Recover;
        stateTimer = recoverDuration;
    }

    void UpdateRecover()
    {
        stateTimer -= Time.fixedDeltaTime;

        Vector3 baseDir;
        if (arenaCenter)
            baseDir = (transform.position - arenaCenter.position).normalized;
        else
            baseDir = -transform.forward;

        baseDir = Quaternion.Euler(0f, Random.Range(-40f, 40f), 0f) * baseDir;
        MoveInDirection(baseDir, chargeSpeed);

        if (stateTimer <= 0f)
            state = AIState.Seek;
    }

    void MoveInDirection(Vector3 dir, float accel)
    {
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));

        rb.AddForce(transform.forward * accel, ForceMode.Acceleration);
    }

    void LimitSpeed()
    {
        Vector3 v = rb.linearVelocity;
        Vector3 horizontal = new Vector3(v.x, 0f, v.z);

        if (horizontal.magnitude > maxSpeed)
        {
            Vector3 clamped = horizontal.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody && other.rigidbody != rb)
        {
            Vector3 n = other.contacts[0].normal;
            other.rigidbody.AddForce(-n * impactForce, ForceMode.Impulse);
            rb.AddForce(n * impactForce, ForceMode.Impulse);
            StartRecover();
        }
    }
}
