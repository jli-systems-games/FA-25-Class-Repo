using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 通用游戏控制器 - 管理场景切换、暂停、退出等功能
/// 整合 SceneFader 动画效果和 PauseMenu 功能
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("必需组件引用")]
    [Tooltip("场景淡入淡出动画控制器")]
    public SceneFader sceneFader;
    
    [Tooltip("暂停菜单脚本（如果场景中已有PauseMenu）")]
    public PauseMenu pauseMenu;
    
    [Tooltip("关卡选择器脚本（如果场景中已有LevelSelector）")]
    public LevelSelector levelSelector;
    
    [Header("暂停菜单设置")]
    [Tooltip("暂停菜单面板（如果不使用PauseMenu脚本）")]
    public GameObject pauseMenuPanel;
    
    [Tooltip("暂停游戏的按键")]
    public KeyCode pauseKey = KeyCode.Escape;
    public KeyCode alternatePauseKey = KeyCode.P;
    
    [Header("场景名称")]
    [Tooltip("主菜单场景名称")]
    public string mainMenuSceneName = "Menu";
    
    [Tooltip("关卡选择场景名称")]
    public string levelSelectorSceneName = "Level Selector";
    
    [Tooltip("游戏场景名称（如果有多个关卡可以留空）")]
    public string gameSceneName = "GameScene";
    
    [Header("时间缩放")]
    [Tooltip("是否在暂停时停止游戏时间")]
    public bool freezeTimeOnPause = true;
    
    [Header("关卡进度")]
    [Tooltip("启用关卡解锁系统")]
    public bool useLevelProgression = true;
    
    private bool isPaused = false;
    
    void Start()
    {
        // 自动查找 SceneFader（如果未设置）
        if (sceneFader == null)
        {
            sceneFader = FindFirstObjectByType<SceneFader>();
        }
        
        // 自动查找 PauseMenu（如果未设置）
        if (pauseMenu == null)
        {
            pauseMenu = FindFirstObjectByType<PauseMenu>();
        }
        
        // 自动查找 LevelSelector（如果未设置）
        if (levelSelector == null)
        {
            levelSelector = FindFirstObjectByType<LevelSelector>();
        }
        
        // 确保游戏开始时不暂停
        ResumeGame();
        
        // 如果有暂停菜单，确保初始状态为隐藏
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    
    void Update()
    {
        // 如果使用 PauseMenu 脚本，则由它处理按键
        if (pauseMenu != null) return;
        
        // 否则自己处理暂停按键
        if (Input.GetKeyDown(pauseKey) || Input.GetKeyDown(alternatePauseKey))
        {
            TogglePause();
        }
    }
    
    #region 暂停功能
    
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        // 优先使用 PauseMenu 脚本
        if (pauseMenu != null && pauseMenu.ui != null && !pauseMenu.ui.activeSelf)
        {
            pauseMenu.Toggle();
            isPaused = true;
            return;
        }
        
        // 否则使用自己的暂停面板
        if (pauseMenuPanel == null) return;
        
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        
        if (freezeTimeOnPause)
        {
            Time.timeScale = 0f;
        }
    }
    
    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        // 优先使用 PauseMenu 脚本
        if (pauseMenu != null && pauseMenu.ui != null && pauseMenu.ui.activeSelf)
        {
            pauseMenu.Toggle();
            isPaused = false;
            return;
        }
        
        // 否则使用自己的暂停面板
        isPaused = false;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        Time.timeScale = 1f;
    }
    
    /// <summary>
    /// 切换暂停状态
    /// </summary>
    public void TogglePause()
    {
        // 优先使用 PauseMenu 脚本
        if (pauseMenu != null)
        {
            pauseMenu.Toggle();
            isPaused = !isPaused;
            return;
        }
        
        // 否则自己处理
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    #endregion
    
    #region 场景管理（带动画效果）
    
    /// <summary>
    /// 加载指定场景（使用淡入淡出动画）
    /// </summary>
    public void LoadScene(string sceneName)
    {
        // 恢复时间缩放
        Time.timeScale = 1f;
        
        // 使用 SceneFader 动画切换
        if (sceneFader != null)
        {
            sceneFader.FadeTo(sceneName);
        }
        else
        {
            // 如果没有 SceneFader，直接切换
            SceneManager.LoadScene(sceneName);
        }
    }
    
    /// <summary>
    /// 加载主菜单
    /// </summary>
    public void LoadMainMenu()
    {
        // 如果使用 PauseMenu 脚本，先关闭暂停
        if (pauseMenu != null && pauseMenu.ui != null && pauseMenu.ui.activeSelf)
        {
            pauseMenu.Menu();
            return;
        }
        
        LoadScene(mainMenuSceneName);
    }
    
    /// <summary>
    /// 加载关卡选择
    /// </summary>
    public void LoadLevelSelector()
    {
        LoadScene(levelSelectorSceneName);
    }
    
    /// <summary>
    /// 加载游戏场景
    /// </summary>
    public void LoadGameScene()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogWarning("游戏场景名称未设置");
            return;
        }
        
        LoadScene(gameSceneName);
    }
    
    /// <summary>
    /// 加载指定关卡（用于 LevelSelector）
    /// </summary>
    public void LoadLevel(string levelName)
    {
        // 如果使用 LevelSelector 脚本
        if (levelSelector != null)
        {
            levelSelector.Select(levelName);
            return;
        }
        
        LoadScene(levelName);
    }
    
    /// <summary>
    /// 重启当前场景（带动画）
    /// </summary>
    public void RestartCurrentScene()
    {
        // 如果使用 PauseMenu 脚本，先关闭暂停
        if (pauseMenu != null && pauseMenu.ui != null && pauseMenu.ui.activeSelf)
        {
            pauseMenu.Retry();
            return;
        }
        
        // 恢复时间缩放
        Time.timeScale = 1f;
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"重启场景: {currentSceneName}");
        LoadScene(currentSceneName);
    }
    
    /// <summary>
    /// 加载下一个场景（按Build Settings顺序）
    /// </summary>
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Time.timeScale = 1f;
            Debug.Log($"加载下一场景，索引: {nextSceneIndex}");
            
            // 解锁下一关
            if (useLevelProgression)
            {
                UnlockNextLevel();
            }
            
            // 使用动画切换
            if (sceneFader != null)
            {
                sceneFader.FadeTo(SceneManager.GetSceneByBuildIndex(nextSceneIndex).name);
            }
            else
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
        else
        {
            Debug.LogWarning("已是最后一个场景");
            LoadMainMenu();
        }
    }
    
    /// <summary>
    /// 加载上一个场景（按Build Settings顺序）
    /// </summary>
    public void LoadPreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int previousSceneIndex = currentSceneIndex - 1;
        
        if (previousSceneIndex >= 0)
        {
            Time.timeScale = 1f;
            Debug.Log($"加载上一场景，索引: {previousSceneIndex}");
            
            if (sceneFader != null)
            {
                sceneFader.FadeTo(SceneManager.GetSceneByBuildIndex(previousSceneIndex).name);
            }
            else
            {
                SceneManager.LoadScene(previousSceneIndex);
            }
        }
        else
        {
            Debug.LogWarning("已是第一个场景");
        }
    }
    
    #endregion
    
    #region 退出游戏
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("退出游戏");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    #endregion
    
    #region 关卡进度系统
    
    /// <summary>
    /// 解锁下一关
    /// </summary>
    public void UnlockNextLevel()
    {
        if (!useLevelProgression) return;
        
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);
        
        if (currentLevel >= levelReached)
        {
            PlayerPrefs.SetInt("levelReached", currentLevel + 1);
            PlayerPrefs.Save();
            Debug.Log($"已解锁关卡: {currentLevel + 1}");
        }
    }
    
    /// <summary>
    /// 解锁指定关卡
    /// </summary>
    public void UnlockLevel(int levelIndex)
    {
        if (!useLevelProgression) return;
        
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);
        
        if (levelIndex > levelReached)
        {
            PlayerPrefs.SetInt("levelReached", levelIndex);
            PlayerPrefs.Save();
            Debug.Log($"已解锁关卡: {levelIndex}");
        }
    }
    
    /// <summary>
    /// 重置关卡进度
    /// </summary>
    public void ResetLevelProgress()
    {
        PlayerPrefs.SetInt("levelReached", 1);
        PlayerPrefs.Save();
        Debug.Log("关卡进度已重置");
    }
    
    /// <summary>
    /// 获取已解锁的最高关卡
    /// </summary>
    public int GetLevelReached()
    {
        return PlayerPrefs.GetInt("levelReached", 1);
    }
    
    /// <summary>
    /// 完成当前关卡（解锁下一关并切换）
    /// </summary>
    public void CompleteLevel()
    {
        if (useLevelProgression)
        {
            UnlockNextLevel();
        }
        LoadNextScene();
    }
    
    #endregion
    
    #region 工具方法
    
    /// <summary>
    /// 获取当前是否暂停
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }
    
    /// <summary>
    /// 延迟加载场景
    /// </summary>
    public void LoadSceneWithDelay(string sceneName, float delay)
    {
        StartCoroutine(LoadSceneDelayed(sceneName, delay));
    }
    
    private System.Collections.IEnumerator LoadSceneDelayed(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadScene(sceneName);
    }
    
    /// <summary>
    /// 设置时间缩放
    /// ⚠️ 警告：此方法仅用于特殊效果，正常游戏应保持timeScale=1
    /// </summary>
    public void SetTimeScale(float scale)
    {
        // 限制范围，防止异常加速导致音乐变调
        Time.timeScale = Mathf.Clamp(scale, 0f, 2f);
    }
    
    /// <summary>
    /// 获取 SceneFader 引用
    /// </summary>
    public SceneFader GetSceneFader()
    {
        return sceneFader;
    }
    
    /// <summary>
    /// 获取 PauseMenu 引用
    /// </summary>
    public PauseMenu GetPauseMenu()
    {
        return pauseMenu;
    }
    
    /// <summary>
    /// 获取 LevelSelector 引用
    /// </summary>
    public LevelSelector GetLevelSelector()
    {
        return levelSelector;
    }
    
    #endregion
}

