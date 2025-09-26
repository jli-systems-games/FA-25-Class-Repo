using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class TPigRBController : MonoBehaviour
{
    public LayerMask groundMask = ~0;

    public float moveSpeed = 4.2f;
    public float sprintMult = 1.55f;
    public float accel = 26f;
    [Range(0f, 1f)] public float airControl = 0.3f;
    public float maxSlopeAngle = 55f;

    public float jumpHeight = 1.1f;

    public float probeSkin = 0.02f;
    public float probeExtra = 0.35f;
    public float probeRadiusScale = 0.92f;

    public float mouseXSens = 140f;

    Rigidbody rb;
    CapsuleCollider cap;
    bool grounded, wasGrounded;
    bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cap = GetComponent<CapsuleCollider>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        if (cap.center == Vector3.zero) cap.center = new Vector3(0f, cap.height * 0.5f, 0f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) jumpPressed = true;

        float mx = Mathf.Clamp(Input.GetAxis("Mouse X"), -5f, 5f) * mouseXSens * Time.deltaTime;
        transform.Rotate(0f, mx, 0f);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !locked;
        }
    }

    void FixedUpdate()
    {
        wasGrounded = grounded;
        grounded = CheckGround(out _);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 raw = new Vector2(h, v);
        if (raw.sqrMagnitude > 1f) raw.Normalize();
        bool sprint = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = moveSpeed * (sprint ? sprintMult : 1f);
        Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, fwd);
        Vector3 wishDir = (fwd * raw.y + right * raw.x);
        if (wishDir.sqrMagnitude > 1e-4f) wishDir.Normalize();

        Vector3 vel = rb.linearVelocity;
        Vector3 velH = new Vector3(vel.x, 0f, vel.z);
        Vector3 targetH = wishDir * targetSpeed;
        float control = grounded ? 1f : airControl;
        Vector3 deltaV = (targetH - velH);
        Vector3 force = deltaV * (accel * control);
        rb.AddForce(force, ForceMode.Acceleration);

        if (jumpPressed && grounded)
        {
            float vJump = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * Mathf.Max(0.01f, jumpHeight));
            Vector3 vNew = rb.linearVelocity; vNew.y = vJump; rb.linearVelocity = vNew;
        }
        jumpPressed = false;
    }

    bool CheckGround(out Vector3 normal)
    {
        float r = cap.radius * probeRadiusScale;
        float bottomY = transform.position.y + cap.center.y - cap.height * 0.5f + r;
        Vector3 origin = new Vector3(transform.position.x, bottomY + probeSkin, transform.position.z);
        bool hit = Physics.SphereCast(origin, r, Vector3.down, out RaycastHit rh, probeExtra, groundMask, QueryTriggerInteraction.Ignore);
        if (hit)
        {
            normal = rh.normal;
            float angle = Vector3.Angle(Vector3.up, normal);
            return angle <= maxSlopeAngle;
        }
        normal = Vector3.up;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        var c = GetComponent<CapsuleCollider>();
        if (!c) return;
        float r = c.radius * probeRadiusScale;
        float bottomY = transform.position.y + c.center.y - c.height * 0.5f + r;
        Vector3 origin = new Vector3(transform.position.x, bottomY + probeSkin, transform.position.z);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, r);
        Gizmos.DrawLine(origin, origin + Vector3.down * probeExtra);
    }
}
