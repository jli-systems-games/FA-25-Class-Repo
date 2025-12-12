using UnityEngine;

/// <summary>
/// 暂停管理器
/// 启用时暂停游戏（除了UI），禁用时恢复游戏
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("调试")]
    [Tooltip("显示调试信息")]
    public bool showDebugInfo = true;

    private bool wasPaused = false;

    void OnEnable()
    {
        // 脚本被启用时，暂停游戏
        PauseGame();
    }

    void OnDisable()
    {
        // 脚本被禁用时，恢复游戏
        ResumeGame();
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    void PauseGame()
    {
        if (Time.timeScale != 0f)
        {
            Time.timeScale = 0f;
            wasPaused = true;

            if (showDebugInfo)
            {
                Debug.Log("⏸️ 游戏已暂停");
            }
        }
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    void ResumeGame()
    {
        if (wasPaused)
        {
            Time.timeScale = 1f;
            wasPaused = false;

            if (showDebugInfo)
            {
                Debug.Log("▶️ 游戏已恢复");
            }
        }
    }

    // 公开方法：手动暂停
    public void Pause()
    {
        PauseGame();
    }

    // 公开方法：手动恢复
    public void Resume()
    {
        ResumeGame();
    }

    // 公开方法：切换暂停状态
    public void TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // 获取当前是否暂停
    public bool IsPaused()
    {
        return Time.timeScale == 0f;
    }
}