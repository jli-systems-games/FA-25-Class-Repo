using UnityEngine;
using TMPro;

/// <summary>
/// 启动流程管理器
/// 管理游戏开始前的两个界面切换
/// </summary>
public class StartupManager : MonoBehaviour
{
    public static StartupManager Instance { get; private set; }
    
    [Header("启动界面")]
    public GameObject startPanel1;              // 第一个启动界面
    public GameObject startPanel2;              // 第二个启动界面
    public GameObject gameUI;                   // 游戏主界面
    
    [Header("提示文本（可选）")]
    public TextMeshProUGUI startPanel1Hint;     // 第一个界面的提示文本
    public TextMeshProUGUI startPanel2Hint;     // 第二个界面的提示文本
    
    [Header("设置")]
    public string panel1HintText = "Press ↓ to continue";
    public string panel2HintText = "Press ↓ to start game";
    public bool disableGameManagersOnStart = true;  // 启动时禁用游戏管理器
    
    // 当前启动状态
    private enum StartupState
    {
        Panel1,     // 显示第一个界面
        Panel2,     // 显示第二个界面
        InGame      // 进入游戏
    }
    
    private StartupState currentState = StartupState.Panel1;
    private bool isTransitioning = false;
    
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
        // 调试：显示面板状态
        Debug.Log($"StartupManager: startPanel1 = {(startPanel1 != null ? startPanel1.name : "NULL")}");
        Debug.Log($"StartupManager: startPanel2 = {(startPanel2 != null ? startPanel2.name : "NULL")}");
        Debug.Log($"StartupManager: gameUI = {(gameUI != null ? gameUI.name : "NULL")}");
        
        // 如果没有设置启动面板，直接跳过启动流程
        if (startPanel1 == null && startPanel2 == null)
        {
            Debug.Log("StartupManager: 未设置启动面板，直接进入游戏模式");
            // 不调用SkipToGame，让游戏自然启动
            // 只需要通知CatManager开始游戏
            if (CatManager.Instance != null)
            {
                CatManager.Instance.StartGame();
                Debug.Log("StartupManager: CatManager.StartGame() 已调用");
            }
            return;
        }
        
