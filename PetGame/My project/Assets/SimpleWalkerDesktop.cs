using UnityEngine;

public class SimpleWalkerDesktop : MonoBehaviour
{
    public PetController pet;
    public float moveSpeed = 2f;
    public float turnSpeed = 360f;
    public bool GoToClosestCorner = false;

    Bounds groundBounds;
    Vector3 target;
    float baseY;

    void Awake() { if (!pet) pet = GetComponent<PetController>(); }

    void Start()
    {
        baseY = transform.position.y;
        groundBounds = ComputeGroundBounds();
        target = transform.position;
    }

    void Update()
    {
        if (pet.petMode == PetMode.Stage) target = EdgePointTowardsMouse();
        else if (GoToClosestCorner) target = CornerPoint(ClosestCornerIndex(transform.position));
        else target = MouseWorldOnGround();

        Vector3 to = target - transform.position; to.y = 0;
        if (to.sqrMagnitude > 0.001f)
        {
            Quaternion q = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turnSpeed * Time.deltaTime);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        transform.position = ClampToBounds(transform.position, groundBounds);
        transform.position = new Vector3(transform.position.x, baseY, transform.position.z);
        pet.SetWorldSpeed(to.magnitude / Time.deltaTime);

        if (GoToClosestCorner && Vector3.Distance(transform.position, target) < 0.3f) GoToClosestCorner = false;
    }

    public void TeleportToCorner(int idx) { transform.position = CornerPoint(idx); }

    Bounds ComputeGroundBounds()
    {
        var cam = Camera.main;
        Vector3[] c = {
            WorldOnPlane(cam, new Vector2(0,0)),
            WorldOnPlane(cam, new Vector2(Screen.width,0)),
            WorldOnPlane(cam, new Vector2(0,Screen.height)),
            WorldOnPlane(cam, new Vector2(Screen.width,Screen.height))
        };
        var b = new Bounds(c[0], Vector3.zero);
        for (int i = 1; i < 4; i++) b.Encapsulate(c[i]);
        return b;
    }

    Vector3 WorldOnPlane(Camera cam, Vector2 screen)
    {
        Ray r = cam.ScreenPointToRay(screen);
        new Plane(Vector3.up, Vector3.zero).Raycast(r, out float d);
        return r.GetPoint(d);
    }

    Vector3 ClampToBounds(Vector3 p, Bounds b)
    {
        return new Vector3(Mathf.Clamp(p.x, b.min.x, b.max.x), p.y, Mathf.Clamp(p.z, b.min.z, b.max.z));
    }

    int ClosestCornerIndex(Vector3 pos)
    {
        int best = 0; float bd = float.MaxValue;
        for (int i = 0; i < 4; i++)
        {
            float d = (CornerPoint(i) - pos).sqrMagnitude;
            if (d < bd) { bd = d; best = i; }
        }
        return best;
    }

    Vector3 CornerPoint(int idx)
    {
        return idx switch
        {
            0 => new Vector3(groundBounds.min.x, baseY, groundBounds.min.z),
            1 => new Vector3(groundBounds.max.x, baseY, groundBounds.min.z),
            2 => new Vector3(groundBounds.min.x, baseY, groundBounds.max.z),
            _ => new Vector3(groundBounds.max.x, baseY, groundBounds.max.z),
        };
    }

    Vector3 EdgePointTowardsMouse()
    {
        var m = MouseWorldOnGround();
        float fx = (m.x > groundBounds.center.x) ? groundBounds.max.x : groundBounds.min.x;
        return new Vector3(fx, baseY, m.z);
    }

    Vector3 MouseWorldOnGround()
    {
        var cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float d);
        return ClampToBounds(ray.GetPoint(d), groundBounds);
    }
}
