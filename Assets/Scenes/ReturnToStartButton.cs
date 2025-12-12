using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 返回开始场景按钮
/// 点击后重置所有技能数据并跳转到 Start 场景
/// </summary>
public class ReturnToStartButton : MonoBehaviour
{
    [Header("场景设置")]
    [Tooltip("要跳转的场景名称")]
    public string startSceneName = "Start";

    [Header("重置设置")]
    [Tooltip("是否重置技能数据")]
    public bool resetAbilities = true;

    [Tooltip("是否重置生命值")]
    public bool resetHealth = true;

    [Header("调试")]
    public bool showDebugInfo = true;

    /// <summary>
    /// 返回开始场景（由按钮调用）
    /// </summary>
    public void ReturnToStart()
    {
        if (showDebugInfo)
        {
            Debug.Log("🔄 返回开始场景...");
        }

        // 重置技能数据
        if (resetAbilities && PlayerAbilityManager.Instance != null)
        {
            PlayerAbilityManager.Instance.ResetAllAbilities();
            
            if (showDebugInfo)
            {
                Debug.Log("✅ 已重置技能数据");
            }
        }

        // 重置生命值
        if (resetHealth && LifeManager.Instance != null)
        {
            LifeManager.Instance.ResetLives();
            
            if (showDebugInfo)
            {
                Debug.Log("✅ 已重置生命值");
            }
        }

        // 恢复时间流速（防止暂停状态）
        Time.timeScale = 1f;

        // 跳转场景
        if (showDebugInfo)
        {
            Debug.Log($"🎬 跳转到场景: {startSceneName}");
        }

        SceneManager.LoadScene(startSceneName);
    }

    /// <summary>
    /// 快速重新开始当前场景（不重置数据）
    /// </summary>
    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 完全重置并返回开始场景
    /// </summary>
    public void FullReset()
    {
        // 销毁所有持久化的管理器
        if (PlayerAbilityManager.Instance != null)
        {
            Destroy(PlayerAbilityManager.Instance.gameObject);
        }

        if (LifeManager.Instance != null)
        {
            Destroy(LifeManager.Instance.gameObject);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneName);

        if (showDebugInfo)
        {
            Debug.Log("🔥 完全重置！已销毁所有管理器");
        }
    }
}
