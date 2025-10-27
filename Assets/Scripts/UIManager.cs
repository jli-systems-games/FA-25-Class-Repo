using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 负责更新UI显示，处理菜单导航
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("UI引用")]
    public GameObject[] selectionIndicators;    // 选中指示器（黑点，3个）
    public Image[] statsBars;                   // 状态条（饱食度、心情、清洁度）
    public TextMeshProUGUI playTimeText;        // 游戏时长显示
    public TextMeshProUGUI ageText;             // 年龄显示（可选）
    public GameObject subMenuPanel;             // 子菜单面板
    public TextMeshProUGUI subMenuText;         // 子菜单文本
    public GameObject gameOverPanel;            // 游戏结束面板
    public TextMeshProUGUI gameOverText;        // 游戏结束文本
    
    [Header("喂食面板")]
    public GameObject feedPanel;                // 喂食面板
    public SpriteAnimator feedAnimator;         // 喂食动画控制器
    public TextMeshProUGUI feedWarningText;     // 喂食警告文本
    
    [Header("玩耍面板")]
    public GameObject playPanel;                // 玩耍面板
    public SpriteAnimator playAnimator;         // 玩耍动画控制器
    public TextMeshProUGUI playWarningText;     // 玩耍警告文本
    
    [Header("清洁面板")]
    public GameObject cleanPanel;               // 清洁面板
    public SpriteAnimator cleanAnimator;        // 清洁动画控制器
    public TextMeshProUGUI cleanWarningText;    // 清洁警告文本
    
    [Header("冷却设置")]
    public float actionCooldown = 10f;          // 动作冷却时间（秒）
    public float actionPanelDuration = 4f;      // 面板显示时长（秒）
    
    // 当前选择
    private MenuOption currentMenuOption = MenuOption.Feed;
    private int currentSubOption = 0;
    private bool isInSubMenu = false;
    
    // 冷却时间追踪
    private float lastFeedTime = -999f;
    private float lastPlayTime = -999f;
    private float lastCleanTime = -999f;
    private MenuOption lastExecutedAction = MenuOption.Feed;
    private Coroutine actionPanelCoroutine;
    
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
        
        // 在Awake阶段就隐藏所有动作面板（在任何其他脚本之前）
        HideAllActionPanels();
    }
    
    void Start()
    {
        // 订阅输入事件
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnUpPressed += OnUpPressed;
            InputManager.Instance.OnDownPressed += OnDownPressed;
            InputManager.Instance.OnLeftPressed += OnLeftPressed;
            InputManager.Instance.OnRightPressed += OnRightPressed;
            InputManager.Instance.OnConfirmPressed += OnConfirmPressed;
        }
        
        // 订阅小猫事件
        if (CatManager.Instance != null)
        {
            CatManager.Instance.OnStatsChanged += UpdateUI;
            CatManager.Instance.OnCatDied += ShowGameOver;
        }
        
        // 初始化UI - 隐藏所有弹出面板
        if (subMenuPanel != null)
            subMenuPanel.SetActive(false);
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        UpdateUI();
        UpdateMenuSelection();
    }
    
    void OnDestroy()
    {
        // 取消订阅
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnUpPressed -= OnUpPressed;
            InputManager.Instance.OnDownPressed -= OnDownPressed;
            InputManager.Instance.OnLeftPressed -= OnLeftPressed;
            InputManager.Instance.OnRightPressed -= OnRightPressed;
            InputManager.Instance.OnConfirmPressed -= OnConfirmPressed;
        }
        
        if (CatManager.Instance != null)
        {
            CatManager.Instance.OnStatsChanged -= UpdateUI;
            CatManager.Instance.OnCatDied -= ShowGameOver;
        }
    }
    
    void Update()
    {
        // 不再需要脉动效果更新
    }
    
    /// <summary>
    /// 更新所有UI显示
    /// </summary>
    public void UpdateUI()
    {
        if (CatManager.Instance == null || CatManager.Instance.catData == null)
            return;
        
        CatData data = CatManager.Instance.catData;
        
        // 更新状态条
        UpdateStatBar(0, data.hunger);
        UpdateStatBar(1, data.happiness);
        UpdateStatBar(2, data.hygiene);
        
        // 更新游戏时长
        if (playTimeText != null)
        {
            playTimeText.text = data.GetFormattedPlayTime();
        }
        
        // 更新年龄（可选）
        if (ageText != null)
        {
            int days = data.GetAgeDays();
            ageText.text = $"{days}天";
        }
    }
    
    /// <summary>
    /// 更新单个状态条
    /// </summary>
    private void UpdateStatBar(int index, float value)
    {
        if (statsBars == null || index >= statsBars.Length || statsBars[index] == null)
            return;
        
        // 更新填充量
        statsBars[index].fillAmount = value / 100f;
        
        // 不改变颜色，保持你在Unity中设置的颜色
    }
    
    /// <summary>
    /// 更新菜单选择显示（使用黑点指示器）
    /// </summary>
    private void UpdateMenuSelection()
    {
        if (selectionIndicators == null) return;
        
        // 隐藏所有指示器，只显示当前选中的
        for (int i = 0; i < selectionIndicators.Length; i++)
        {
            if (selectionIndicators[i] != null)
            {
                selectionIndicators[i].SetActive(i == (int)currentMenuOption);
            }
        }
    }
    
    /// <summary>
    /// 显示子菜单
    /// </summary>
    private void ShowSubMenu()
    {
        isInSubMenu = true;
        currentSubOption = 0;
        
        if (subMenuPanel != null)
            subMenuPanel.SetActive(true);
        
        UpdateSubMenuText();
    }
    
    /// <summary>
    /// 隐藏子菜单
    /// </summary>
    private void HideSubMenu()
    {
        isInSubMenu = false;
        
        if (subMenuPanel != null)
            subMenuPanel.SetActive(false);
    }
    
    /// <summary>
    /// 更新子菜单文本
    /// </summary>
    private void UpdateSubMenuText()
    {
        if (subMenuText == null) return;
        
        string text = "";
        
        switch (currentMenuOption)
        {
            case MenuOption.Feed:
                switch (currentSubOption)
                {
                    case 0:
                        text = "Basic Food ($5)\n+20 Hunger, +5 Happy";
                        break;
                    case 1:
                        text = "Premium Food ($15)\n+35 Hunger, +15 Happy";
                        break;
                    case 2:
                        text = "Snack ($10)\n+10 Hunger, +20 Happy";
                        break;
                }
                break;
                
            case MenuOption.Play:
                switch (currentSubOption)
                {
                    case 0:
                        text = "Cat Stick\n+25 Happiness";
                        break;
                    case 1:
                        text = "Yarn Ball\n+20 Happiness";
                        break;
                }
                break;
                
            case MenuOption.Clean:
                switch (currentSubOption)
                {
                    case 0:
                        text = "Basic Bath ($5)\n+30 Hygiene, +5 Happy";
                        break;
                    case 1:
                        text = "Premium Bath ($12)\n+50 Hygiene, +15 Happy";
                        break;
                }
                break;
        }
        
        subMenuText.text = text;
    }
    
    /// <summary>
    /// 获取当前菜单的子选项数量
    /// </summary>
    private int GetSubOptionCount()
    {
        switch (currentMenuOption)
        {
            case MenuOption.Feed:
                return 3; // 3种食物
            case MenuOption.Play:
                return 2; // 2种玩具
            case MenuOption.Clean:
                return 2; // 2种清洁方式
            default:
                return 0;
        }
    }
    
    /// <summary>
    /// 显示游戏结束画面
    /// </summary>
    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameOverText != null && CatManager.Instance != null)
        {
            CatData data = CatManager.Instance.catData;
            int days = data.GetAgeDays();
            string playTime = data.GetFormattedPlayTime();
            
            gameOverText.text = $"GAME OVER\n\nSurvived: {days} days\nPlay Time: {playTime}";
        }
        
        // 3秒后可以重新开始
        StartCoroutine(WaitForRestart());
    }
    
    private IEnumerator WaitForRestart()
    {
        yield return new WaitForSeconds(3f);
        
        if (gameOverText != null)
        {
            gameOverText.text += "\n\nPress SPACE to restart";
        }
        
        // 等待空格键重启
        bool waiting = true;
        while (waiting)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                waiting = false;
                RestartGame();
            }
            yield return null;
        }
    }
    
    /// <summary>
    /// 重启游戏
    /// </summary>
    private void RestartGame()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        if (CatManager.Instance != null)
            CatManager.Instance.ResetGame();
        
        currentMenuOption = MenuOption.Feed;
        isInSubMenu = false;
        UpdateMenuSelection();
        UpdateUI();
    }
    
    // ===== 输入处理 =====
    
    private void OnUpPressed()
    {
        if (isInSubMenu) return;
        
        int option = (int)currentMenuOption;
        option--;
        if (option < 0) option = 2;
        currentMenuOption = (MenuOption)option;
        
        UpdateMenuSelection();
    }
    
    private void OnDownPressed()
    {
        if (isInSubMenu) return;
        
        int option = (int)currentMenuOption;
        option++;
        if (option > 2) option = 0;
        currentMenuOption = (MenuOption)option;
        
        UpdateMenuSelection();
    }
    
    private void OnLeftPressed()
    {
        if (!isInSubMenu) return;
        
        currentSubOption--;
        if (currentSubOption < 0)
            currentSubOption = GetSubOptionCount() - 1;
        
        UpdateSubMenuText();
    }
    
    private void OnRightPressed()
    {
        if (!isInSubMenu) return;
        
        currentSubOption++;
        if (currentSubOption >= GetSubOptionCount())
            currentSubOption = 0;
        
        UpdateSubMenuText();
    }
    
    private void OnConfirmPressed()
    {
        // 直接执行动作，不显示子菜单
        ExecuteActionDirectly();
    }
    
    /// <summary>
    /// 直接执行动作（不显示子菜单）
    /// </summary>
    private void ExecuteActionDirectly()
    {
        if (CatManager.Instance == null) return;
        
        bool isInCooldown = IsActionInCooldown(currentMenuOption);
        
        // 执行对应的游戏逻辑（使用默认选项：索引0）
        switch (currentMenuOption)
        {
            case MenuOption.Feed:
                CatManager.Instance.Feed(FoodType.BasicFood);  // 使用基础食物
                lastFeedTime = Time.time;
                break;
                
            case MenuOption.Play:
                CatManager.Instance.Play(ToyType.Stick);  // 使用逗猫棒
                lastPlayTime = Time.time;
                break;
                
            case MenuOption.Clean:
                CatManager.Instance.Clean(CleanType.BasicBath);  // 使用基础洗澡
                lastCleanTime = Time.time;
                break;
        }
        
        // 显示动作面板
        ShowActionPanel(currentMenuOption, isInCooldown);
        lastExecutedAction = currentMenuOption;
    }
    
    /// <summary>
    /// 检查动作是否在冷却中
    /// </summary>
    private bool IsActionInCooldown(MenuOption option)
    {
        float lastTime = 0f;
        
        switch (option)
        {
            case MenuOption.Feed:
                lastTime = lastFeedTime;
                break;
            case MenuOption.Play:
                lastTime = lastPlayTime;
                break;
            case MenuOption.Clean:
                lastTime = lastCleanTime;
                break;
        }
        
        return (Time.time - lastTime) < actionCooldown;
    }
    
    /// <summary>
    /// 显示动作面板
    /// </summary>
    private void ShowActionPanel(MenuOption action, bool isInCooldown)
    {
        // 停止之前的协程
        if (actionPanelCoroutine != null)
        {
            StopCoroutine(actionPanelCoroutine);
        }
        
        // 先隐藏所有面板
        HideAllActionPanels();
        
        // 根据动作类型显示对应面板
        GameObject panel = null;
        SpriteAnimator animator = null;
        TextMeshProUGUI warningText = null;
        
        switch (action)
        {
            case MenuOption.Feed:
                panel = feedPanel;
                animator = feedAnimator;
                warningText = feedWarningText;
                break;
            case MenuOption.Play:
                panel = playPanel;
                animator = playAnimator;
                warningText = playWarningText;
                break;
            case MenuOption.Clean:
                panel = cleanPanel;
                animator = cleanAnimator;
                warningText = cleanWarningText;
                break;
        }
        
        if (panel == null)
        {
            Debug.LogWarning($"UIManager: {action} 面板未设置！");
            return;
        }
        
        panel.SetActive(true);
        Debug.Log($"UIManager: 显示 {action} 面板");
        
        if (isInCooldown)
        {
            // 显示警告文本，不播放动画
            if (warningText != null)
            {
                warningText.gameObject.SetActive(true);
                warningText.text = GetCooldownWarningText(action);
            }
            
            // 停止动画
            if (animator != null)
            {
                animator.Stop();
                animator.gameObject.SetActive(false);
            }
        }
        else
        {
            // 隐藏警告文本，播放动画
            if (warningText != null)
            {
                warningText.gameObject.SetActive(false);
            }
            
            // 播放sprite动画
            if (animator != null)
            {
                animator.gameObject.SetActive(true);
                animator.Play();
            }
        }
        
        // 启动自动关闭协程
        actionPanelCoroutine = StartCoroutine(HideActionPanelAfterDelay(action));
    }
    
    /// <summary>
    /// 隐藏所有动作面板
    /// </summary>
    private void HideAllActionPanels()
    {
        if (feedPanel != null)
        {
            feedPanel.SetActive(false);
            Debug.Log("UIManager: 隐藏FeedPanel");
        }
        
        if (playPanel != null)
        {
            playPanel.SetActive(false);
            Debug.Log("UIManager: 隐藏PlayPanel");
        }
        
        if (cleanPanel != null)
        {
            cleanPanel.SetActive(false);
            Debug.Log("UIManager: 隐藏CleanPanel");
        }
    }
    
    /// <summary>
    /// 获取冷却警告文本
    /// </summary>
    private string GetCooldownWarningText(MenuOption action)
    {
        switch (action)
        {
            case MenuOption.Feed:
                return "Too soon!\nLet the cat digest first!";
            case MenuOption.Play:
                return "Too soon!\nThe cat needs a rest!";
            case MenuOption.Clean:
                return "Too soon!\nThe cat is still clean!";
            default:
                return "Please wait a moment!";
        }
    }
    
    /// <summary>
    /// 延迟后隐藏动作面板
    /// </summary>
    private IEnumerator HideActionPanelAfterDelay(MenuOption action)
    {
        yield return new WaitForSeconds(actionPanelDuration);
        
        // 隐藏对应的面板
        switch (action)
        {
            case MenuOption.Feed:
                if (feedPanel != null) feedPanel.SetActive(false);
                break;
            case MenuOption.Play:
                if (playPanel != null) playPanel.SetActive(false);
                break;
            case MenuOption.Clean:
                if (cleanPanel != null) cleanPanel.SetActive(false);
                break;
        }
        
        actionPanelCoroutine = null;
    }
}

