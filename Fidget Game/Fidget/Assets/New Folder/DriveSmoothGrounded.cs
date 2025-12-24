using UnityEngine;

/// 终版：不会掉到沙面下面的越野车控制（贴地 + 安全夹层 + 多点采样）
/// - 只命中 ground 层
/// - SphereCast + 4邻域多点采样，算平均高度/法线
/// - 安全夹层：如果任何时候低于地面高度 -> 立即夹到 地面+clearance
/// - 速度、朝向、侧倾、下压力、阻尼 都可调
public class DriveSmoothGrounded : MonoBehaviour
{
    [Header("Refs")]
    public Transform carRoot;      // CarRig（含相机/鼓）
    public Camera cam;             // Main Camera
    public Transform steeringWheel;// 可空

    [Header("Movement")]
    public float maxSpeed = 14f;
    public float acceleration = 30f;
    public float decel = 22f;
    public float turnYawSpeed = 150f;

    [Header("Grounding (只勾 ground 层)")]
    public LayerMask groundMask;       // 只选 ground
    public float probeRadius = 0.6f;   // SphereCast 半径
    public float probeHeight = 2.2f;   // 探测起点在车上方
    public float probeDistance = 8f;   // 探测总距离
    public float groundClearance = 0.35f; // 车底离地
    public float groundAlignSpeed = 12f;  // 对齐法线速度
    public float bodyTilt = 7f;           // 横向侧倾
    public float downforce = 60f;         // 下压力（速度越快越压地）
    public float springDamp = 22f;        // 垂直阻尼（抑制上下弹）

    [Header("Safety")]
    public float safetySnap = 0.05f;  // 安全夹层：发现低于地面阈值就立刻抬起
    public float maxSlope = 65f;      // 过陡的面直接忽略（防卡墙/峭壁）

    [Header("Feel")]
    public float fovBase = 60f, fovKick = 1.2f, fovReturn = 3f;

    // 公开速度给其他系统
    public float Speed => vel.magnitude;

    Rigidbody rb;
    Vector3 vel;     // XZ 水平速度
    float yaw;       // 水平朝向
    float lastGroundY; Vector3 lastNormal = Vector3.up;

    void OnValidate()
    {
        // 若有名为 "ground" 的层，自动写入 groundMask
        int g = LayerMask.NameToLayer("ground");
        if (g >= 0 && groundMask == 0) groundMask = 1 << g;
    }

    void Reset() { cam = Camera.main; if (!carRoot && cam) carRoot = cam.transform.root; }

    void Awake()
    {
        if (!carRoot) { Debug.LogError("DriveGroundedFinal: carRoot 未设置"); enabled = false; return; }

        rb = carRoot.GetComponent<Rigidbody>();
        if (!rb) rb = carRoot.gameObject.AddComponent<Rigidbody>();
        rb.mass = 1000f;
        rb.useGravity = false;                       // 手动贴地
        rb.linearDamping = 0f; rb.angularDamping = 0.05f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (cam) cam.fieldOfView = fovBase;
        yaw = carRoot.eulerAngles.y;

        // 自动找方向盘（可选）
        if (!steeringWheel)
        {
            foreach (var t in carRoot.GetComponentsInChildren<Transform>(true))
            {
                string n = t.name.ToLower();
                if (n.Contains("steer") || n.Contains("wheel")) { steeringWheel = t; break; }
            }
        }
    }

    void Update()
    {
        // 输入（长按）
        float up = (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.R) ? -1f : 0f);
        float right = (Input.GetKey(KeyCode.E) ? 1f : 0f) + (Input.GetKey(KeyCode.Q) ? -1f : 0f);

        // 目标速度（以当前车头为参考）
        Vector3 fwd = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
        Vector3 rgt = Quaternion.Euler(0, yaw, 0) * Vector3.right;
        Vector3 desired = (fwd * up + rgt * right).normalized * maxSpeed;

        // 把期望速度投影到地面切平面（减少顺着法线“浮”）
        desired = Vector3.ProjectOnPlane(desired, lastNormal).normalized * desired.magnitude;

        // 平滑加减速
        float a = (desired.sqrMagnitude > 0.01f) ? acceleration : decel;
        vel += Vector3.ClampMagnitude(desired - vel, a * Time.deltaTime);

