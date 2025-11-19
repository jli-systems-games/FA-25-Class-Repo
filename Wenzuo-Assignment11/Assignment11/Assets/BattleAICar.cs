using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BattleAICar : MonoBehaviour
{
    [Header("=== �˶����� ===")]
    public float accelerate = 35f;      // ���ٶ�
    public float maxSpeed = 25f;        // �����
    public float steerSpeed = 150f;     // ת���ٶ�
    public float edgeDist = 10f;        // ��Ե���

    [Header("=== AI ���� ===")]
    public float detectRange = 40f;     // ��ⷶΧ
    public LayerMask carLayer = 1 << 6; // Car Layer

    private Rigidbody rb;
    private Transform nearestEnemy;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0;
        rb.angularDamping = 0.05f;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        gameObject.layer = 6;
        gameObject.tag = "Car";
    }

    void FixedUpdate()
    {
        FindNearestEnemy();

        // === ת�� ===
        float h = 0f;
        if (nearestEnemy != null)
        {
            Vector3 dir = nearestEnemy.position - transform.position;
            dir.y = 0;
            float angle = Vector3.SignedAngle(transform.forward, dir.normalized, Vector3.up);
            h = Mathf.Clamp(angle, -steerSpeed, steerSpeed) * 0.01f;
        }
        else
        {
            h = Random.Range(-0.5f, 0.5f);
        }
        rb.angularVelocity = new Vector3(0, h, 0);

        // === ǰ�� ===
        Vector3 force = transform.forward * accelerate;
        rb.AddForce(force, ForceMode.Acceleration);

        // === ���� ===
        Vector3 flatVel = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);
        if (flatVel.magnitude > maxSpeed)
        {
            rb.linearVelocity = flatVel.normalized * maxSpeed + Vector3.up * rb.linearVelocity.y;
        }

        // === ��Եɲ�� ===
        if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, -transform.right, edgeDist))
        {
            rb.AddForce(-rb.linearVelocity * 5f, ForceMode.Acceleration);
            rb.angularVelocity = new Vector3(0, -steerSpeed * 0.5f, 0);
        }

        // === �������� ===
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    void FindNearestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, carLayer);
        nearestEnemy = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.transform == transform) continue;
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = hit.transform;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}