using UnityEngine;

/// <summary>
/// 机械摆动脚本
/// 让物体在两个位置/旋转之间晃来晃去，效果类似定格动画
/// </summary>
public class MechanicalSwing : MonoBehaviour
{
    [Header("摆动类型")]
    [Tooltip("摆动类型：位置或旋转")]
    public SwingType swingType = SwingType.Rotation;

    public enum SwingType
    {
        Position,  // 位置摆动
        Rotation   // 旋转摆动
    }

    [Header("位置摆动范围")]
    [Tooltip("起始位置（本地坐标）")]
    public Vector3 startPosition = new Vector3(-2f, 0f, 0f);

    [Tooltip("结束位置（本地坐标）")]
    public Vector3 endPosition = new Vector3(2f, 0f, 0f);

    [Header("旋转摆动范围")]
    [Tooltip("起始旋转角度（欧拉角）")]
    public Vector3 startRotation = new Vector3(-23.989f, 118.895f, 27.416f);

    [Tooltip("结束旋转角度（欧拉角）")]
    public Vector3 endRotation = new Vector3(-59.313f, 118.201f, 69.521f);

    [Header("摆动设置")]
    [Tooltip("完成一次来回摆动的时间（秒）")]
    public float swingDuration = 2f;

    [Tooltip("定格动画帧率（每秒多少帧）- 数值越低越像定格动画")]
    public float frameRate = 12f;

    [Header("控制")]
    [Tooltip("是否启用摆动")]
    public bool enableSwing = true;

    [Tooltip("开始摆动前的延迟时间")]
    public float startDelay = 0f;

    [Header("调试")]
    public bool showDebugInfo = false;

    private Vector3 initialLocalPosition;
    private Vector3 initialLocalRotation;
    private float timer = 0f;
    private float lastFrameTime = 0f;
    private bool hasStarted = false;

    void Start()
    {
        // 记录初始本地坐标和旋转
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localEulerAngles;

        // 延迟启动
        if (startDelay > 0)
        {
            Invoke("ActivateSwing", startDelay);
        }
        else
        {
            hasStarted = true;
        }
    }

    void Update()
    {
        if (!enableSwing || !hasStarted) return;

        timer += Time.deltaTime;

        // 计算定格动画的时间间隔
        float frameInterval = 1f / frameRate;

        // 只在达到下一帧时更新位置/旋转（定格动画效果）
        if (timer - lastFrameTime >= frameInterval)
        {
            lastFrameTime = timer;
            UpdateTransform();
        }
    }

    void UpdateTransform()
    {
        // 计算摆动进度（0到1之间循环）
        float progress = Mathf.PingPong(timer / swingDuration, 1f);

        if (swingType == SwingType.Position)
        {
            // 位置摆动
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, progress);
            transform.localPosition = newPosition;

            if (showDebugInfo)
            {
                Debug.Log($"位置摆动进度: {progress:F2}, 位置: {newPosition}");
            }
        }
        else // SwingType.Rotation
        {
            // 旋转摆动
            Vector3 newRotation = Vector3.Lerp(startRotation, endRotation, progress);
            transform.localEulerAngles = newRotation;

            if (showDebugInfo)
            {
                Debug.Log($"旋转摆动进度: {progress:F2}, 旋转: {newRotation}");
            }
        }
    }

    void ActivateSwing()
    {
        hasStarted = true;
        if (showDebugInfo)
        {
            Debug.Log("开始摆动");
        }
    }

    // 公开方法：启用/禁用摆动
    public void SetSwingEnabled(bool enabled)
    {
        enableSwing = enabled;
    }

    // 公开方法：重置到起始状态
    public void ResetToStart()
    {
        if (swingType == SwingType.Position)
        {
            transform.localPosition = startPosition;
        }
        else
        {
            transform.localEulerAngles = startRotation;
        }
        timer = 0f;
        lastFrameTime = 0f;
    }

    // 公开方法：设置摆动速度
    public void SetSwingDuration(float duration)
    {
        swingDuration = duration;
    }

    // 公开方法：从当前Transform捕获起始状态
    [ContextMenu("捕获当前状态为起始状态")]
    public void CaptureStartState()
    {
        if (swingType == SwingType.Position)
        {
            startPosition = transform.localPosition;
        }
        else
        {
            startRotation = transform.localEulerAngles;
        }
        Debug.Log("已捕获起始状态");
    }

    // 公开方法：从当前Transform捕获结束状态
    [ContextMenu("捕获当前状态为结束状态")]
    public void CaptureEndState()
    {
        if (swingType == SwingType.Position)
        {
            endPosition = transform.localPosition;
        }
        else
        {
            endRotation = transform.localEulerAngles;
        }
        Debug.Log("已捕获结束状态");
    }

    // 在编辑器中可视化摆动范围（仅位置模式）
    void OnDrawGizmosSelected()
    {
        if (swingType != SwingType.Position) return;

        Gizmos.color = Color.green;

        // 显示起始位置
        Vector3 worldStart = transform.parent != null
            ? transform.parent.TransformPoint(startPosition)
            : startPosition;
        Gizmos.DrawSphere(worldStart, 0.1f);

        Gizmos.color = Color.red;

        // 显示结束位置
        Vector3 worldEnd = transform.parent != null
            ? transform.parent.TransformPoint(endPosition)
            : endPosition;
        Gizmos.DrawSphere(worldEnd, 0.1f);

        // 显示连线
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(worldStart, worldEnd);
    }
}