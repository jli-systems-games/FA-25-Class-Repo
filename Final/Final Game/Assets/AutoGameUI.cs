using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.TopDownEngine;

public class AutoGameUI_Final : MonoBehaviour
{
    [Header("游戏设置")]
    public float TotalGameDuration = 120f; // 总时长 2分钟
    public float MaxWallScore = 20f;

    [Header("武器生成设置")]
    public float TimeToStartSpawningGuns = 30f; // 30秒后开始刷枪
    public float GunSpawnInterval = 10f; // 每10秒刷一把
    public GameObject GunPickupPrefab;
    public Transform[] SpawnPoints;

    // 内部引用
    private Text _timerText;
    private Slider _p1Slider;
    private Slider _p2Slider;
    private Text _centerText;

    private SmartScalingWall _p1Wall;
    private SmartScalingWall _p2Wall;

    private float _timer;
    private bool _gameEnded = false;
    private bool _hasStartedSpawningGuns = false;

    void Start()
    {
        CreateOverlayInterface();
        FindWalls();
        _timer = TotalGameDuration;
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
        if (_gameEnded) return;

        // 1. 倒计时
        _timer -= Time.deltaTime;

        // 2. 检查是否该刷枪了
        float timeElapsed = TotalGameDuration - _timer;
        if (!_hasStartedSpawningGuns && timeElapsed >= TimeToStartSpawningGuns)
        {
            _hasStartedSpawningGuns = true;
            StartCoroutine(SpawnGunsRoutine());
            ShowCenterText("WEAPONS SPAWNING!", Color.red, 2f);
        }

        // 3. 更新 UI
        UpdateUI();

        // 4. 游戏结束
        if (_timer <= 0)
        {
            EndGame();
        }
    }

    void UpdateUI()
    {
        if (_timerText != null)
        {
            _timerText.text = Mathf.CeilToInt(_timer).ToString() + "s";
            if (_hasStartedSpawningGuns) _timerText.color = Color.yellow;
        }

        // 显示墙的分数
        if (_p1Slider != null) { _p1Slider.maxValue = MaxWallScore; _p1Slider.value = (_p1Wall != null) ? _p1Wall.CurrentScore : 0; }
        if (_p2Slider != null) { _p2Slider.maxValue = MaxWallScore; _p2Slider.value = (_p2Wall != null) ? _p2Wall.CurrentScore : 0; }
    }

    IEnumerator SpawnGunsRoutine()
    {
        while (!_gameEnded)
        {
            SpawnGun();
            yield return new WaitForSeconds(GunSpawnInterval);
        }
    }

    void SpawnGun()
    {
        if (GunPickupPrefab != null && SpawnPoints.Length > 0)
        {
            Transform point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            Instantiate(GunPickupPrefab, point.position, Quaternion.identity);
        }
    }

    void EndGame()
    {
        _gameEnded = true;

        float score1 = (_p1Wall != null) ? _p1Wall.CurrentScore : 0;
        float score2 = (_p2Wall != null) ? _p2Wall.CurrentScore : 0;

        string result = "";
        Color col = Color.white;

        if (score1 > score2) { result = "PLAYER 1 WINS!"; col = Color.cyan; }
        else if (score2 > score1) { result = "PLAYER 2 WINS!"; col = Color.red; }
        else { result = "DRAW!"; col = Color.yellow; }

        if (_timerText) _timerText.text = "GAME OVER";
        ShowCenterText(result, col, 0); // 0表示不自动隐藏
        Time.timeScale = 0f;
    }

    void ShowCenterText(string content, Color col, float duration)
    {
        if (_centerText != null)
        {
            _centerText.text = content;
            _centerText.color = col;
            _centerText.gameObject.SetActive(true);
            if (duration > 0) StartCoroutine(HideTextDelay(duration));
        }
    }