        Debug.Log("StartupManager: 检测到启动面板，初始化启动流程");
        InitializeStartup();
    }
    
    void Update()
    {
        if (isTransitioning) return;
        
        // 检测下键
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log($"StartupManager.Update: 检测到下键，当前状态={currentState}");
            AdvanceState();
        }
    }
    
    /// <summary>
    /// 初始化启动流程
    /// </summary>
    private void InitializeStartup()
    {
        // 自动查找游戏UI（如果没有手动指定）
        if (gameUI == null)
        {
            // 查找Canvas下的MainPanel或第一个激活的Panel
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"StartupManager: 找到Canvas - {canvas.name}");
                
                // 尝试查找名为MainPanel的对象
                Transform mainPanel = canvas.transform.Find("MainPanel");
                if (mainPanel != null)
                {
                    gameUI = mainPanel.gameObject;
                    Debug.Log($"StartupManager: 自动设置gameUI为 MainPanel");
                }
                else
                {
                    Debug.LogWarning("StartupManager: 未找到MainPanel，请手动连接gameUI字段！");
                    // 列出Canvas下的子对象
                    Debug.Log("Canvas下的子对象：");
                    for (int i = 0; i < canvas.transform.childCount; i++)
                    {
                        Debug.Log($"  - {canvas.transform.GetChild(i).name}");
                    }
                }
            }
        }
        else
        {
            Debug.Log($"StartupManager: gameUI已手动设置为 {gameUI.name}");
        }
        
        // 显示第一个界面
        ShowPanel1();
        
        // 设置提示文本
        if (startPanel1Hint != null)
        {
            startPanel1Hint.text = panel1HintText;
        }
        
        if (startPanel2Hint != null)
        {
            startPanel2Hint.text = panel2HintText;
        }
        
        // 禁用游戏管理器（可选）
        Debug.Log($"StartupManager: disableGameManagersOnStart = {disableGameManagersOnStart}");
        if (disableGameManagersOnStart)
        {
            Debug.Log("StartupManager: 禁用游戏管理器");
            DisableGameManagers();
        }
        
        Debug.Log("StartupManager: InitializeStartup 完成");
    }
    
    /// <summary>
    /// 推进到下一个状态
    /// </summary>
    private void AdvanceState()
    {
        Debug.Log($"StartupManager.AdvanceState: 当前状态={currentState}");
        isTransitioning = true;
        
        switch (currentState)
        {
            case StartupState.Panel1:
                Debug.Log("StartupManager: Panel1 → Panel2");
                currentState = StartupState.Panel2;
                ShowPanel2();
                break;
                
            case StartupState.Panel2:
                Debug.Log("StartupManager: Panel2 → InGame，准备开始游戏");
                currentState = StartupState.InGame;
                StartGame();
                break;
                
            default:
                Debug.LogWarning($"StartupManager: 未处理的状态 {currentState}");
                break;
        }
        
        isTransitioning = false;
    }
    
    /// <summary>
    /// 显示第一个界面
    /// </summary>
    private void ShowPanel1()
    {
        if (startPanel1 != null)
        {
            startPanel1.SetActive(true);
            Debug.Log($"StartupManager: 启动面板1已激活");
        }
        
        if (startPanel2 != null)
        {
            startPanel2.SetActive(false);
        }
        
        if (gameUI != null)
        {
            gameUI.SetActive(false);
            Debug.Log($"StartupManager: 隐藏游戏UI - {gameUI.name}");
        }
        else
        {
            Debug.LogWarning("StartupManager: gameUI未设置，无法隐藏游戏界面！");
        }
        
        Debug.Log("显示启动界面1");
    }
    
    /// <summary>
    /// 显示第二个界面
    /// </summary>
    private void ShowPanel2()
    {
        if (startPanel1 != null) startPanel1.SetActive(false);
        if (startPanel2 != null) startPanel2.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        
        Debug.Log("显示启动界面2");
    }
    
    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartGame()
    {
        Debug.Log("StartupManager.StartGame() 被调用");
        
        if (startPanel1 != null) startPanel1.SetActive(false);
        if (startPanel2 != null) startPanel2.SetActive(false);
        if (gameUI != null)
        {
            gameUI.SetActive(true);
            Debug.Log($"StartupManager: 激活游戏UI - {gameUI.name}");
        }
        
        // 启用游戏管理器
        if (disableGameManagersOnStart)
        {
            Debug.Log("StartupManager: 启用游戏管理器");
            EnableGameManagers();
        }
        
        // 通知CatManager开始游戏
        if (CatManager.Instance != null)
        {
            Debug.Log("StartupManager: 调用 CatManager.StartGame()");
            CatManager.Instance.StartGame();
        }
        else
        {
            Debug.LogError("StartupManager: CatManager.Instance 为空！");
        }
        
        Debug.Log("StartupManager: 游戏开始！");
    }
    
    /// <summary>
    /// 禁用游戏管理器
    /// </summary>
    private void DisableGameManagers()
    {
        if (CatManager.Instance != null)
        {
            CatManager.Instance.enabled = false;
        }
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.enabled = false;
        }
        
        if (InputManager.Instance != null)
        {
            InputManager.Instance.enabled = false;
        }
    }
    
    /// <summary>
    /// 启用游戏管理器
    /// </summary>
    private void EnableGameManagers()
    {
        Debug.Log("StartupManager.EnableGameManagers: 启用所有游戏管理器");
        
        if (CatManager.Instance != null)
        {
            CatManager.Instance.enabled = true;
            Debug.Log($"StartupManager: CatManager已启用，enabled={CatManager.Instance.enabled}");
        }
        else
        {
            Debug.LogWarning("StartupManager: CatManager.Instance为空，无法启用");
        }
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.enabled = true;
            Debug.Log($"StartupManager: UIManager已启用，enabled={UIManager.Instance.enabled}");
        }
        
        if (InputManager.Instance != null)
        {
            InputManager.Instance.enabled = true;
            Debug.Log($"StartupManager: InputManager已启用，enabled={InputManager.Instance.enabled}");
        }
    }
    
    /// <summary>
    /// 跳过启动界面，直接进入游戏（用于测试）
    /// </summary>
    public void SkipToGame()
    {
        Debug.Log("StartupManager: SkipToGame() 被调用");
        currentState = StartupState.InGame;
        
        // 确保游戏UI激活
        if (gameUI != null)
        {
            gameUI.SetActive(true);
            Debug.Log($"StartupManager: 激活游戏UI - {gameUI.name}");
        }
        else
        {
            Debug.LogWarning("StartupManager: gameUI为空，尝试自动查找MainPanel");
            // 自动查找MainPanel
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                Transform mainPanel = canvas.transform.Find("MainPanel");
                if (mainPanel != null)
                {
                    gameUI = mainPanel.gameObject;
                    gameUI.SetActive(true);
                    Debug.Log($"StartupManager: 找到并激活MainPanel");
                }
            }
        }
        
        // 启用游戏管理器
        EnableGameManagers();
        
        // 通知CatManager开始游戏
        if (CatManager.Instance != null)
        {
            CatManager.Instance.StartGame();
            Debug.Log("StartupManager: 已调用CatManager.StartGame()");
        }
        else
        {
            Debug.LogError("StartupManager: CatManager.Instance为空！");
        }
        
        Debug.Log("StartupManager: 游戏开始（跳过启动界面）！");
    }
}

