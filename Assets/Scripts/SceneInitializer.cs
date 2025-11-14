using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景初始化器 - 确保每次进入场景都正确初始化所有管理器
/// 附加到场景中的一个GameObject上（建议附加到GameManager）
/// </summary>
public class SceneInitializer : MonoBehaviour
{
    [Header("初始化设置")]
    [Tooltip("玩家初始生命值")]
    public int initialLives = 7;
    
    [Tooltip("玩家初始技能点")]
    public int initialSkillPoints = 3;

    void Awake()
    {
        // 场景加载时立即重置
        ResetAllManagers();
    }

    void Start()
    {
        // 延迟一帧，确保所有脚本的Start()都已执行
        StartCoroutine(LateInitialize());
    }
    
    System.Collections.IEnumerator LateInitialize()
    {
        // 等待一帧，确保所有Start()方法都已执行完毕
        yield return null;
        
        // 现在安全地初始化游戏系统
        InitializeGameSystems();
    }

    void OnEnable()
    {
        // 订阅场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 取消订阅
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 场景加载完成后重置
        ResetAllManagers();
        
        // 延迟初始化游戏系统
        StartCoroutine(LateInitialize());
    }

    /// <summary>
    /// 重置所有管理器状态
    /// </summary>
    void ResetAllManagers()
    {
        // 1. 重置音乐系统
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ResetMusicSystem();
        }

        // 2. 重置玩家数据（静态变量）
        PlayerStats.Lives = initialLives;
        PlayerStats.SkillPoints = initialSkillPoints;
        PlayerStats.Rounds = 0;

        // 3. 重置游戏状态
        GameManager.GameIsOver = false;

        // 4. 重置敌人计数
        WaveSpawner.EnemiesAlive = 0;
        
        // 4.5 重置WaveSpawner状态
        if (WaveSpawner.Instance != null)
        {
            WaveSpawner.Instance.ResetWaveSpawner();
        }

        // 5. 确保时间缩放正常
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 初始化游戏系统
    /// </summary>
    void InitializeGameSystems()
    {
        // 确保BuildManager清除选择状态
        if (BuildManager.instance != null)
        {
            BuildManager.instance.ClearTurretSelection();
            BuildManager.instance.ExitSellMode();
        }

        // 确保GamePhaseManager从PreGame开始
        if (GamePhaseManager.Instance != null)
        {
            GamePhaseManager.Instance.currentStage = 0;
            GamePhaseManager.Instance.SetPhase(GamePhase.PreGame);
        }

        // 确保Shop按钮状态正确
        Shop shop = FindFirstObjectByType<Shop>();
        if (shop != null)
        {
            shop.DeselectAll();
        }
    }
}

