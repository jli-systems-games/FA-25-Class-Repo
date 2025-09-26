using UnityEngine;

public class PigProceduralAnimStrong : MonoBehaviour
{
    public Rigidbody rb;
    public Transform visualRoot;

    public float bankFactor = 0.25f;
    public float pitchFactor = 0.02f;
    public float bobAmount = 0.12f;
    public float bobSpeed = 9f;
    public float poseLerp = 12f;
    public float landSquash = 0.18f;
    public float squashRecover = 10f;
    public float maxBank = 25f;
    public float maxPitch = 14f;

    Vector3 basePos;
    Quaternion baseRot;
    Vector3 lastVelH;
    Vector3 lastFwd;
    float bobT;
    float squash;
    bool wasGrounded;

    void Awake()
    {
        if (!visualRoot) visualRoot = transform;
        basePos = visualRoot.localPosition;
        baseRot = visualRoot.localRotation;
        lastVelH = Vector3.zero;
        lastFwd = rb ? rb.transform.forward : transform.forward;
    }

    void Update()
    {
        if (!rb || !visualRoot) return;

        Vector3 velH = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = velH.magnitude;

        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        Vector3 accelH = (velH - lastVelH) / dt;
        lastVelH = velH;

        Vector3 fwd = rb.transform.forward;
        float yawDelta = Vector3.SignedAngle(lastFwd, fwd, Vector3.up) / dt;
        lastFwd = fwd;

        float bank = Mathf.Clamp(-yawDelta * bankFactor, -maxBank, maxBank);
        float pitch = Mathf.Clamp(-Vector3.Dot(accelH.normalized, fwd) * pitchFactor * accelH.magnitude, -maxPitch, maxPitch);

        float s01 = Mathf.Clamp01(speed / 6f);
        bobT += Time.deltaTime * (bobSpeed + s01 * 6f);
        float bob = Mathf.Sin(bobT) * bobAmount * s01;

        bool grounded = Physics.Raycast(rb.position + Vector3.up * 0.1f, Vector3.down, out _, 0.32f, ~0, QueryTriggerInteraction.Ignore);
        if (!wasGrounded && grounded) squash = landSquash;
        wasGrounded = grounded;
        squash = Mathf.MoveTowards(squash, 0f, squashRecover * Time.deltaTime);

        Vector3 targetPos = basePos + new Vector3(0f, bob - squash, 0f);
        Quaternion targetRot = baseRot * Quaternion.Euler(pitch, 0f, bank);

        float k = 1f - Mathf.Exp(-poseLerp * Time.deltaTime);
        visualRoot.localPosition = Vector3.Lerp(visualRoot.localPosition, targetPos, k);
        visualRoot.localRotation = Quaternion.Slerp(visualRoot.localRotation, targetRot, k);
    }
}
