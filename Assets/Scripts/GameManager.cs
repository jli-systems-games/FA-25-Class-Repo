using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 总体游戏流程控制，包括游戏开始、结束、时间管理等
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("游戏设置")]
    public bool isPaused = false;
    
    [Header("时间设置")]
    public float gameTimeScale = 1f; // 游戏时间倍速
    
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
        }
    }
    
    void Start()
    {
        // 设置目标帧率
        Application.targetFrameRate = 60;
        
        // 初始化游戏
        InitializeGame();
    }
    
    void Update()
    {
        // 更新时间缩放
        Time.timeScale = isPaused ? 0f : gameTimeScale;
        
        // ESC键暂停/继续
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    /// <summary>
    /// 初始化游戏
    /// </summary>
    private void InitializeGame()
    {
        isPaused = false;
        Time.timeScale = gameTimeScale;
    }
    
    /// <summary>
    /// 暂停/继续游戏
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
    }
    
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;
    }
    
    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
    }
    
    /// <summary>
    /// 重新加载当前场景
    /// </summary>
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

