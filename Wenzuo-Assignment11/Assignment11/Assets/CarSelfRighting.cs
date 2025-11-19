using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarUprightClamp : MonoBehaviour
{
    public LayerMask groundMask = ~0;
    public float rayLen = 3f;
    public float downforce = 30f;
    public Vector3 comOffset = new Vector3(0, -0.3f, 0);

    public float softMaxTiltDeg = 20f;
    public float hardUprightDeg = 55f;
    public float alignLerp = 8f;
    public float liftWhenHard = 0.4f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += comOffset;
    }

    void FixedUpdate()
    {
        Vector3 up = GetGroundUp();
        Vector3 fwdOnPlane = Vector3.ProjectOnPlane(transform.forward, up);
        if (fwdOnPlane.sqrMagnitude < 1e-4f) fwdOnPlane = Vector3.ProjectOnPlane(transform.right, up);
        if (fwdOnPlane.sqrMagnitude < 1e-4f) fwdOnPlane = Vector3.forward;

        Quaternion targetRot = Quaternion.LookRotation(fwdOnPlane.normalized, up);

        float tilt = Mathf.Acos(Mathf.Clamp(Vector3.Dot(transform.up, up), -1f, 1f)) * Mathf.Rad2Deg;

        if (tilt > hardUprightDeg)
        {
            rb.linearVelocity = rb.linearVelocity;
            rb.angularVelocity = Vector3.zero;
            rb.MovePosition(transform.position + up * liftWhenHard);
            rb.MoveRotation(targetRot);
        }
        else if (tilt > softMaxTiltDeg)
        {
            Quaternion q = Quaternion.Slerp(rb.rotation, targetRot, 1f - Mathf.Exp(-alignLerp * Time.fixedDeltaTime));
            rb.MoveRotation(q);
        }

        rb.AddForce(-up * downforce, ForceMode.Acceleration);
    }

    Vector3 GetGroundUp()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(origin, Vector3.down, out var hit, rayLen, groundMask))
            return hit.normal.normalized;
        return Vector3.up;
    }
}
