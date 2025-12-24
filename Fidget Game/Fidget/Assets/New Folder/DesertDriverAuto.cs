using UnityEngine;
using System.Linq;

public class DesertDriverAuto : MonoBehaviour
{
    [Header("Auto refs (可留空)")]
    public Camera cam;              // 为空用 Camera.main
    public Transform carRoot;       // 为空用 cam 的最顶层 root
    public Transform steeringWheel; // 为空自动从 carRoot 下按名字匹配(*steer*|*wheel*)

    [Header("前进")]
    public float speed = 22f;       // m/s
    public bool moveWorldInstead = false; // 勾上=世界往后移
    public Transform worldRoot;     // 勾上时把外景根节点拖进来(可空则不动)

    [Header("转向手感")]
    [Range(-1, 1)] public float steer;    // -1..1
    public float pulse = 0.45f;          // 每次Q/E脉冲
    public float returnRate = 2.8f;      // 回正速度
    public float maxSteer = 0.9f;

    [Header("相机手感")]
    public float laneWidth = 2.6f;       // 左右位移幅度
    public float laneSmooth = 0.08f;     // SmoothDamp 时间
    public float yawMax = 11f;           // 左右偏航角
    public float yawSmooth = 0.08f;
    public float rollMax = 4.5f;         // 侧倾
    public float fovBase = 60f;
    public float fovKick = 2f;
    public float fovReturn = 4f;

    [Header("沙漠颠簸(Perlin)")]
    public float bumpAmpLow = 0.08f;     // 低频起伏
    public float bumpFreqLow = 0.35f;
    public float bumpAmpHi = 0.015f;     // 高频细抖
    public float bumpFreqHi = 8f;
    public float bumpSpeedInfluence = 0.25f;

    float steerTarget, xVel, yawVel, t, baseY;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!cam) { Debug.LogError("DesertDriverAuto: 找不到相机，请把主相机拖到 cam 字段"); enabled = false; return; }
        if (!carRoot) carRoot = cam.transform.root;

        if (!steeringWheel)
        {
            // 在车体上递归找名字里包含 steer/wheel 的物体
            steeringWheel = carRoot.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(tr => {
                    string n = tr.name.ToLower();
                    return n.Contains("steer") || n.Contains("wheel");
                });
        }
    }

    void Start()
    {
        baseY = cam.transform.localPosition.y;
        cam.fieldOfView = fovBase;
    }

    void Update()
    {
        float dt = Time.deltaTime; t += dt;

        // —— 前进：两种方式二选一 ——
        if (!moveWorldInstead)
        {
            carRoot.position += Vector3.forward * (speed * dt);
        }
        else if (worldRoot)
        {
            worldRoot.position -= Vector3.forward * (speed * dt);
        }

        // —— 回正/限幅 ——
        steerTarget = Mathf.MoveTowards(steerTarget, 0f, returnRate * dt);
        steer = Mathf.Clamp(steerTarget, -maxSteer, maxSteer);

        // —— 相机左右平滑 + 颠簸 —— 
        var lp = cam.transform.localPosition;
        float targetX = laneWidth * steer;
        lp.x = Mathf.SmoothDamp(lp.x, targetX, ref xVel, laneSmooth);

        float speedFactor = 1f + speed * 0.01f * bumpSpeedInfluence;
        float low = (Mathf.PerlinNoise(0f, t * bumpFreqLow) - .5f) * 2f * bumpAmpLow * speedFactor;
        float hi = (Mathf.PerlinNoise(10f, t * bumpFreqHi) - .5f) * 2f * bumpAmpHi * speedFactor;
        lp.y = baseY + low + hi;
        cam.transform.localPosition = lp;

        float targetYaw = steer * yawMax;
        float yaw = Mathf.SmoothDampAngle(cam.transform.localEulerAngles.y, targetYaw, ref yawVel, yawSmooth);
        float roll = -steer * rollMax;
        cam.transform.localRotation = Quaternion.Euler(0f, yaw, roll);

        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, fovBase, fovReturn * dt);

        // —— 键盘 —— 
        if (Input.GetKeyDown(KeyCode.Q)) TapLeft();
        if (Input.GetKeyDown(KeyCode.W)) TapCenter();
        if (Input.GetKeyDown(KeyCode.E)) TapRight();
        if (Input.GetKeyDown(KeyCode.R)) TapFX();
    }

    // 提供给鼓脚本/键盘的入口
    public void TapLeft(float s = 1f) { steerTarget = Mathf.Clamp(steerTarget - pulse * s, -maxSteer, maxSteer); KickWheel(); KickFOV(); }
    public void TapRight(float s = 1f) { steerTarget = Mathf.Clamp(steerTarget + pulse * s, -maxSteer, maxSteer); KickWheel(); KickFOV(); }
    public void TapCenter(float s = 1f) { steerTarget = Mathf.Lerp(steerTarget, 0f, 0.35f * s); KickWheel(); KickFOV(); }
    public void TapFX(float s = 1f) { KickFOV(0.6f); }

    void KickWheel()
    {
        if (!steeringWheel) return;
        // 大多数方向盘模型转Z轴比较自然；不对就把 Z 改成 Y 试试
        steeringWheel.localRotation = Quaternion.Euler(0, 0, -steerTarget * 60f);
    }

    void KickFOV(float scale = 1f)
    {
        cam.fieldOfView = Mathf.Min(cam.fieldOfView + fovKick * scale, fovBase + fovKick * 1.5f);
    }
}
