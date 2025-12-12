using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 简单的延迟场景跳转
/// 点击后等待指定时间再跳转到目标场景
/// </summary>
public class DelayedSceneTransition : MonoBehaviour
{
    [Header("场景设置")]
    [Tooltip("要跳转的场景名称")]
    public string targetSceneName = "Start";

    [Header("时间设置")]
    [Tooltip("点击后等待多少秒跳转")]
    public float delayTime = 2f;

    [Header("调试")]
    public bool showDebugInfo = true;

    /// <summary>
    /// 延迟跳转场景（由按钮调用）
    /// </summary>
    public void LoadSceneWithDelay()
    {
        if (showDebugInfo)
        {
            Debug.Log($"⏳ {delayTime} 秒后跳转到场景: {targetSceneName}");
        }

        StartCoroutine(DelayedLoad());
    }

    IEnumerator DelayedLoad()
    {
        // 等待指定时间
        yield return new WaitForSeconds(delayTime);

        if (showDebugInfo)
        {
            Debug.Log($"🎬 跳转到场景: {targetSceneName}");
        }

        // 跳转场景
        SceneManager.LoadScene(targetSceneName);
    }

    /// <summary>
    /// 立即跳转场景（无延迟）
    /// </summary>
    public void LoadSceneImmediately()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}
