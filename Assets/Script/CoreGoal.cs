using UnityEngine;
using UnityEngine.Events;

public class CoreGoal : MonoBehaviour
{
    public LayerMask groundMask;
    public float minY = 0f;
    public float yTolerance = 0.03f;
    public float probeRadius = 0.12f;
    public float probeOffset = 0.05f;
    public float probeInterval = 0.1f;
    public UnityEvent onCoreFailed;

    bool failed;
    Collider col;
    Rigidbody rb;
    float nextProbeTime;

    void Awake()
    {
        col = GetComponent<Collider>();
        rb  = GetComponent<Rigidbody>();
        if (col is MeshCollider mc) mc.convex = true;
    }

    void Update()
    {
        if (failed) return;

        if (transform.position.y < (minY - yTolerance))
        {
            Fail("Below minY");
            return;
        }

        if (Time.time >= nextProbeTime)
        {
            nextProbeTime = Time.time + probeInterval;
            Vector3 origin = transform.position + Vector3.down * (col.bounds.extents.y - probeOffset);
            float rayLen = probeOffset + 0.02f;
            if (Physics.SphereCast(origin, probeRadius, Vector3.down, out RaycastHit hit, rayLen, groundMask, QueryTriggerInteraction.Collide))
            {
                Fail("SphereCast hit Ground");
                return;
            }
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (failed) return;
        if (IsGround(c.collider.gameObject)) Fail("Collision with Ground");
    }
    void OnCollisionStay(Collision c)
    {
        if (failed) return;
        if (IsGround(c.collider.gameObject)) Fail("CollisionStay with Ground");
    }
    void OnTriggerEnter(Collider other)
    {
        if (failed) return;
        if (IsGround(other.gameObject)) Fail("Trigger with Ground");
    }
    void OnTriggerStay(Collider other)
    {
        if (failed) return;
        if (IsGround(other.gameObject)) Fail("TriggerStay with Ground");
    }

    bool IsGround(GameObject go)
    {
        return ((1 << go.layer) & groundMask) != 0;
    }

    void Fail(string reason)
    {
        if (failed) return;
        failed = true;
        onCoreFailed?.Invoke();
    }
}