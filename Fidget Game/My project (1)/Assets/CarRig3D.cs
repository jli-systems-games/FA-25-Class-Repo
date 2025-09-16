using UnityEngine;

public class CarRig3D : MonoBehaviour
{
    [Header("Refs")]
    public Transform carRig;        // 拖 CarRig
    public Camera cam;              // 拖 Main Camera
    public Transform steeringWheel; // UI 可空

    [Header("Motion")]
    public float forwardSpeed = 20f;   // 前进速度（m/s）
    public float steerStep = 0.35f;    // 每次按键的转向“脉冲”
    public float steerDecay = 2.5f;    // 自动回正
    public float maxSteer = 0.9f;      // 转向限幅（-1~1）
    public float yawAngleMax = 12f;    // 车身（相机）左右偏航角度
    public float laneAmplitude = 2.2f; // 视觉上的左右位移幅度（车道宽度感）

    [Header("Shake")]
    public float engineBobAmp = 0.02f; // 轻微颠簸
    public float engineBobFreq = 8f;

    float steer;   // -1~1
    float t;

    void Reset()
    {
        if (!carRig) carRig = GameObject.Find("CarRig").transform;
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        // 车向前
        carRig.position += Vector3.forward * (forwardSpeed * Time.deltaTime);

        // 回正
        steer = Mathf.MoveTowards(steer, 0f, steerDecay * Time.deltaTime);

        // 视觉：车身左右位移 + 偏航
        float xOffset = laneAmplitude * steer;
        float yaw = steer * yawAngleMax;

        // 相机跟随到 CarRig 局部偏移（只改局部 X/Y，不改 Z）
        Vector3 camLocal = cam.transform.localPosition;
        camLocal.x = Mathf.Lerp(camLocal.x, xOffset, 8f * Time.deltaTime);

        // 轻微颠簸
        t += Time.deltaTime;
        camLocal.y = 1.2f + Mathf.Sin(t * engineBobFreq) * engineBobAmp;

        cam.transform.localPosition = camLocal;
        cam.transform.localRotation = Quaternion.Lerp(
            cam.transform.localRotation,
            Quaternion.Euler(0f, yaw, 0f),
            8f * Time.deltaTime
        );

        // 方向盘 UI
        if (steeringWheel)
        {
            float z = -steer * 22f; // 轮盘最大转角 UI
            steeringWheel.localRotation = Quaternion.Euler(0, 0, z);
        }
    }

    // 鼓键调用
    public void PulseLeft(float strength = 1f) { steer = Mathf.Clamp(steer - steerStep * strength, -maxSteer, maxSteer); }
    public void PulseRight(float strength = 1f) { steer = Mathf.Clamp(steer + steerStep * strength, -maxSteer, maxSteer); }
    public void PulseCenter(float strength = 1f) { steer = Mathf.Lerp(steer, 0f, 0.25f * strength); }
    public void PulseShake(float strength = 1f) { /* 可在这里做瞬时摄像机抖动，简单版略 */ }
}
