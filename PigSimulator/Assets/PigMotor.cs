using UnityEngine;

/// 第三人称移动：走/跑/跳/蹲（无需骨骼动画）
/// 使用旧输入系统（WASD/Shift/Space/Ctrl）。Player > Active Input Handling 设为 Both。
[RequireComponent(typeof(CharacterController))]
public class PigMotor : MonoBehaviour
{
    [Header("References")]
    [Tooltip("主相机或跟随猪的 Cinemachine 相机的 Transform")]
    public Transform cameraTransform;
    [Tooltip("猪模型的根节点，用于朝向旋转和假动画")]
    public Transform modelRoot;

    [Header("Move")]
    public float moveSpeed = 3.0f;      // 走路速度
    public float sprintMult = 1.6f;     // 冲刺倍率（按住Shift）
    public float crouchMult = 0.55f;    // 蹲下减速倍率
    public float rotationSpeed = 12f;   // 模型朝向旋转速度

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.1f;     // Space 跳跃高度（米）
    public float gravity = -18f;        // 重力（更负更重）
    public float groundedStick = -2f;   // 贴地小负值，防抖

    [Header("Crouch (CharacterController)")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public bool toggleCrouch = true;    // true=切换式，false=按住
    [Range(0.4f, 0.9f)]
    public float crouchHeightMult = 0.6f; // 碰撞体高度倍率(蹲下)
    public float crouchCamDrop = 0.25f;   // 蹲下相机下降（相机是子物体时生效）

    [Header("Fake Anim (可关)")]
    public bool fakeAnim = true;
    public float leanAmount = 8f;       // 左右倾斜角度（度）
    public float bobAmount = 0.04f;     // 行走上下起伏
    public float bobSpeed = 11f;        // 起伏频率

    CharacterController cc;
    Vector3 velocity;                   // 只用 y 分量作为竖直速度
    float baseHeight, baseCenterY;

    bool isCrouching = false;
    float bobT = 0f;
    float camBaseLocalY = 0f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        baseHeight = cc.height;
        baseCenterY = cc.center.y;

        if (cameraTransform && cameraTransform.parent)
            camBaseLocalY = cameraTransform.localPosition.y;
    }

    void Update()
    {
        // 1) 输入
        float h = Input.GetAxisRaw("Horizontal"); // A/D 或 ←/→
        float v = Input.GetAxisRaw("Vertical");   // W/S 或 ↑/↓
        Vector2 raw = new Vector2(h, v);
        if (raw.sqrMagnitude > 1f) raw.Normalize();

        bool wantSprint = Input.GetKey(KeyCode.LeftShift);
        bool wantJump = Input.GetKeyDown(KeyCode.Space);

        // 蹲下
        if (toggleCrouch)
        {
            if (Input.GetKeyDown(crouchKey)) isCrouching = !isCrouching;
        }
        else isCrouching = Input.GetKey(crouchKey);

        // 2) 由相机方向决定移动方向
        Vector3 camFwd = cameraTransform
            ? Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized
            : Vector3.forward;
        Vector3 camRight = Vector3.Cross(Vector3.up, camFwd);
        Vector3 moveDir = (camFwd * raw.y + camRight * raw.x);
        if (moveDir.sqrMagnitude > 1e-4f) moveDir.Normalize();

        // 速度倍率
        float mult = 1f;
        if (wantSprint && !isCrouching) mult *= sprintMult;
        if (isCrouching) mult *= crouchMult;
        Vector3 move = moveDir * moveSpeed * mult;

        // 3) 跳跃/重力
        bool grounded = cc.isGrounded;
        if (grounded && velocity.y < 0f) velocity.y = groundedStick;

        if (grounded && wantJump && !isCrouching)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;

        // 4) 应用移动
        Vector3 displacement = move * Time.deltaTime + velocity * Time.deltaTime;
        cc.Move(displacement);

        // 5) 模型朝向
        if (modelRoot && moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            modelRoot.rotation = Quaternion.Slerp(modelRoot.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // 6) 蹲下：碰撞体高度/中心 & 相机下降
        float targetHeight = isCrouching ? baseHeight * crouchHeightMult : baseHeight;
        cc.height = Mathf.Lerp(cc.height, targetHeight, 12f * Time.deltaTime);
        cc.center = new Vector3(
            cc.center.x,
            Mathf.Lerp(cc.center.y, isCrouching ? baseCenterY * crouchHeightMult : baseCenterY, 12f * Time.deltaTime),
            cc.center.z
        );

        if (cameraTransform && cameraTransform.parent) // 相机是子物体时才移动
        {
            float tgtY = isCrouching ? camBaseLocalY - crouchCamDrop : camBaseLocalY;
            Vector3 lp = cameraTransform.localPosition;
            lp.y = Mathf.Lerp(lp.y, tgtY, 10f * Time.deltaTime);
            cameraTransform.localPosition = lp;
        }

        // 7) 假动画：左右倾斜 + 行走起伏
        if (fakeAnim && modelRoot)
        {
            // 倾斜
            float lean = -leanAmount * Mathf.Clamp(h, -1f, 1f);
            Quaternion extra = Quaternion.Euler(0, 0, lean);
            modelRoot.localRotation = Quaternion.Slerp(modelRoot.localRotation, extra, 10f * Time.deltaTime);

            // 起伏
            float speed01 = Mathf.Clamp01(move.magnitude / (moveSpeed * sprintMult));
            bobT += Time.deltaTime * bobSpeed * speed01;
            float bob = Mathf.Sin(bobT) * bobAmount * speed01;
            Vector3 p = modelRoot.localPosition; p.y = Mathf.Lerp(p.y, bob, 12f * Time.deltaTime);
            modelRoot.localPosition = p;
        }
    }

    void OnDrawGizmosSelected()
    {
        // 显示控制器体积，便于调试高度/中心
        var c = GetComponent<CharacterController>();
        if (!c) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * (c.center.y - c.height * 0.5f + c.radius), c.radius);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * (c.center.y + c.height * 0.5f - c.radius), c.radius);
    }
}
