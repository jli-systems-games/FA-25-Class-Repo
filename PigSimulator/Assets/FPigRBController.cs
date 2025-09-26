using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class FPigRBControllerSmoothGroundFix : MonoBehaviour
{
    public Transform cameraTransform;
    public LayerMask groundMask = ~0;
    public float moveSpeed = 4.2f;
    public float sprintMult = 1.55f;
    public float accel = 26f;
    [Range(0f, 1f)] public float airControl = 0.35f;
    public float maxSlopeAngle = 55f;
    public float jumpHeight = 1.1f;
    public float probeSkin = 0.04f;
    public float probeExtra = 0.5f;
    public float probeRadiusScale = 0.95f;
    public float mouseXSens = 120f;
    public float mouseYSens = 0.95f;
    public float minPitch = -80f;
    public float maxPitch = 80f;
    public float lookSmoothing = 0.12f;
    public bool snapOnStart = true;
    public float snapRayHeight = 5f;
    public float snapRayDistance = 50f;

    Rigidbody rb;
    CapsuleCollider cap;
    float pitch;
    float _yawSm, _pitchSm;
    bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cap = GetComponent<CapsuleCollider>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        if (!cameraTransform) cameraTransform = GetComponentInChildren<Camera>()?.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        if (!snapOnStart) return;
        Vector3 origin = transform.position + Vector3.up * snapRayHeight;
        if (Physics.Raycast(origin, Vector3.down, out var hit, snapRayDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            float r = cap.radius * probeRadiusScale;
            float bottomToCenter = cap.height * 0.5f - r;
            transform.position = new Vector3(transform.position.x, hit.point.y + bottomToCenter + 0.01f, transform.position.z);
            rb.linearVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            float vJump = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * Mathf.Max(0.01f, jumpHeight));
            Vector3 vNew = rb.linearVelocity; vNew.y = vJump; rb.linearVelocity = vNew;
        }

        float rawX = Input.GetAxis("Mouse X");
        float rawY = Input.GetAxis("Mouse Y");
        float mx = Mathf.Clamp(rawX, -5f, 5f) * mouseXSens * Time.deltaTime;
        float my = Mathf.Clamp(rawY, -5f, 5f) * mouseYSens;

        if (lookSmoothing > 0f)
        {
            float k = 1f - Mathf.Exp(-Time.deltaTime / lookSmoothing);
            _yawSm = Mathf.Lerp(_yawSm, mx, k);
            _pitchSm = Mathf.Lerp(_pitchSm, my, k);
        }
        else
        {
            _yawSm = mx; _pitchSm = my;
        }

        transform.Rotate(0f, _yawSm, 0f);
        pitch = Mathf.Clamp(pitch - _pitchSm, minPitch, maxPitch);
        if (cameraTransform) cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !locked;
        }
    }

    void FixedUpdate()
    {
        grounded = CheckGround();

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
        rb.AddForce(deltaV * (accel * control), ForceMode.Acceleration);
    }

    bool CheckGround()
    {
        float r = cap.radius * probeRadiusScale;
        Vector3 center = transform.position + cap.center;
        Vector3 bottom = center + Vector3.down * (cap.height * 0.5f - r);
        Vector3 origin = bottom + Vector3.up * probeSkin;

        if (Physics.SphereCast(origin, r, Vector3.down, out RaycastHit rh, probeExtra, groundMask, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(Vector3.up, rh.normal);
            if (angle <= maxSlopeAngle) return true;
        }
        if (Physics.SphereCast(origin, r, Vector3.down, out rh, probeExtra, ~0, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(Vector3.up, rh.normal);
            if (angle <= maxSlopeAngle) return true;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        var c = GetComponent<CapsuleCollider>();
        if (!c) return;
        float r = c.radius * probeRadiusScale;
        Vector3 center = transform.position + c.center;
        Vector3 bottom = center + Vector3.down * (c.height * 0.5f - r);
        Vector3 origin = bottom + Vector3.up * probeSkin;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, r);
        Gizmos.DrawLine(origin, origin + Vector3.down * probeExtra);
    }
}
