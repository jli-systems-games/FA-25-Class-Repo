using UnityEngine;
using UnityEngine.SceneManagement;

using GreatAchievement.Systems;

/// <summary>
/// 鼠标管理器 - 全局控制鼠标显示和锁定状态
/// 按 ESC 键可以切换鼠标状态
/// 按 P 键重置游戏并返回主菜单
/// </summary>
public class CursorManager : MonoBehaviour
{
    [Header("鼠标设置")]
    [Tooltip("游戏开始时是否隐藏鼠标")]
    public bool hideCursorOnStart = true;

    [Tooltip("按ESC键时显示鼠标")]
    public bool showCursorOnEscape = true;

    [Tooltip("切换鼠标的按键")]
    public KeyCode toggleKey = KeyCode.Escape;

    [Tooltip("是否锁定鼠标到屏幕中心")]
    public bool lockCursor = false;

    [Header("重置游戏设置")]
    [Tooltip("启用重置游戏功能")]
    public bool enableResetGame = true;

    [Tooltip("重置游戏的按键")]
    public KeyCode resetGameKey = KeyCode.P;

    [Tooltip("主菜单场景名称")]
    public string mainMenuSceneName = "Main";

    [Tooltip("重置时显示提示")]
    public bool showResetConfirmation = false;

    private static CursorManager instance;
    private bool isCursorVisible;

    public static CursorManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        // 单例模式
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化鼠标状态
        if (hideCursorOnStart)
        {
            HideCursor();
        }
        else
        {
            ShowCursor();
        }
    }

    void Update()
    {
        // 按ESC键切换鼠标显示状态
        if (showCursorOnEscape && Input.GetKeyDown(toggleKey))
        {
            ToggleCursor();
        }

        // 按P键重置游戏并返回主菜单
        if (enableResetGame && Input.GetKeyDown(resetGameKey))
        {
            ResetAndReturnToMenu();
        }
    }

    /// <summary>
    /// 显示鼠标
    /// </summary>
    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isCursorVisible = true;
    }

    /// <summary>
    /// 隐藏鼠标
    /// </summary>
    public void HideCursor()
    {
        Cursor.visible = false;
        
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
        isCursorVisible = false;
    }

    /// <summary>
    /// 切换鼠标显示状态
    /// </summary>
    public void ToggleCursor()
    {
        if (isCursorVisible)
        {
            HideCursor();
        }
        else
        {
            ShowCursor();
        }
    }

    /// <summary>
    /// 获取鼠标是否可见
    /// </summary>
    public bool IsCursorVisible()
    {
        return isCursorVisible;
    }

    /// <summary>
    /// 强制显示鼠标（用于UI界面）
    /// </summary>
    public void ForceShowCursor()
    {
        ShowCursor();
    }

    /// <summary>
    /// 强制隐藏鼠标（用于游戏中）
    /// </summary>
    public void ForceHideCursor()
    {
        HideCursor();
    }

    /// <summary>
    /// 重置游戏并返回主菜单
    /// </summary>
    public void ResetAndReturnToMenu()
    {
        if (showResetConfirmation)
        {
            Debug.Log("按 P 键重置游戏并返回主菜单...");
        }

        // 重置游戏状态
        ResetGameState();

        // 显示鼠标（主菜单需要）
        ShowCursor();

        // 加载主菜单场景
        LoadMainMenu();
    }

    /// <summary>
    /// 重置游戏状态
    /// </summary>
    private void ResetGameState()
    {
        // 恢复时间缩放
        Time.timeScale = 1f;

        // 调用 GameManager 重置能力和进度
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetAbilities();
        }
        
        // 可以在这里添加其他重置逻辑，例如：
        // - 重置玩家数据
        // - 清除临时对象
        // - 重置音频设置等
    }

    /// <summary>
    /// 加载主菜单场景
    /// </summary>
    private void LoadMainMenu()
    {
        // 检查场景是否存在于 Build Settings 中
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(mainMenuSceneName);
        
        if (sceneIndex >= 0)
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            // 如果场景名称不正确，尝试加载第一个场景（通常是主菜单）
            Debug.LogWarning($"找不到场景 '{mainMenuSceneName}'，加载第一个场景...");
            SceneManager.LoadScene(0);
        }
    }

    /// <summary>
    /// 设置主菜单场景名称
    /// </summary>
    public void SetMainMenuSceneName(string sceneName)
    {
        mainMenuSceneName = sceneName;
    }
}

