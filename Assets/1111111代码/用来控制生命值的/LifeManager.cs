using UnityEngine;

// 单例模式的生命值管理器，跨场景保持数据
public class LifeManager : MonoBehaviour
{
    // 单例实例
    private static LifeManager instance;
    public static LifeManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 尝试在场景中找到已存在的实例
                instance = FindObjectOfType<LifeManager>();

                // 如果场景中没有，创建一个新的
                if (instance == null)
                {
                    GameObject go = new GameObject("LifeManager");
                    instance = go.AddComponent<LifeManager>();
                }
            }
            return instance;
        }
    }

    [Header("生命值设置")]
    [Tooltip("最大生命值")]
    public int maxLives = 7;

    [Tooltip("当前剩余生命值")]
    public int currentLives = 7;

    [Header("调试设置")]
    public bool showDebugInfo = true;

    void Awake()
    {
        // 确保只有一个实例存在
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景不销毁

            // 初始化生命值
            currentLives = maxLives;

            if (showDebugInfo)
            {
                Debug.Log("LifeManager 初始化完成");
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 销毁重复的实例
        }
    }

    // 减少生命值
    public void LoseLife()
    {
        if (currentLives > 0)
        {
            currentLives--;

            if (showDebugInfo)
            {
                Debug.Log($"失去1条生命！剩余生命: {currentLives}/{maxLives}");
            }

            // 通知UI更新
            NotifyUIUpdate();
        }
        else
        {
            if (showDebugInfo)
            {
                Debug.Log("生命值已为0！");
            }
        }
    }

    // 增加生命值
    public void GainLife(int amount = 1)
    {
        currentLives = Mathf.Min(currentLives + amount, maxLives);

        if (showDebugInfo)
        {
            Debug.Log($"获得 {amount} 条生命！当前生命: {currentLives}/{maxLives}");
        }

        // 通知UI更新
        NotifyUIUpdate();
    }

    // 重置生命值
    public void ResetLives()
    {
        currentLives = maxLives;

        if (showDebugInfo)
        {
            Debug.Log("生命值已重置为满");
        }

        // 通知UI更新
        NotifyUIUpdate();
    }

    // 获取当前生命值
    public int GetCurrentLives()
    {
        return currentLives;
    }

    // 获取最大生命值
    public int GetMaxLives()
    {
        return maxLives;
    }

    // 检查是否还有生命
    public bool IsAlive()
    {
        return currentLives > 0;
    }

    // 新增：通知UI更新
    private void NotifyUIUpdate()
    {
        LifeUIDisplay uiDisplay = FindObjectOfType<LifeUIDisplay>();
        if (uiDisplay != null)
        {
            uiDisplay.UpdateLifeDisplay();
        }
    }
}