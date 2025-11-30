using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.TopDownEngine;

public class AutoGameUI_Final : MonoBehaviour
{
    [Header("Phase 1: 建造阶段设置")]
    public float BuildDuration = 60f;
    public float MaxWallScore = 20f;

    [Header("Phase 2: 猎杀阶段设置")]
    public float HuntDuration = 60f;
    public int BaseHealthBonus = 30;
    [Range(0f, 1f)]
    public float EnemyHealthBonusRatio = 0.5f;

    private Text _timerText;
    private Slider _p1Slider;
    private Slider _p2Slider;
    private Text _centerText;

    private SmartScalingWall _p1Wall;
    private SmartScalingWall _p2Wall;
    private Character _p1Character;
    private Character _p2Character;

    private float _timer;
    private bool _isHuntingPhase = false;
    private bool _gameEnded = false;

    // 【新增】用于UI显示的统一最大值，而不是各自的最大血量
    private float _huntPhaseUIMax;

    void Start()
    {
        CreateOverlayInterface();
        FindWalls();
        FindCharacters();
        _timer = BuildDuration;
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

    void FindCharacters()
    {
        Character[] chars = FindObjectsByType<Character>(FindObjectsSortMode.None);
        foreach (var c in chars)
        {
            if (c.PlayerID == "Player1") _p1Character = c;
            else if (c.PlayerID == "Player2") _p2Character = c;
        }
    }

    void Update()
    {
        if (_p1Character == null || _p2Character == null)
        {
            FindCharacters();
        }

        if (_gameEnded) return;

        _timer -= Time.deltaTime;

        if (_timerText != null)
        {
            float timeToShow = Mathf.Max(0, Mathf.CeilToInt(_timer));
            string phasePrefix = _isHuntingPhase ? "HUNT: " : "BUILD: ";
            _timerText.text = phasePrefix + timeToShow.ToString() + "s";
            if (_isHuntingPhase) _timerText.color = Color.red;
        }

        // --- 进度条更新逻辑 ---
        if (!_isHuntingPhase)
        {
            // 建造阶段：显示墙分
            if (_p1Slider != null) { _p1Slider.maxValue = MaxWallScore; _p1Slider.value = (_p1Wall != null) ? _p1Wall.CurrentScore : 0; }
            if (_p2Slider != null) { _p2Slider.maxValue = MaxWallScore; _p2Slider.value = (_p2Wall != null) ? _p2Wall.CurrentScore : 0; }
        }
        else
        {
            // 【关键修复】猎杀阶段：使用统一的最大值 _huntPhaseUIMax
            // 这样如果分低，条就不是满的
            if (_p1Slider != null && _p1Character != null)
            {
                _p1Slider.maxValue = _huntPhaseUIMax;
                _p1Slider.value = _p1Character.GetComponent<Health>().CurrentHealth;
            }
            if (_p2Slider != null && _p2Character != null)
            {
                _p2Slider.maxValue = _huntPhaseUIMax;
                _p2Slider.value = _p2Character.GetComponent<Health>().CurrentHealth;
            }
        }

        if (_timer <= 0)
        {
            if (!_isHuntingPhase) StartCoroutine(TransitionToHuntPhase());
            else EndGame();
        }
    }

    IEnumerator TransitionToHuntPhase()
    {
        _timer = 9999f;
        FindCharacters();

        float score1 = (_p1Wall != null) ? _p1Wall.CurrentScore : 0;
        float score2 = (_p2Wall != null) ? _p2Wall.CurrentScore : 0;

        string result = "";
        Color color = Color.white;
        if (score1 > score2) { result = "P1 BUILD WIN!"; color = Color.cyan; }
        else if (score2 > score1) { result = "P2 BUILD WIN!"; color = Color.red; }
        else { result = "BUILD DRAW!"; color = Color.yellow; }

        ShowCenterText(result, color);
        yield return new WaitForSeconds(1.5f);

        ShowCenterText("HUNTING TIME!", Color.red);

        // 计算血量
        int p1FinalHealth = Mathf.RoundToInt(score1 + BaseHealthBonus + (score2 * EnemyHealthBonusRatio));
        int p2FinalHealth = Mathf.RoundToInt(score2 + BaseHealthBonus + (score1 * EnemyHealthBonusRatio));

        // 【关键修复】计算UI条的理论最大值 (满墙分 + 基础加成 + 满墙分的50%)
        // 这样条的长度是固定的，血量少的人条就会短
        _huntPhaseUIMax = MaxWallScore + BaseHealthBonus + (MaxWallScore * EnemyHealthBonusRatio);

        // 应用血量
        SetPlayerHealth(_p1Character, p1FinalHealth);
        SetPlayerHealth(_p2Character, p2FinalHealth);

        Debug.Log($"P1血量: {p1FinalHealth}, P2血量: {p2FinalHealth}, UI最大值: {_huntPhaseUIMax}");

        _isHuntingPhase = true;
        _timer = HuntDuration;

        yield return new WaitForSeconds(1f);
        _centerText.gameObject.SetActive(false);
    }

    void SetPlayerHealth(Character p, int healthValue)
    {
        if (p == null) return;
        Health health = p.GetComponent<Health>();
        if (health != null)
        {
            health.MaximumHealth = healthValue;
            health.CurrentHealth = healthValue;
            health.UpdateHealthBar(true);
        }
    }

    void EndGame()
    {
        _gameEnded = true;
        _timerText.text = "GAME OVER";
        ShowCenterText("GAME OVER", Color.white);
        Time.timeScale = 0f;
    }

    void ShowCenterText(string content, Color col)
    {
        if (_centerText != null)
        {
            _centerText.text = content;
            _centerText.color = col;
            _centerText.gameObject.SetActive(true);
        }
    }

    void CreateOverlayInterface()
    {
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        GameObject canvasObj = new GameObject("FinalOverlayCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject timerObj = CreateTextObj("TimerText", canvasObj.transform, 60, Color.white, TextAnchor.LowerCenter);
        RectTransform timerRect = timerObj.GetComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(0.5f, 0); timerRect.anchorMax = new Vector2(0.5f, 0); timerRect.pivot = new Vector2(0.5f, 0);
        timerRect.anchoredPosition = new Vector2(0, 50);
        _timerText = timerObj.GetComponent<Text>();

        GameObject s1 = CreateSliderObj("P1_Bar", canvasObj.transform, Color.cyan);
        RectTransform r1 = s1.GetComponent<RectTransform>();
        r1.anchorMin = new Vector2(0, 1); r1.anchorMax = new Vector2(0, 1); r1.pivot = new Vector2(0, 1);
        r1.anchoredPosition = new Vector2(30, -30);
        _p1Slider = s1.GetComponent<Slider>();

        GameObject s2 = CreateSliderObj("P2_Bar", canvasObj.transform, Color.red);
        RectTransform r2 = s2.GetComponent<RectTransform>();
        r2.anchorMin = new Vector2(1, 1); r2.anchorMax = new Vector2(1, 1); r2.pivot = new Vector2(1, 1);
        r2.anchoredPosition = new Vector2(-30, -30);
        _p2Slider = s2.GetComponent<Slider>();
        _p2Slider.direction = Slider.Direction.RightToLeft;

        GameObject winObj = CreateTextObj("CenterText", canvasObj.transform, 120, Color.white, TextAnchor.MiddleCenter);
        RectTransform winRect = winObj.GetComponent<RectTransform>();
        winRect.anchorMin = new Vector2(0.5f, 0.5f); winRect.anchorMax = new Vector2(0.5f, 0.5f); winRect.pivot = new Vector2(0.5f, 0.5f);
        winRect.anchoredPosition = Vector2.zero;
        winRect.sizeDelta = new Vector2(1200, 400);
        _centerText = winObj.GetComponent<Text>();
        _centerText.fontStyle = FontStyle.Bold;
        _centerText.gameObject.SetActive(false);
    }

    GameObject CreateTextObj(string name, Transform parent, int size, Color color, TextAnchor align)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        Text t = go.AddComponent<Text>();
        t.font = Font.CreateDynamicFontFromOSFont("Arial", size);
        if (t.font == null) t.font = Resources.FindObjectsOfTypeAll<Font>()[0];
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.rectTransform.sizeDelta = new Vector2(400, 150);
        t.raycastTarget = false;
        return go;
    }

    GameObject CreateSliderObj(string name, Transform parent, Color fillCol)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent);
        Slider slider = root.AddComponent<Slider>();
        slider.maxValue = MaxWallScore;
        slider.interactable = false;
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(400, 35);

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(root.transform);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        bgImg.rectTransform.anchorMin = Vector2.zero; bgImg.rectTransform.anchorMax = Vector2.one;
        bgImg.rectTransform.offsetMin = Vector2.zero; bgImg.rectTransform.offsetMax = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(root.transform);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero; fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero; fillAreaRect.offsetMax = Vector2.zero;

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