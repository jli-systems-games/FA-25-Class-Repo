using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 在运行时构建UI界面
/// 如果场景中已有UI元素，则使用现有的；否则创建新的
/// </summary>
public class UIBuilder : MonoBehaviour
{
    [Header("UI父对象")]
    public Canvas canvas;
    
    [Header("UI资源")]
    public Sprite feedIcon;
    public Sprite playIcon;
    public Sprite cleanIcon;
    public Sprite statBarSprite;
    
    [Header("字体")]
    public TMP_FontAsset pixelFont;
    
    void Start()
    {
        // 如果没有Canvas，创建一个
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }
        
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // 设置Canvas Scaler
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }
        
        BuildUI();
    }
    
    void BuildUI()
    {
        // 创建主UI面板
        GameObject mainPanel = CreatePanel("MainPanel", canvas.transform);
        RectTransform mainRect = mainPanel.GetComponent<RectTransform>();
        mainRect.anchorMin = Vector2.zero;
        mainRect.anchorMax = Vector2.one;
        mainRect.offsetMin = Vector2.zero;
        mainRect.offsetMax = Vector2.zero;
        
        // 设置背景色（像素风格的浅色背景）
        Image mainBg = mainPanel.GetComponent<Image>();
        mainBg.color = new Color(0.9f, 0.9f, 0.85f, 1f);
        
        // 创建左侧菜单区域
        CreateMenuArea(mainPanel.transform);
        
        // 创建右侧小猫显示区域
        CreateCatArea(mainPanel.transform);
        
        // 创建顶部信息栏
        CreateInfoBar(mainPanel.transform);
        
        // 创建子菜单面板
        CreateSubMenuPanel(mainPanel.transform);
        
        // 创建游戏结束面板
        CreateGameOverPanel(mainPanel.transform);
    }
    
    GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<Image>();
        return panel;
    }
    
    void CreateMenuArea(Transform parent)
    {
        GameObject menuArea = CreatePanel("MenuArea", parent);
        RectTransform rect = menuArea.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0.4f, 0.85f);
        rect.offsetMin = new Vector2(50, 100);
        rect.offsetMax = new Vector2(-20, -50);
        
        Image bg = menuArea.GetComponent<Image>();
        bg.color = new Color(0.8f, 0.8f, 0.75f, 0.5f);
        
        // 创建三个菜单选项
        string[] menuNames = { "Feed", "Play", "Clean" };
        Image[] menuIcons = new Image[3];
        Image[] statBars = new Image[3];
        
        for (int i = 0; i < 3; i++)
        {
            // 创建菜单项容器
            GameObject menuItem = CreatePanel($"MenuItem_{menuNames[i]}", menuArea.transform);
            RectTransform itemRect = menuItem.GetComponent<RectTransform>();
            
            float yPos = 0.75f - (i * 0.3f);
            itemRect.anchorMin = new Vector2(0.1f, yPos - 0.15f);
            itemRect.anchorMax = new Vector2(0.9f, yPos + 0.15f);
            itemRect.offsetMin = Vector2.zero;
            itemRect.offsetMax = Vector2.zero;
            
            Image itemBg = menuItem.GetComponent<Image>();
            itemBg.color = new Color(1f, 1f, 1f, 0.3f);
            
            // 创建图标
            GameObject icon = new GameObject($"Icon_{menuNames[i]}");
            icon.transform.SetParent(menuItem.transform, false);
            RectTransform iconRect = icon.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.anchoredPosition = new Vector2(60, 0);
            iconRect.sizeDelta = new Vector2(80, 80);
            
            Image iconImage = icon.AddComponent<Image>();
            iconImage.color = Color.white;
            menuIcons[i] = iconImage;
            
            // 创建状态条背景
            GameObject barBg = CreatePanel($"BarBg_{menuNames[i]}", menuItem.transform);
            RectTransform barBgRect = barBg.GetComponent<RectTransform>();
            barBgRect.anchorMin = new Vector2(0.35f, 0.3f);
            barBgRect.anchorMax = new Vector2(0.95f, 0.7f);
            barBgRect.offsetMin = Vector2.zero;
            barBgRect.offsetMax = Vector2.zero;
            
            Image barBgImage = barBg.GetComponent<Image>();
            barBgImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            
            // 创建状态条
            GameObject bar = CreatePanel($"Bar_{menuNames[i]}", barBg.transform);
            RectTransform barRect = bar.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0, 0);
            barRect.anchorMax = new Vector2(1, 1);
            barRect.offsetMin = new Vector2(5, 5);
            barRect.offsetMax = new Vector2(-5, -5);
            
            Image barImage = bar.GetComponent<Image>();
            barImage.type = Image.Type.Filled;
            barImage.fillMethod = Image.FillMethod.Horizontal;
            barImage.fillAmount = 1f;
            barImage.color = Color.green;
            statBars[i] = barImage;
        }
        
        // 将引用传递给UIManager
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            // menuIcons已移除，不再需要
            uiManager.statsBars = statBars;
            // selectionIndicators需要手动连接
        }
    }
    
    void CreateCatArea(Transform parent)
    {
        GameObject catArea = CreatePanel("CatArea", parent);
        RectTransform rect = catArea.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.45f, 0.15f);
        rect.anchorMax = new Vector2(0.95f, 0.85f);
        rect.offsetMin = new Vector2(20, 0);
        rect.offsetMax = new Vector2(-50, -50);
        
        Image bg = catArea.GetComponent<Image>();
        bg.color = new Color(0.95f, 0.95f, 0.9f, 1f);
        
        // 创建小猫显示
        GameObject catDisplay = new GameObject("CatDisplay");
        catDisplay.transform.SetParent(catArea.transform, false);
        RectTransform catRect = catDisplay.AddComponent<RectTransform>();
        catRect.anchorMin = new Vector2(0.5f, 0.5f);
        catRect.anchorMax = new Vector2(0.5f, 0.5f);
        catRect.anchoredPosition = Vector2.zero;
        catRect.sizeDelta = new Vector2(400, 400);
        
        Image catImage = catDisplay.AddComponent<Image>();
        catImage.color = Color.white;
        catImage.preserveAspect = true;
        
        // 添加动画控制器
        CatAnimationController animController = catDisplay.AddComponent<CatAnimationController>();
        animController.catImage = catImage;
        
        // 添加动画桥接
        catDisplay.AddComponent<CatAnimationBridge>();
        
        // 将引用传递给CatManager
        CatManager catManager = FindFirstObjectByType<CatManager>();
        if (catManager != null)
        {
            // CatManager不需要直接引用animator，因为使用了桥接脚本
        }
    }
    
    void CreateInfoBar(Transform parent)
    {
        GameObject infoBar = CreatePanel("InfoBar", parent);
        RectTransform rect = infoBar.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.85f);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = new Vector2(50, 0);
        rect.offsetMax = new Vector2(-50, -20);
        
        Image bg = infoBar.GetComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // 创建游戏时长文本
        GameObject playTimeObj = new GameObject("PlayTimeText");
        playTimeObj.transform.SetParent(infoBar.transform, false);
        RectTransform playTimeRect = playTimeObj.AddComponent<RectTransform>();
        playTimeRect.anchorMin = new Vector2(0, 0);
        playTimeRect.anchorMax = new Vector2(0.3f, 1);
        playTimeRect.offsetMin = new Vector2(20, 0);
        playTimeRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI playTimeText = playTimeObj.AddComponent<TextMeshProUGUI>();
        playTimeText.text = "00:00";
        playTimeText.fontSize = 36;
        playTimeText.color = Color.yellow;
        playTimeText.alignment = TextAlignmentOptions.MidlineLeft;
        if (pixelFont != null) playTimeText.font = pixelFont;
        
        // 创建年龄文本
        GameObject ageObj = new GameObject("AgeText");
        ageObj.transform.SetParent(infoBar.transform, false);
        RectTransform ageRect = ageObj.AddComponent<RectTransform>();
        ageRect.anchorMin = new Vector2(0.7f, 0);
        ageRect.anchorMax = new Vector2(1, 1);
        ageRect.offsetMin = Vector2.zero;
        ageRect.offsetMax = new Vector2(-20, 0);
        
        TextMeshProUGUI ageText = ageObj.AddComponent<TextMeshProUGUI>();
        ageText.text = "0天";
        ageText.fontSize = 36;
        ageText.color = Color.white;
        ageText.alignment = TextAlignmentOptions.MidlineRight;
        if (pixelFont != null) ageText.font = pixelFont;
        
        // 创建标题文本
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(infoBar.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.3f, 0);
        titleRect.anchorMax = new Vector2(0.7f, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "TAMAGOTCHI CAT";
        titleText.fontSize = 40;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        if (pixelFont != null) titleText.font = pixelFont;
        
        // 将引用传递给UIManager
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.playTimeText = playTimeText;
            uiManager.ageText = ageText;
        }
    }
    
    void CreateSubMenuPanel(Transform parent)
    {
        GameObject subMenu = CreatePanel("SubMenuPanel", parent);
        RectTransform rect = subMenu.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.3f, 0.3f);
        rect.anchorMax = new Vector2(0.7f, 0.7f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        Image bg = subMenu.GetComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        // 创建文本
        GameObject textObj = new GameObject("SubMenuText");
        textObj.transform.SetParent(subMenu.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.1f);
        textRect.anchorMax = new Vector2(0.9f, 0.9f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Select Option";
        text.fontSize = 32;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        if (pixelFont != null) text.font = pixelFont;
        
        // 创建提示文本
        GameObject hintObj = new GameObject("HintText");
        hintObj.transform.SetParent(subMenu.transform, false);
        RectTransform hintRect = hintObj.AddComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0.1f, 0);
        hintRect.anchorMax = new Vector2(0.9f, 0.15f);
        hintRect.offsetMin = Vector2.zero;
        hintRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI hintText = hintObj.AddComponent<TextMeshProUGUI>();
        hintText.text = "← → to select, SPACE to confirm";
        hintText.fontSize = 20;
        hintText.color = Color.gray;
        hintText.alignment = TextAlignmentOptions.Center;
        if (pixelFont != null) hintText.font = pixelFont;
        
        subMenu.SetActive(false);
        
        // 将引用传递给UIManager
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.subMenuPanel = subMenu;
            uiManager.subMenuText = text;
        }
    }
    
    void CreateGameOverPanel(Transform parent)
    {
        GameObject gameOver = CreatePanel("GameOverPanel", parent);
        RectTransform rect = gameOver.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        Image bg = gameOver.GetComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.9f);
        
        // 创建文本
        GameObject textObj = new GameObject("GameOverText");
        textObj.transform.SetParent(gameOver.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.2f, 0.3f);
        textRect.anchorMax = new Vector2(0.8f, 0.7f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "BYE DAD";
        text.fontSize = 72;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;
        if (pixelFont != null) text.font = pixelFont;
        
        gameOver.SetActive(false);
        
        // 将引用传递给UIManager
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.gameOverPanel = gameOver;
            uiManager.gameOverText = text;
        }
    }
}