        // 车头朝向跟速度方向
        if (vel.sqrMagnitude > 0.01f)
        {
            float ty = Mathf.Atan2(vel.x, vel.z) * Mathf.Rad2Deg;
            yaw = Mathf.MoveTowardsAngle(yaw, ty, turnYawSpeed * Time.deltaTime);
        }

        // 方向盘微转/相机 FOV
        if (steeringWheel)
        {
            float steerAmt = Mathf.Clamp(right - up * 0.25f, -1, 1);
            steeringWheel.localRotation = Quaternion.Euler(0, 0, -steerAmt * 60f);
        }
        if (cam)
        {
            float tf = fovBase + fovKick * (vel.magnitude / maxSpeed);
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, tf, fovReturn * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // 1) 多点采样（中心 + 四方向），求平均高度与法线
        bool hitAny = false;
        Vector3 nSum = Vector3.zero;
        float hSum = 0f; int cnt = 0;

        Vector3[] offs = {
            Vector3.zero,
            carRoot.right * 0.8f, -carRoot.right * 0.8f,
            carRoot.forward * 0.8f, -carRoot.forward * 0.8f
        };

        for (int i = 0; i < offs.Length; i++)
        {
            Vector3 o = carRoot.position + offs[i] + Vector3.up * probeHeight;
            if (Physics.SphereCast(o, probeRadius, Vector3.down, out var hit, probeDistance, groundMask, QueryTriggerInteraction.Ignore))
            {
                // 忽略过陡的面（峭壁/石柱侧面）
                if (Vector3.Angle(hit.normal, Vector3.up) > maxSlope) continue;

                hitAny = true;
                nSum += hit.normal;
                hSum += hit.point.y;
                cnt++;
            }
        }

        // 没打到地：仅平面移动，保持朝向
        if (!hitAny || cnt == 0)
        {
            Vector3 flat = new Vector3(vel.x, 0, vel.z) * Time.fixedDeltaTime;
            rb.MovePosition(carRoot.position + flat);
            rb.MoveRotation(Quaternion.Euler(0, yaw, 0));
            return;
        }

        Vector3 avgNormal = (nSum / cnt).normalized;
        float avgHeight = (hSum / cnt) / 1f;

        // 2) 目标位置：水平推进 + 抬到平均高度 + 离地 + 下压力 + 垂直阻尼
        Vector3 target = carRoot.position + new Vector3(vel.x, 0, vel.z) * Time.fixedDeltaTime;

        float goalY = avgHeight + groundClearance;
        float vy = (goalY - carRoot.position.y) * springDamp * Time.fixedDeltaTime; // 垂直阻尼
        target.y = carRoot.position.y + vy;

        target += Vector3.down * (downforce * (vel.magnitude / maxSpeed) * Time.fixedDeltaTime);

        // —— 安全夹层：若当前高度已经低于（地面+clearance - safetySnap），立即夹上来
        float groundYNow = avgHeight + groundClearance - safetySnap;
        if (target.y < groundYNow) target.y = groundYNow;

        rb.MovePosition(target);

        // 3) 目标旋转：对齐平均法线 + 侧倾 + 朝向
        Quaternion groundRot = Quaternion.FromToRotation(carRoot.up, avgNormal) * carRoot.rotation;
        Quaternion yawRot = Quaternion.Euler(0, yaw, 0);
        float roll = Mathf.Clamp(Vector3.Dot(vel.normalized, carRoot.right), -1f, 1f) * bodyTilt;
        Quaternion tilt = Quaternion.Euler(0, 0, -roll);
        Quaternion want = Quaternion.Slerp(groundRot, yawRot * tilt, 0.55f);
        rb.MoveRotation(Quaternion.Slerp(carRoot.rotation, want, groundAlignSpeed * Time.fixedDeltaTime));

        // 4) 记录法线供下一帧切线投影
        lastNormal = avgNormal;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!carRoot) return;
        Gizmos.color = new Color(0, 1, 1, 0.6f);
        Vector3[] offs = {
            Vector3.zero,
            carRoot.right * 0.8f, -carRoot.right * 0.8f,
            carRoot.forward * 0.8f, -carRoot.forward * 0.8f
        };
        foreach (var off in offs)
        {
            Vector3 o = carRoot.position + off + Vector3.up * probeHeight;
            Gizmos.DrawWireSphere(o, probeRadius);
            Gizmos.DrawLine(o, o + Vector3.down * probeDistance);
        }
    }
#endif
}
