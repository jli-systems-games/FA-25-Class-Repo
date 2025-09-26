using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PigRam : MonoBehaviour
{
    public Transform snoutPoint;
    public string targetTag = "Knockable";
    public LayerMask allowedLayers = ~0;

    public KeyCode key = KeyCode.Mouse0;
    public float cooldown = 0.06f;

    public bool autoWhileShift = true;
    public float autoInterval = 0.12f;
    public float runSpeedThreshold = 3.0f;

    public float range = 1.0f;
    public float sphereRadius = 0.45f;
    public float impulse = 30f;
    public float upFactor = 0.25f;
    public bool useVelocityChange = false;

    public float defaultMass = 10f;
    public float defaultDrag = 0.1f;
    public float defaultAngularDrag = 0.05f;

    public Camera cam;
    public float fovNormal = 70f;
    public float fovPunchAdd = 10f;
    public float fovPunchTime = 0.08f;
    public float fovRecoverTime = 0.2f;

    public float selfLockTime = 0.12f;

    public AudioSource audioSource;
    public AudioClip ramClip;
    public float ramVol = 1f;
    public Vector2 ramPitchRange = new Vector2(0.95f, 1.05f);

    Rigidbody rb;
    RigidbodyConstraints originalConstraints;
    float nextTime;
    float autoNextTime;
    float fovT;
    float fovPhase;
    float fovFrom;
    float fovTo;
    float lockUntil;
    Vector3 lockPosXZ;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalConstraints = rb.constraints;
        if (!cam) cam = Camera.main;
        if (cam) cam.fieldOfView = fovNormal;
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(key) && Time.time >= nextTime)
        {
            if (TryRam()) StartSelfLock();
            nextTime = Time.time + Mathf.Max(0f, cooldown);
        }

        if (autoWhileShift && Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 hv = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (hv.magnitude >= runSpeedThreshold && Time.time >= autoNextTime)
            {
                if (TryRam()) StartSelfLock();
                autoNextTime = Time.time + Mathf.Max(autoInterval, cooldown);
            }
        }
        else
        {
            autoNextTime = 0f;
        }

        UpdateFOV(Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (Time.time < lockUntil)
        {
            Vector3 p = rb.position;
            p.x = lockPosXZ.x;
            p.z = lockPosXZ.z;
            rb.MovePosition(p);
            Vector3 v = rb.linearVelocity;
            v.x = 0f;
            v.z = 0f;
            rb.linearVelocity = v;
            rb.AddForce(Vector3.down * 50f, ForceMode.Acceleration);
        }
        else
        {
            if (rb.constraints != originalConstraints) rb.constraints = originalConstraints;
        }
    }

    bool TryRam()
    {
        Vector3 src = snoutPoint ? snoutPoint.position : transform.position + transform.forward * 0.5f;
        Vector3 dir = transform.forward;

        RaycastHit bestHit;
        bool has = SphereCastBest(src, dir, out bestHit);
        if (!has) has = Physics.Raycast(src, dir, out bestHit, range, allowedLayers, QueryTriggerInteraction.Ignore);
        if (!has) return false;

        Transform tagRoot = FindTagged(bestHit.collider.transform);
        if (!tagRoot) return false;

        Rigidbody other = tagRoot.GetComponent<Rigidbody>();
        if (!other) other = EnsureRigid(tagRoot.gameObject);
        if (!other || other.isKinematic) return false;

        Vector3 toCOM = other.worldCenterOfMass - src;
        Vector3 aim = Vector3.Lerp(toCOM.normalized, transform.forward, 0.4f).normalized;
        aim.y = Mathf.Max(aim.y, upFactor);

        Vector3 hitPoint = bestHit.point != Vector3.zero ? bestHit.point : other.worldCenterOfMass;
        if (useVelocityChange) other.AddForceAtPosition(aim * impulse, hitPoint, ForceMode.VelocityChange);
        else other.AddForceAtPosition(aim * impulse, hitPoint, ForceMode.Impulse);

        StartFOVPunch();
        PlayRamSfx();
        return true;
    }

    void PlayRamSfx()
    {
        if (!audioSource || !ramClip) return;
        float p = Random.Range(ramPitchRange.x, ramPitchRange.y);
        audioSource.pitch = p;
        audioSource.PlayOneShot(ramClip, ramVol);
    }

    void StartSelfLock()
    {
        lockPosXZ = new Vector3(rb.position.x, 0f, rb.position.z);
        lockUntil = Time.time + selfLockTime;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    bool SphereCastBest(Vector3 src, Vector3 dir, out RaycastHit best)
    {
        var hits = Physics.SphereCastAll(src, sphereRadius, dir, range, allowedLayers, QueryTriggerInteraction.Ignore);
        float bestDist = float.MaxValue;
        bool found = false;
        best = new RaycastHit();
        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (!h.collider) continue;
            if (!FindTagged(h.collider.transform)) continue;
            float d = h.distance;
            if (d < bestDist)
            {
                bestDist = d;
                best = h;
                found = true;
            }
        }
        return found;
    }

    Transform FindTagged(Transform t)
    {
        while (t)
        {
            if (t.CompareTag(targetTag)) return t;
            t = t.parent;
        }
        return null;
    }

    Rigidbody EnsureRigid(GameObject go)
    {
        var rb2 = go.GetComponent<Rigidbody>();
        if (!rb2) rb2 = go.AddComponent<Rigidbody>();
        rb2.isKinematic = false;
        rb2.useGravity = true;
        rb2.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb2.mass = defaultMass;
        rb2.linearDamping = defaultDrag;
        rb2.angularDamping = defaultAngularDrag;

        var mcs = go.GetComponentsInChildren<MeshCollider>(true);
        for (int i = 0; i < mcs.Length; i++) mcs[i].convex = true;
        if (!go.GetComponent<Collider>())
        {
            var r = go.GetComponentInChildren<Renderer>();
            if (r)
            {
                var bc = go.AddComponent<BoxCollider>();
                bc.center = go.transform.InverseTransformPoint(r.bounds.center);
                bc.size = go.transform.InverseTransformVector(r.bounds.size);
            }
        }
        return rb2;
    }

    void StartFOVPunch()
    {
        if (!cam) return;
        fovPhase = 0f;
        fovT = 0f;
        fovFrom = cam.fieldOfView;
        fovTo = fovNormal + fovPunchAdd;
    }

    void UpdateFOV(float dt)
    {
        if (!cam) return;
        if (fovPhase == 0f)
        {
            fovT += dt / Mathf.Max(0.0001f, fovPunchTime);
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(fovT));
            cam.fieldOfView = Mathf.Lerp(fovFrom, fovTo, k);
            if (fovT >= 1f) { fovPhase = 1f; fovT = 0f; fovFrom = cam.fieldOfView; fovTo = fovNormal; }
        }
        else
        {
            fovT += dt / Mathf.Max(0.0001f, fovRecoverTime);
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(fovT));
            cam.fieldOfView = Mathf.Lerp(fovFrom, fovTo, k);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 src = snoutPoint ? snoutPoint.position : transform.position + transform.forward * 0.5f;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(src, sphereRadius);
        Gizmos.DrawLine(src, src + transform.forward * range);
        Gizmos.DrawWireSphere(src + transform.forward * range, sphereRadius);
    }
}
