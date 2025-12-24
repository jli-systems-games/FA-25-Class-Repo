using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FinalCarDrive : MonoBehaviour
{
    [Header("Speed Settings")]
    public float accelRate = 10f;      // 加速率
    public float brakeRate = 20f;      // 刹车率
    public float maxSpeed = 12f;       // 最大速度

    [Header("Steer Settings")]
    public float turnSpeed = 80f;      // 转向速度（度/秒）

    [Header("Visual Bounce")]
    public Transform carVisual;        // 车的外观（模型，不是刚体）
    public float bounceAmplitude = 0.05f; // 上下抖动幅度
    public float bounceFrequency = 6f;    // 抖动频率（Hz）
                                          // 在 FinalCarDrive 类里新增：
    public bool IsDrive => isDrive;          // 给UI读当前档位
    public float CurrentSpeed => currentSpeed; // 给UI读当前速度（m/s）

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private bool isDrive = true;   // true = D档，false = R档
    private float baseY;           // 视觉原始高度
    private float bounceTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // 防止翻车
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (carVisual != null)
            baseY = carVisual.localPosition.y;
    }

    void Update()
    {
        // ----------------- 切换档位 -----------------
        if (Input.GetKeyDown(KeyCode.T))
        {
            isDrive = !isDrive;
            Debug.Log(isDrive ? "切换到 D 档" : "切换到 R 档");
        }
    }

    void FixedUpdate()
    {
        // ----------------- 踏板输入 -----------------
        if (Input.GetKey(KeyCode.W))
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, accelRate * Time.fixedDeltaTime);
        else if (Input.GetKey(KeyCode.E))
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeRate * Time.fixedDeltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, accelRate * 0.5f * Time.fixedDeltaTime);

        // ----------------- 转向 -----------------
        float turn = 0f;
        if (Input.GetKey(KeyCode.Q)) turn = -1f;
        if (Input.GetKey(KeyCode.R)) turn = 1f;

        if (Mathf.Abs(turn) > 0.1f && currentSpeed > 0.1f)
        {
            float turnAmount = turn * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turnAmount, 0));
        }

        // ----------------- 设置速度 -----------------
        Vector3 forward = transform.forward;
        forward.y = 0f; forward.Normalize();

        // 根据档位决定前进/后退
        Vector3 driveDir = isDrive ? forward : -forward;

        rb.linearVelocity = driveDir * currentSpeed + new Vector3(0, rb.linearVelocity.y, 0);

        // ----------------- 视觉颠簸 -----------------
        if (carVisual != null && currentSpeed > 0.1f)
        {
            bounceTimer += Time.fixedDeltaTime * bounceFrequency;
            float offsetY = Mathf.Sin(bounceTimer) * bounceAmplitude;
            Vector3 localPos = carVisual.localPosition;
            localPos.y = baseY + offsetY;
            carVisual.localPosition = localPos;
        }
    }
}
