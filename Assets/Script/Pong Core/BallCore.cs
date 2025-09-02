using UnityEngine;

/// <summary>
/// 3D Pong 球：恒定速度，碰撞后仅改变方向；与球拍碰撞可根据击中位置“切角”。
/// 场地在 XZ 平面（Y=0）。
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BallPong3D : MonoBehaviour
{
    [Header("运动")]
    public float speed = 12f;                // 恒定移速
    public float speedKickMul = 1f;          // 需要“逐渐加速”可设 1.03f 之类，否则保持 1
    public float maxSpeed = 20f;
    public float minAngleFromX = 7f;         // 防止几乎水平贴边

    [Header("切球")]
    public string paddleTag = "Paddle";
    public float angleFactor = 6f;           // 击中偏移 -> Z 分量放大倍数

    Rigidbody rb;
    Vector3 lastDir = Vector3.right;         // 记录上一帧方向，防止归零
    Vector3 preVel;                          // 碰撞前速度（用于反射）

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
        // 给球和墙体都配一个 PhysicMaterial：bounciness=1, friction=0, BounceCombine=Maximum
    }

    void Start()
    {
        Serve(Random.value < 0.5f ? -1 : 1);
    }

    public void Serve(int dirX)
    {
        lastDir = new Vector3(Mathf.Sign(dirX), 0f, Random.Range(-0.5f, 0.5f)).normalized;
        rb.linearVelocity = lastDir * speed;
    }

    void FixedUpdate()
    {
        // 记录碰撞前速度
        preVel = rb.linearVelocity;

        // 强制恒速并锁 Y
        Vector3 v = rb.linearVelocity;
        if (v.sqrMagnitude < 1e-4f) v = (lastDir.sqrMagnitude > 0.01f ? lastDir : Vector3.right);
        v.y = 0f;

        // 夹最小角（避免纯水平）
        v = EnforceMinAngleXZ(v);

        // 恒速
        float cur = v.magnitude;
        if (cur > 1e-4f)
        {
            v = v / cur * Mathf.Min(speed, maxSpeed);
            rb.linearVelocity = v;
            lastDir = v.normalized;
        }
    }

    void OnCollisionEnter(Collision c)
    {
        // 基础：用法线做反射，保持模长
        Vector3 normal = c.contacts[0].normal;
        Vector3 incoming = preVel.sqrMagnitude > 1e-6f ? preVel : lastDir * speed;
        Vector3 v = Vector3.Reflect(incoming, normal);

        // 与球拍：按击中偏移“切角”，只改 XZ 并保持朝向 x 不反转过度
        if (c.collider.CompareTag(paddleTag))
        {
            // 击中点相对球拍中心的 Z 偏移决定出球角度
            float zOffset = transform.position.z - c.collider.bounds.center.z;
            v = new Vector3(Mathf.Sign(v.x), 0f, zOffset * angleFactor);
        }

        // 清理 Y，夹最小角，并保持恒速（可选加速）
        v.y = 0f;
        v = EnforceMinAngleXZ(v).normalized * Mathf.Clamp(speed * speedKickMul, 0f, maxSpeed);
        speed = v.magnitude; // 若使用逐渐加速，更新恒速
        rb.linearVelocity = v;
        lastDir = v.normalized;
    }

    Vector3 EnforceMinAngleXZ(Vector3 v)
    {
        // 确保相对 X 轴的夹角 >= minAngleFromX（防止“贴边直线”）
        Vector2 xz = new Vector2(v.x, v.z);
        if (xz.sqrMagnitude < 1e-6f) return new Vector3(Mathf.Sign(lastDir.x), 0f, 0.1f);
        float ang = Vector2.Angle(new Vector2(Mathf.Sign(xz.x), 0f), xz);
        if (ang < minAngleFromX)
        {
            float signZ = Mathf.Sign(xz.y == 0 ? Random.Range(-1f, 1f) : xz.y);
            float rad = minAngleFromX * Mathf.Deg2Rad;
            xz = new Vector2(Mathf.Sign(xz.x) * Mathf.Cos(rad), signZ * Mathf.Sin(rad));
            return new Vector3(xz.x, 0f, xz.y) * v.magnitude;
        }
        return v;
    }
}