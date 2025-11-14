using UnityEngine;

/// <summary>
/// 时间缩放控制器 - 确保游戏以正常速度运行
/// 防止Time.timeScale被意外修改导致游戏加速
/// </summary>
public class TimeScaleController : MonoBehaviour
{
    public static TimeScaleController Instance;

    [Header("时间缩放设置")]
    [Tooltip("是否强制锁定timeScale为1（正常速度）")]
    public bool lockTimeScale = true;

    [Tooltip("允许的最大timeScale值（防止异常加速）")]
    [Range(1f, 2f)]
    public float maxTimeScale = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 确保游戏开始时timeScale为1
        Time.timeScale = 1f;
    }

    void Update()
    {
        // 如果启用锁定，强制timeScale为1
        if (lockTimeScale)
        {
            if (Time.timeScale != 1f && Time.timeScale != 0f) // 0f是暂停状态，允许
            {
                Time.timeScale = 1f;
            }
        }
        else
        {
            // 如果没有锁定，也要限制最大值
            if (Time.timeScale > maxTimeScale && Time.timeScale != 0f)
            {
                Time.timeScale = maxTimeScale;
            }
        }
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 获取当前是否暂停
    /// </summary>
    public bool IsPaused()
    {
        return Time.timeScale == 0f;
    }
}


