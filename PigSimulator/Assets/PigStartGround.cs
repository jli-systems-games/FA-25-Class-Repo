using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PigStartGround : MonoBehaviour
{
    public LayerMask groundMask = ~0;
    public float castUp = 2f;
    public float castDown = 6f;
    public float skin = 0.02f;
    public float settleTime = 0.5f;
    public float settleGravity = 40f;

    Rigidbody rb;
    CapsuleCollider cap;
    float settleUntil;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cap = GetComponent<CapsuleCollider>();
        rb.isKinematic = false;
        rb.detectCollisions = true;
        cap.enabled = true;
        cap.isTrigger = false;
    }

    void Start()
    {
        Vector3 center = transform.position + cap.center;
        float r = cap.radius;
        float half = Mathf.Max(r + 0.001f, cap.height * 0.5f - r);
        Vector3 top = center + Vector3.up * half;
        Vector3 bottom = center - Vector3.up * half;

        Vector3 startTop = top + Vector3.up * castUp;
        Vector3 startBottom = bottom + Vector3.up * castUp;

        if (Physics.CapsuleCast(startTop, startBottom, r, Vector3.down, out RaycastHit hit, castUp + castDown, groundMask, QueryTriggerInteraction.Ignore)
            || Physics.CapsuleCast(startTop, startBottom, r, Vector3.down, out hit, castUp + castDown, ~0, QueryTriggerInteraction.Ignore))
        {
            float bottomToCenter = half;
            float y = hit.point.y + bottomToCenter + skin;
            Vector3 p = transform.position;
            p.y = y;
            rb.position = p;
            rb.linearVelocity = Vector3.zero;
        }
        settleUntil = Time.time + settleTime;
    }

    void FixedUpdate()
    {
        if (Time.time < settleUntil)
        {
            rb.AddForce(Vector3.down * settleGravity, ForceMode.Acceleration);
        }
    }
}
