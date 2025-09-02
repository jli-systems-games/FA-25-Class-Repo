using UnityEngine;

/// <summary>
/// 在 XZ 平面用刚体移动的球拍控制器。支持自定义键位与边界限制。
/// 挂在 Paddle（带 Rigidbody、BoxCollider 等）上。
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PaddleController3D : MonoBehaviour
{
    [Header("按键映射")]
    public KeyCode keyUp = KeyCode.W;
    public KeyCode keyDown = KeyCode.S;
    public KeyCode keyLeft = KeyCode.A;
    public KeyCode keyRight = KeyCode.D;

    [Header("移动参数")]
    [Tooltip("最大移动速度（单位/秒）")]
    public float maxSpeed = 10f;
    [Tooltip("加速度（越大越“跟手”）")]
    public float acceleration = 40f;
    [Tooltip("松开按键后的减速度")]
    public float deceleration = 50f;

    [Header("移动边界（用 BoxCollider 的包围盒定义）")]
    public BoxCollider movementBounds; // 拖一个空物体加 BoxCollider 做边界框
    [Tooltip("是否将 Y 锁到 0（建议勾上）")]
    public bool lockYZero = true;

    private Rigidbody rb;
    private Vector3 velocityXZ; // 仅存 XZ 分量

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
        if (lockYZero)
        {
            // 避免数值漂移
            var p = rb.position; p.y = 0f; rb.position = p;
        }
    }

    void FixedUpdate()
    {
        // 1) 读输入 → 目标方向（XZ）
        Vector3 input = Vector3.zero;
        if (Input.GetKey(keyLeft)) input.x -= 1f;
        if (Input.GetKey(keyRight)) input.x += 1f;
        if (Input.GetKey(keyUp)) input.z += 1f;
        if (Input.GetKey(keyDown)) input.z -= 1f;
        input = input.sqrMagnitude > 1f ? input.normalized : input;

        // 2) 速度积分（加/减速）
        Vector3 targetVel = input * maxSpeed;
        float a = input == Vector3.zero ? deceleration : acceleration;
        velocityXZ = Vector3.MoveTowards(velocityXZ, targetVel, a * Time.fixedDeltaTime);

        // 3) 计算下一位置（只改 XZ）
        Vector3 next = rb.position + new Vector3(velocityXZ.x, 0f, velocityXZ.z) * Time.fixedDeltaTime;

        // 4) 边界夹紧
        if (movementBounds)
        {
            Bounds b = movementBounds.bounds;
            next.x = Mathf.Clamp(next.x, b.min.x, b.max.x);
            next.z = Mathf.Clamp(next.z, b.min.z, b.max.z);
        }

        if (lockYZero) next.y = 0f;

        rb.MovePosition(next);
    }

    // 方便运行时切换键位（例如左右玩家复用同一脚本）
    public void SetKeys(KeyCode up, KeyCode down, KeyCode left, KeyCode right)
    {
        keyUp = up; keyDown = down; keyLeft = left; keyRight = right;
    }
}