    IEnumerator HideTextDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_centerText) _centerText.gameObject.SetActive(false);
    }

    // ---------------- UI 生成部分 (关键修改：设置锚点) ----------------
    void CreateOverlayInterface()
    {
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null) new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

        GameObject canvasObj = new GameObject("FinalOverlayCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f; // 兼顾横竖屏

        canvasObj.AddComponent<GraphicRaycaster>();

        // --- 1. 倒计时 (Top Center) ---
        GameObject tObj = CreateTextObj("TimerText", canvasObj.transform, 60, Color.white, TextAnchor.LowerCenter);
        RectTransform tr = tObj.GetComponent<RectTransform>();
        tr.anchorMin = new Vector2(0.5f, 1f); // 锚点设为顶部中间
        tr.anchorMax = new Vector2(0.5f, 1f);
        tr.pivot = new Vector2(0.5f, 1f);
        tr.anchoredPosition = new Vector2(0, -30); // 距离顶端向下30
        _timerText = tObj.GetComponent<Text>();

        // --- 2. P1 Slider (Top Left) ---
        GameObject s1 = CreateSliderObj("P1_Bar", canvasObj.transform, Color.cyan);
        RectTransform sr1 = s1.GetComponent<RectTransform>();
        sr1.anchorMin = new Vector2(0f, 1f); // 锚点设为左上角
        sr1.anchorMax = new Vector2(0f, 1f);
        sr1.pivot = new Vector2(0f, 1f); // 轴心左上
        sr1.anchoredPosition = new Vector2(50, -50); // 距离左上角的偏移
        _p1Slider = s1.GetComponent<Slider>();

        // --- 3. P2 Slider (Top Right) ---
        GameObject s2 = CreateSliderObj("P2_Bar", canvasObj.transform, Color.red);
        RectTransform sr2 = s2.GetComponent<RectTransform>();
        sr2.anchorMin = new Vector2(1f, 1f); // 锚点设为右上角
        sr2.anchorMax = new Vector2(1f, 1f);
        sr2.pivot = new Vector2(1f, 1f); // 轴心右上
        sr2.anchoredPosition = new Vector2(-50, -50); // 距离右上角的偏移
        _p2Slider = s2.GetComponent<Slider>();
        _p2Slider.direction = Slider.Direction.RightToLeft;

        // --- 4. Center Text (Middle) ---
        GameObject wObj = CreateTextObj("CenterText", canvasObj.transform, 120, Color.white, TextAnchor.MiddleCenter);
        // 中间文字保持默认居中即可
        _centerText = wObj.GetComponent<Text>();
        _centerText.gameObject.SetActive(false);
    }

    GameObject CreateTextObj(string name, Transform p, int s, Color c, TextAnchor a)
    {
        GameObject g = new GameObject(name); g.transform.SetParent(p, false);
        Text t = g.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (t.font == null) t.font = Font.CreateDynamicFontFromOSFont("Arial", s);
        t.fontSize = s; t.color = c; t.alignment = a;
        t.rectTransform.sizeDelta = new Vector2(600, 150);
        t.raycastTarget = false;
        return g;
    }

    GameObject CreateSliderObj(string name, Transform p, Color c)
    {
        GameObject r = new GameObject(name); r.transform.SetParent(p, false);
        Slider s = r.AddComponent<Slider>(); s.interactable = false;
        RectTransform rt = r.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(400, 35);

        GameObject b = new GameObject("Background"); b.transform.SetParent(r.transform, false);
        Image bi = b.AddComponent<Image>(); bi.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        bi.rectTransform.anchorMin = Vector2.zero; bi.rectTransform.anchorMax = Vector2.one;
        bi.rectTransform.offsetMin = Vector2.zero; bi.rectTransform.offsetMax = Vector2.zero;

        GameObject f = new GameObject("Fill Area"); f.transform.SetParent(r.transform, false);
        RectTransform fr = f.AddComponent<RectTransform>();
        fr.anchorMin = Vector2.zero; fr.anchorMax = Vector2.one;
        fr.offsetMin = Vector2.zero; fr.offsetMax = Vector2.zero;

        GameObject fi = new GameObject("Fill"); fi.transform.SetParent(f.transform, false);
        Image fii = fi.AddComponent<Image>(); fii.color = c;
        RectTransform fir = fi.GetComponent<RectTransform>();
        fir.anchorMin = Vector2.zero; fir.anchorMax = Vector2.one;
        fir.offsetMin = Vector2.zero; fir.offsetMax = Vector2.zero;

        s.targetGraphic = bi; s.fillRect = fir; s.direction = Slider.Direction.LeftToRight;
        return r;
    }
}