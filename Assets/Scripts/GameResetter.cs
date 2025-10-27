using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏重置管理器
/// 按P键或游戏结束后按空格/回车重置游戏
/// </summary>
public class GameResetter : MonoBehaviour
{
    public static GameResetter Instance { get; private set; }
    
    [Header("设置")]
    [Tooltip("是否显示调试信息")]
    public bool showDebugInfo = true;
    
    private bool isGameOver = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // 订阅游戏结束事件
        if (CatManager.Instance != null)
        {
            CatManager.Instance.OnCatDied += OnGameOver;
            if (showDebugInfo)
            {
                Debug.Log("GameResetter: 已订阅猫咪死亡事件");
            }
        }
    }
    
    void OnDestroy()
    {
        // 取消订阅
        if (CatManager.Instance != null)
        {
            CatManager.Instance.OnCatDied -= OnGameOver;
        }
    }
    
    void Update()
    {
        // 按P键随时重置
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (showDebugInfo)
            {
                Debug.Log("GameResetter: 检测到P键，重置游戏");
            }
            ResetGame();
        }
        
        // 游戏结束后，按空格或回车重置
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (showDebugInfo)
                {
                    Debug.Log("GameResetter: 游戏结束后检测到空格/回车键，重置游戏");
                }
                ResetGame();
            }
        }
    }
    
    /// <summary>
    /// 游戏结束回调
    /// </summary>
    private void OnGameOver()
    {
        isGameOver = true;
        if (showDebugInfo)
        {
            Debug.Log("GameResetter: 游戏结束！按空格或回车重置游戏");
        }
    }
    
    /// <summary>
    /// 重置游戏（重新加载场景，回到第一个页面）
    /// </summary>
    public void ResetGame()
    {
        if (showDebugInfo)
        {
            Debug.Log("=== GameResetter: 重新加载场景，回到启动页面 ===");
        }
        
        // 直接重新加载场景，这样所有状态都会重置，并回到第一个启动页面
        ReloadScene();
    }
    
    /// <summary>
    /// 完全重新加载场景（备用方案）
    /// </summary>
    public void ReloadScene()
    {
        if (showDebugInfo)
        {
            Debug.Log("GameResetter: 重新加载场景");
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

