using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.TopDownEngine;

public class AutoGameUI : MonoBehaviour
{
    [Header("游戏设置")]
    public float GameDuration = 60f;
    public float MaxScore = 20f;

    private Text _timerText;
    private Slider _p1Slider;
    private Slider _p2Slider;
    private Text _winnerText;

    private SmartScalingWall _p1Wall;
    private SmartScalingWall _p2Wall;

    private float _timer;
    private bool _isGameOver = false;

    void Start()
    {
        // 1. 找墙
        FindWalls();

        // 2. 生成 UI (修复了字体报错)
        CreateUserInterface();

        // 3. 初始化
        _timer = GameDuration;
    }

    void FindWalls()
    {
        SmartScalingWall[] walls = FindObjectsByType<SmartScalingWall>(FindObjectsSortMode.None);
        foreach (var wall in walls)
        {
            if (wall.OwnerID == "Player1") _p1Wall = wall;
            else if (wall.OwnerID == "Player2") _p2Wall = wall;
        }
    }

    void Update()
    {
        if (_isGameOver) return;

        _timer -= Time.deltaTime;

        // 安全更新：先检查 UI 是否存在
        if (_timerText != null)
        {
            float timeToShow = Mathf.Max(0, Mathf.CeilToInt(_timer));
            _timerText.text = timeToShow.ToString() + "s";
        }

        if (_p1Slider != null) _p1Slider.value = (_p1Wall != null) ? _p1Wall.CurrentScore : 0;
        if (_p2Slider != null) _p2Slider.value = (_p2Wall != null) ? _p2Wall.CurrentScore : 0;

        if (_timer <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        _isGameOver = true;
        _timer = 0;
        if (_timerText) _timerText.text = "0s";

        float score1 = (_p1Wall != null) ? _p1Wall.CurrentScore : 0;
        float score2 = (_p2Wall != null) ? _p2Wall.CurrentScore : 0;
        string result = "";
        Color winColor = Color.white;

        if (score1 > score2) { result = "PLAYER 1 WINS!"; winColor = Color.cyan; }
        else if (score2 > score1) { result = "PLAYER 2 WINS!"; winColor = Color.red; }
        else { result = "DRAW!"; winColor = Color.yellow; }

        if (_winnerText)
        {
            _winnerText.text = result;
            _winnerText.color = winColor;
            _winnerText.gameObject.SetActive(true);
        }

        // 游戏结束暂停
        Time.timeScale = 0f;
    }

    // ----------------- UI 生成 (修复版) -----------------
    void CreateUserInterface()
    {
        // 防止重复 EventSystem
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        GameObject canvasObj = new GameObject("AutoCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // 保证在最上层

        // 关键修复：设置缩放模式，防止 UI 巨大无比
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); // 设定标准分辨率

        canvasObj.AddComponent<GraphicRaycaster>();

        // Timer (左下)
        GameObject timerObj = CreateTextObj("TimerText", canvasObj.transform, 80, Color.white, TextAnchor.LowerLeft); // 字号改大
        RectTransform timerRect = timerObj.GetComponent<RectTransform>();
        timerRect.anchorMin = Vector2.zero; timerRect.anchorMax = Vector2.zero; timerRect.pivot = Vector2.zero;
        timerRect.anchoredPosition = new Vector2(50, 50); // 稍微离边缘远点
        _timerText = timerObj.GetComponent<Text>();

        // P1 Bar (左上)
        GameObject s1 = CreateSliderObj("P1_Bar", canvasObj.transform, Color.cyan);
        RectTransform r1 = s1.GetComponent<RectTransform>();
        r1.anchorMin = new Vector2(0, 1); r1.anchorMax = new Vector2(0, 1); r1.pivot = new Vector2(0, 1);
        r1.anchoredPosition = new Vector2(50, -50);
        _p1Slider = s1.GetComponent<Slider>();

        // P2 Bar (右上)
        GameObject s2 = CreateSliderObj("P2_Bar", canvasObj.transform, Color.red);
        RectTransform r2 = s2.GetComponent<RectTransform>();
        r2.anchorMin = new Vector2(1, 1); r2.anchorMax = new Vector2(1, 1); r2.pivot = new Vector2(1, 1);
        r2.anchoredPosition = new Vector2(-50, -50);
        _p2Slider = s2.GetComponent<Slider>();
        _p2Slider.direction = Slider.Direction.RightToLeft;

        // Winner Text (中间)
        GameObject winObj = CreateTextObj("WinnerText", canvasObj.transform, 100, Color.white, TextAnchor.MiddleCenter);
        RectTransform winRect = winObj.GetComponent<RectTransform>();
        winRect.anchorMin = new Vector2(0.5f, 0.5f); winRect.anchorMax = new Vector2(0.5f, 0.5f); winRect.pivot = new Vector2(0.5f, 0.5f);
        winRect.anchoredPosition = Vector2.zero;
        winRect.sizeDelta = new Vector2(1000, 300);
        _winnerText = winObj.GetComponent<Text>();
        _winnerText.gameObject.SetActive(false);
    }

    GameObject CreateTextObj(string name, Transform parent, int size, Color color, TextAnchor align)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        Text t = go.AddComponent<Text>();

        // 修复字体：尝试用 Arial，如果不行就用默认
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (t.font == null) t.font = Font.CreateDynamicFontFromOSFont("Arial", size);

        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.rectTransform.sizeDelta = new Vector2(400, 150);
        return go;
    }

    GameObject CreateSliderObj(string name, Transform parent, Color fillCol)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent);
        Slider slider = root.AddComponent<Slider>();
        slider.maxValue = MaxScore;
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(400, 40); // 条稍微大一点

        // 背景
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(root.transform);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero; bgRect.offsetMax = Vector2.zero;

        // 填充区
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(root.transform);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero; fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(0, 0); fillAreaRect.offsetMax = new Vector2(0, 0);

        // 填充
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = fillCol;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero; fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero; fillRect.offsetMax = Vector2.zero;

        slider.targetGraphic = bgImg;
        slider.fillRect = fillRect;
        slider.direction = Slider.Direction.LeftToRight;

        return root;
    }
}