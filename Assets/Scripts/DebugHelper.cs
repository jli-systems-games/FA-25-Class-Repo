using UnityEngine;

/// <summary>
/// 调试辅助脚本
/// 按F2键查看游戏状态
/// </summary>
public class DebugHelper : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            CheckGameStatus();
        }
    }
    
    void CheckGameStatus()
    {
        Debug.Log("=== 游戏状态检查 ===");
        
        // 检查Time Scale
        Debug.Log($"Time.timeScale: {Time.timeScale}");
        
        // 检查CatManager
        if (CatManager.Instance != null)
        {
            Debug.Log($"CatManager存在: YES");
            Debug.Log($"CatManager启用: {CatManager.Instance.enabled}");
            
            if (CatManager.Instance.catData != null)
            {
                Debug.Log($"饱食度: {CatManager.Instance.catData.hunger:F1}");
                Debug.Log($"心情: {CatManager.Instance.catData.happiness:F1}");
                Debug.Log($"清洁度: {CatManager.Instance.catData.hygiene:F1}");
                Debug.Log($"衰减速率 - 饱食: {CatManager.Instance.catData.hungerDecayRate}");
                Debug.Log($"衰减速率 - 心情: {CatManager.Instance.catData.happinessDecayRate}");
                Debug.Log($"衰减速率 - 清洁: {CatManager.Instance.catData.hygieneDecayRate}");
            }
            else
            {
                Debug.LogWarning("CatData为空！");
            }
        }
        else
        {
            Debug.LogError("CatManager不存在！");
        }
        
        // 检查UIManager
        if (UIManager.Instance != null)
        {
            Debug.Log($"UIManager存在: YES");
            Debug.Log($"UIManager启用: {UIManager.Instance.enabled}");
        }
        else
        {
            Debug.LogError("UIManager不存在！");
        }
        
        // 检查GameManager
        if (GameManager.Instance != null)
        {
            Debug.Log($"GameManager存在: YES");
            Debug.Log($"游戏暂停: {GameManager.Instance.isPaused}");
        }
        
        // 检查StartupManager
        if (StartupManager.Instance != null)
        {
            Debug.Log($"StartupManager存在: YES");
        }
        
        Debug.Log("==================");
    }
}

