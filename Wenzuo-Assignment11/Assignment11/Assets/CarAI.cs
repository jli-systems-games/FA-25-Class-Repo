using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CarHealth))]
public class CarSimpleAI : MonoBehaviour
{
    public float accel = 20f;
    public float maxSpeed = 18f;
    public float turnLerp = 6f;
    public float ramDamageBase = 10f;
    public float minHitSpeed = 4f;
    public float jitter = 0.6f;
    public Transform platformCenter;
    public float centerBias = 10f;

    Rigidbody rb;
    Transform target;
    float nextPick;

    void Awake() { rb = GetComponent<Rigidbody>(); }
    void FixedUpdate()
    {
        if (target == null || Time.time > nextPick) PickTarget();
        Vector3 dir = transform.forward;
        if (target != null) dir = Vector3.Slerp(dir, (target.position - transform.position).withY(0).normalized, 0.6f);
        dir += Random.insideUnitSphere * jitter; dir.y = 0; dir.Normalize();
        Vector3 hvel = rb.linearVelocity.withY(0);
        Vector3 steer = dir * maxSpeed - hvel;
        rb.AddForce(steer.normalized * accel, ForceMode.Acceleration);
        float ang = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.AngleAxis(ang, Vector3.up) * rb.rotation, turnLerp * Time.fixedDeltaTime * 0.25f));
        if (platformCenter != null) { Vector3 toC = (platformCenter.position - transform.position).withY(0).normalized; rb.AddForce(toC * centerBias * 0.02f, ForceMode.Acceleration); }
        Vector3 v = rb.linearVelocity; Vector3 h = v.withY(0); if (h.magnitude > maxSpeed) h = h.normalized * maxSpeed; rb.linearVelocity = new Vector3(h.x, v.y, h.z);
    }

    void PickTarget()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        Transform best = null; float bestS = -1; Vector3 p = transform.position;
        for (int i = 0; i < cars.Length; i++) { if (cars[i] == gameObject) continue; float d = Vector3.Distance(p, cars[i].transform.position); float s = 1f / (0.1f + d); if (s > bestS) { bestS = s; best = cars[i].transform; } }
        target = best; nextPick = Time.time + Random.Range(1.0f, 1.8f);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.rigidbody == null) return;
        if (!c.gameObject.CompareTag("Car")) return;
        float rel = c.relativeVelocity.magnitude; if (rel < minHitSpeed) return;
        var h = c.gameObject.GetComponent<CarHealth>(); if (h != null) h.Damage(ramDamageBase + rel * 1.0f);
    }
}

static class V3E { public static Vector3 withY(this Vector3 v, float y) { return new Vector3(v.x, y, v.z); } }
