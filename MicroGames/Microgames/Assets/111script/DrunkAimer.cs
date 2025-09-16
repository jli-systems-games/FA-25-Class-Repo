using UnityEngine;

public class DrunkAim : MonoBehaviour
{
    [Header("Refs")]
    public Transform modelRoot;   // 要旋转的根（建议：BottleRoot；留空=当前transform）
    public Transform baseMarker;  // 瓶底锚点（放在瓶底那个点）
    public Transform muzzle;      // 瓶口（空物体）
    public Camera cam;            // 主相机

    [Header("Aim")]
    public float aimDistance = 6f;    // 从相机沿鼠标方向取点的距离（不用墙）
    public Vector3 rotationOffsetEuler; // 如果模型整体有偏转，用这个微调
    public float smooth = 0.0f;       // 0=即时；>0=转动平滑（秒）

    [Header("Clamp (限制左右/上下)")]
    public bool clampYaw = true; public float yawMin = -80, yawMax = 80;
    public bool clampPitch = true; public float pitchMin = -20, pitchMax = 50;

    [Header("Drunk 摆动")]
    public float swayYawDeg = 12f;     // 左右摆动幅度（度）
    public float swayPitchDeg = 4f;    // 上下摆动幅度（度）
    public float swaySpeed = 0.8f;     // 摆动速度（Hz，越大摆得越快）
    public float swayNoise = 0.35f;    // 每帧额外微随机（度）

    public Vector3 AimPoint { get; private set; } // 供发射用
    public Vector3 AimDir { get; private set; } // 瓶口朝向（世界方向，未归一）

    float t; // 时间

    void Reset() { cam = Camera.main; }

    void LateUpdate()
    {
        if (!cam || !muzzle) return;
        if (!modelRoot) modelRoot = transform;
        if (!baseMarker) baseMarker = modelRoot;

        t += Time.deltaTime;

        // 1) 目标点：相机射线 + 固定距离
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        AimPoint = ray.origin + ray.direction * aimDistance;

        // 2) 以瓶底为基点，把“底->口”旋到“底->目标”
        Vector3 basePos = baseMarker.position;
        Vector3 vNow = muzzle.position - basePos;                    // 当前向量
        if (vNow.sqrMagnitude < 1e-6f) return;
        float len = vNow.magnitude;

        Vector3 vTargetDir = (AimPoint - basePos);                   // 目标方向
        if (vTargetDir.sqrMagnitude < 1e-6f) return;
        vTargetDir.Normalize();
        Vector3 vTarget = vTargetDir * len;

        Quaternion worldRot = modelRoot.rotation;
        Quaternion delta = Quaternion.FromToRotation(vNow, vTarget);
        Quaternion lookWorld = delta * worldRot * Quaternion.Euler(rotationOffsetEuler);

        // 3) 转到父空间 + 角度限制
        Quaternion parentRot = modelRoot.parent ? modelRoot.parent.rotation : Quaternion.identity;
        Quaternion local = Quaternion.Inverse(parentRot) * lookWorld;
        Vector3 e = ToSigned(local.eulerAngles);

        if (clampYaw) e.y = Mathf.Clamp(e.y, yawMin, yawMax);
        if (clampPitch) e.x = Mathf.Clamp(e.x, pitchMin, pitchMax);

        // 4) 叠加“醉酒摆动”
        float yawSway = Mathf.Sin(2f * Mathf.PI * swaySpeed * t) * swayYawDeg;
        float pitchSway = Mathf.Sin(2f * Mathf.PI * (swaySpeed * 0.7f) * t + 1.3f) * swayPitchDeg;
        float nYaw = (Random.value - 0.5f) * 2f * swayNoise;
        float nPitch = (Random.value - 0.5f) * 2f * swayNoise;

        e.y += (clampYaw ? Mathf.Clamp(yawSway + nYaw, -Mathf.Abs(swayYawDeg), Mathf.Abs(swayYawDeg)) : yawSway + nYaw);
        e.x += (clampPitch ? Mathf.Clamp(pitchSway + nPitch, -Mathf.Abs(swayPitchDeg), Mathf.Abs(swayPitchDeg)) : pitchSway + nPitch);

        Quaternion targetLocal = Quaternion.Euler(e);

        // 5) 应用旋转（可平滑）
        if (smooth > 0f)
            modelRoot.localRotation = Quaternion.Slerp(modelRoot.localRotation, targetLocal, Time.deltaTime / smooth);
        else
            modelRoot.localRotation = targetLocal;

        // 6) 钉住瓶底：旋转后把 baseMarker 拉回原位
        Vector3 before = basePos;
        Vector3 after = baseMarker.position;
        modelRoot.position += (before - after);

        // 7) 输出当前瓶口指向
        AimDir = (muzzle.position - baseMarker.position);
    }

    static Vector3 ToSigned(Vector3 e)
    {
        if (e.x > 180f) e.x -= 360f;
        if (e.y > 180f) e.y -= 360f;
        if (e.z > 180f) e.z -= 360f;
        return e;
    }
}
