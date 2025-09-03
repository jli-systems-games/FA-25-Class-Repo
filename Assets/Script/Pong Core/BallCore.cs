using UnityEngine;

/// <summary>
/// 3D Pong 球：恒定速度；墙体按物理反射；
/// 撞球拍时：依据击中相对偏移与拍子自身速度，计算目标出球角，
/// 再与物理反射方向做插值，获得更自然但可控的“擦球”手感。
/// 运动平面：XZ（Y=0）。
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BallPong3D : MonoBehaviour
{
    [Header("运动")]
    [Tooltip("恒定移动速度")]
    public float speed = 12f;
    [Tooltip("每次撞击后的速度倍率（1 = 不加速）")]
    public float speedKickMul = 1f;
    [Tooltip("速度上限")]
    public float maxSpeed = 20f;
    [Tooltip("相对 X 轴的最小夹角，避免几乎水平贴边（单位：度）")]
    public float minAngleFromX = 7f;

    [Header("识别/碰撞")]
    [Tooltip("球拍的 Tag（用于识别是否撞到了球拍）")]
    public string paddleTag = "Paddle";

    [Header("控球手感（击中球拍时生效）")]
    [Range(0f, 80f)]
    [Tooltip("最大可偏转角，越大越容易打出“大斜线”（度）")]
    public float maxDeflectAngle = 50f;
    [Range(0f, 1f)]
    [Tooltip("0=完全物理反射，1=完全按控球角；推荐 0.6~0.7")]
    public float controlBlend = 0.65f;
    [Tooltip("拍子自身 Z 速度对出球的影响系数（相对球速），0.08~0.15 比较有手感")]
    public float paddleInfluence = 0.10f;
    [Tooltip("击中拍子中心附近的死区（相对半幅 0..1），避免中心也出过大角")]
    public float hitDeadZone = 0.06f;
    [Tooltip("偏移曲线指数，>1 中间更平缓、边缘更激进")]
    public float offsetCurveExp = 1.2f;

    Rigidbody rb;
    Vector3 lastDir = Vector3.right;   // 记录上一帧方向，防止归零
    Vector3 preVel;                    // 碰撞前速度（用于反射）

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
        // 提示：给球 & 墙体设置 PhysicMaterial：Bounciness=1, Friction=0, BounceCombine=Maximum
    }

    void Start()
    {
        Serve(Random.value < 0.5f ? -1 : 1);
    }

    /// <summary>从左/右发球（dirX = -1 或 1）</summary>
    public void Serve(int dirX)
    {
        lastDir = new Vector3(Mathf.Sign(dirX), 0f, Random.Range(-0.5f, 0.5f)).normalized;
        rb.linearVelocity = lastDir * speed;
    }

    void FixedUpdate()
    {
        // 记录碰撞前速度
        preVel = rb.linearVelocity;

        // 强制恒速 & 锁 Y
        Vector3 v = rb.linearVelocity;
        if (v.sqrMagnitude < 1e-4f)
            v = (lastDir.sqrMagnitude > 0.01f ? lastDir : Vector3.right);

        v.y = 0f;
        v = EnforceMinAngleXZ(v);

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
        // ——基础反射——
        Vector3 normal = c.contacts[0].normal;
        Vector3 incoming = preVel.sqrMagnitude > 1e-6f ? preVel : lastDir * speed;
        Vector3 v = Vector3.Reflect(incoming, normal); // 不改模长（下方再统一恒速）

        // ——撞到球拍：基于“相对偏移 + 拍子自身速度”控制出球角——
        if (c.collider.CompareTag(paddleTag))
        {
            // 1) 当前反射的单位方向
            Vector3 reflectDir = v.normalized;

            // 2) 计算相对偏移（-1..1），按球拍尺寸归一化
            Bounds pb = c.collider.bounds;
            float halfZ = Mathf.Max(0.0001f, pb.extents.z);
            float offsetZ = (transform.position.z - pb.center.z) / halfZ; // -1..1

            // 死区与曲线 shaping
            float sign = Mathf.Sign(offsetZ);
            float abs = Mathf.Max(0f, Mathf.Abs(offsetZ) - hitDeadZone) / Mathf.Max(1e-6f, (1f - hitDeadZone));
            float shaped = Mathf.Pow(abs, Mathf.Max(1f, offsetCurveExp)) * sign;

            // 拍子自身速度带动（Z 向），按球速归一化
            float paddleVelZ = 0f;
            var paddleRB = c.rigidbody; // 被撞的刚体（拍子）
            if (paddleRB) paddleVelZ = paddleRB.linearVelocity.z;
            shaped += (paddleVelZ * paddleInfluence) / Mathf.Max(1f, speed);
            shaped = Mathf.Clamp(shaped, -1f, 1f);

            // 3) 目标角（相对 X 轴）
            float theta = shaped * Mathf.Clamp(maxDeflectAngle, 0f, 89.9f) * Mathf.Deg2Rad;

            // 保持朝向对侧（由反射后的 X 号确定）
            float dirX = Mathf.Sign(reflectDir.x == 0f ? lastDir.x : reflectDir.x);
            Vector3 aimedDir = new Vector3(Mathf.Cos(theta) * dirX, 0f, Mathf.Sin(theta));

            // 4) 物理反射 与 目标角 插值（Slerp 更顺）
            Vector3 blended = Vector3.Slerp(reflectDir, aimedDir, Mathf.Clamp01(controlBlend)).normalized;

            // 5) 最小角保护 + 恒速
            blended = EnforceMinAngleXZ(blended);
            v = blended * Mathf.Clamp(speed * speedKickMul, 0f, maxSpeed);
        }

        // ——统一清理 & 赋回——
        v.y = 0f;
        v = EnforceMinAngleXZ(v).normalized * Mathf.Clamp(speed * speedKickMul, 0f, maxSpeed);
        speed = v.magnitude;         // 若逐步加速，更新恒速
        rb.linearVelocity = v;
        lastDir = v.normalized;

        // ——触发染色（你已有的 PaddlePaint 脚本）——
        var paddle = c.collider.GetComponent<PaddlePaint>();
        if (paddle != null)
        {
            paddle.PaintBall(gameObject); // 会调用 BallTeamSkin / BallTrailPainter3D 设置阵营与外观
        }
    }

    /// <summary>确保相对 X 轴的最小夹角，避免几乎水平贴边</summary>
    Vector3 EnforceMinAngleXZ(Vector3 v)
    {
        Vector2 xz = new Vector2(v.x, v.z);
        if (xz.sqrMagnitude < 1e-6f) return new Vector3(Mathf.Sign(lastDir.x), 0f, 0.1f);

        float ang = Vector2.Angle(new Vector2(Mathf.Sign(xz.x), 0f), xz); // 与 ±X 的夹角
        if (ang < minAngleFromX)
        {
            float signZ = Mathf.Sign(Mathf.Approximately(xz.y, 0f) ? Random.Range(-1f, 1f) : xz.y);
            float rad = Mathf.Clamp(minAngleFromX, 0f, 89.9f) * Mathf.Deg2Rad;
            xz = new Vector2(Mathf.Sign(xz.x) * Mathf.Cos(rad), signZ * Mathf.Sin(rad));
            return new Vector3(xz.x, 0f, xz.y) * v.magnitude;
        }
        return v;
    }
}