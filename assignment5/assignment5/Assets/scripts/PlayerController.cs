using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移动")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 4.5f;
    public float gravity = -9.81f;

    [Header("相机与坐标系")]
    [Tooltip("勾选=按相机方向行走；不勾选=世界方向(W=+Z, A=-X)")]
    public bool moveRelativeToCamera = true;
    public Transform cam; // 留空自动用 Camera.main

    [Header("朝向")]
    [Tooltip("是否自动让角色面向移动方向（相机是玩家子物体时建议关掉）")]
    public bool rotateToFaceMovement = false;
    public float rotateSpeed = 12f;

    [Header("噪音/每秒")]
    public float walkNoisePerSec = 0.08f;
    public float runNoisePerSec = 0.40f;

    CharacterController cc;
    Vector3 velocity;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!cam && Camera.main) cam = Camera.main.transform;
    }

    void Update()
    {
        // 1) 输入
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v);
        input = Vector3.ClampMagnitude(input, 1f);
        bool isMoving = input.sqrMagnitude > 0.0001f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        // 2) 计算移动方向
        Vector3 moveDir;
        if (moveRelativeToCamera && cam != null)
        {
            // 用相机的“水平”前/右
            Vector3 camF = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 camR = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

            // 先缓存相机方向，避免本帧因玩家旋转而反馈
            Vector3 desired = (camF * v + camR * h);
            moveDir = desired.sqrMagnitude > 0.0001f ? desired.normalized : Vector3.zero;
        }
        else
        {
            // 世界方向
            moveDir = new Vector3(h, 0f, v).normalized;
        }

        // 3) （可选）朝向移动方向 —— 相机是玩家子物体时建议关闭
        if (rotateToFaceMovement && isMoving && moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion face = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, face, rotateSpeed * Time.deltaTime);
        }

        // 4) 重力 + 位移
        if (cc.isGrounded && velocity.y < 0f) velocity.y = -0.5f;
        velocity.y += gravity * Time.deltaTime;

        Vector3 motion = moveDir * speed * Time.deltaTime + velocity * Time.deltaTime;
        cc.Move(motion);

        // 5) 连续叠加噪音
        if (NoiseSystem.I && isMoving)
        {
            float add = (isRunning ? runNoisePerSec : walkNoisePerSec) * Time.deltaTime;
            NoiseSystem.I.AddNoise(add);
        }
    }
}
