using UnityEngine;

/// <summary>
/// 游戏初始化脚本
/// 确保所有必要的管理器都存在并正确初始化
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("预制体引用（可选）")]
    public GameObject gameManagerPrefab;
    public GameObject catManagerPrefab;
    public GameObject inputManagerPrefab;
    public GameObject uiManagerPrefab;
    
    void Awake()
    {
        // 确保StartupManager存在（优先使用场景中已有的）
        // 先查找场景中是否已有StartupManager（不管Instance是否为null）
        StartupManager existing = FindFirstObjectByType<StartupManager>();
        if (existing != null)
        {
            Debug.Log($"GameInitializer: 场景中已有StartupManager - {existing.gameObject.name}，不创建新的");
        }
        else
        {
            Debug.Log("GameInitializer: 场景中没有StartupManager，创建新的");
            GameObject go = new GameObject("StartupManager");
            go.AddComponent<StartupManager>();
        }
        
        // 确保GameManager存在
        if (GameManager.Instance == null)
        {
            if (gameManagerPrefab != null)
            {
                Instantiate(gameManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }
        }
        
        // 确保InputManager存在
        if (InputManager.Instance == null)
        {
            if (inputManagerPrefab != null)
            {
                Instantiate(inputManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("InputManager");
                go.AddComponent<InputManager>();
            }
        }
        
        // 确保CatManager存在
        if (CatManager.Instance == null)
        {
            if (catManagerPrefab != null)
            {
                Instantiate(catManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("CatManager");
                CatManager catManager = go.AddComponent<CatManager>();
                catManager.catData = new CatData();
            }
        }
        
        // 确保UIManager存在
        if (UIManager.Instance == null)
        {
            if (uiManagerPrefab != null)
            {
                Instantiate(uiManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("UIManager");
                go.AddComponent<UIManager>();
            }
        }
        
        // UIBuilder已禁用 - 用户使用自定义Canvas
        // 占位符生成器已禁用 - 用户使用自定义精灵
        
        // 控制提示和调试显示已禁用 - 用户可以手动添加
        
        // 添加调试辅助工具（按F2查看状态）
        if (FindFirstObjectByType<DebugHelper>() == null)
        {
            GameObject go = new GameObject("DebugHelper");
            go.AddComponent<DebugHelper>();
        }
    }
}

