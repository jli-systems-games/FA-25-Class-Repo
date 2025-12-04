using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.TopDownEngine;

public class AutoGameUI_Final : MonoBehaviour
{
    [Header("游戏设置")]
    public float TotalGameDuration = 120f;
    public float MaxWallScore = 20f;

    [Header("武器生成设置")]
    public float TimeToStartSpawningGuns = 30f;
    public float GunSpawnInterval = 10f;
    public GameObject GunPickupPrefab;
    public Transform[] SpawnPoints;

    [Header("特写效果设置")]
    public float ZoomDuration = 2.5f;
    public float StayDuration = 1.5f;
    public float ZoomSize = 5f;

    // 内部引用
    private Text _timerText;
    private Slider _p1Slider;
    private Slider _p2Slider;
    private Text _centerText;
    private GameObject _centerTextBg;

    private SmartScalingWall _p1Wall;
    private SmartScalingWall _p2Wall;

    private float _timer;
    private bool _gameEnded = false;
    private bool _hasStartedSpawningGuns = false;
    private bool _hasShownWeaponIntro = false;

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

        _timer -= Time.deltaTime;

        float timeElapsed = TotalGameDuration - _timer;
        if (!_hasStartedSpawningGuns && timeElapsed >= TimeToStartSpawningGuns)
        {
            _hasStartedSpawningGuns = true;
            StartCoroutine(SpawnGunsRoutine());
        }

        UpdateUI();

        if (_timer <= 0)
        {
            EndGame();
        }
    }

    void UpdateUI()
    {
        if (_timerText != null)
        {
            float displayTime = Mathf.Max(0, _timer);
            _timerText.text = Mathf.CeilToInt(displayTime).ToString() + "s";

            if (_hasStartedSpawningGuns) _timerText.color = Color.yellow;
            if (_timer <= 10) _timerText.color = Color.red;
        }

        if (_p1Slider != null) { _p1Slider.maxValue = MaxWallScore; _p1Slider.value = (_p1Wall != null) ? _p1Wall.CurrentScore : 0; }
        if (_p2Slider != null) { _p2Slider.maxValue = MaxWallScore; _p2Slider.value = (_p2Wall != null) ? _p2Wall.CurrentScore : 0; }
    }

    // =========================================================
    //  游戏结束逻辑
    // =========================================================
    void EndGame()
    {
        _gameEnded = true;
        _timer = 0;

        float score1 = (_p1Wall != null) ? _p1Wall.CurrentScore : 0;
        float score2 = (_p2Wall != null) ? _p2Wall.CurrentScore : 0;

        string result = "";
        Color col = Color.white;

        if (score1 > score2)
        {
            result = "PLAYER 1 WINS!";
            col = Color.cyan;
        }
        else if (score2 > score1)
        {
            result = "PLAYER 2 WINS!";
            col = Color.red;
        }
        else
        {
            result = "DRAW!";
            col = Color.yellow;
        }

        if (_timerText) _timerText.text = "GAME OVER";

        ShowCenterText(result, col, 0);

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // =========================================================
    //  刷枪逻辑（已移除头顶文字，只保留特写）
    // =========================================================
    IEnumerator SpawnGunsRoutine()
    {
        while (!_gameEnded)
        {
            GameObject newGun = SpawnGun();

            // 只有第一次生成时，播放特写镜头
            if (!_hasShownWeaponIntro && newGun != null)
            {
                _hasShownWeaponIntro = true;
                yield return StartCoroutine(PlayWeaponIntroSequence(newGun));
            }
            yield return new WaitForSeconds(GunSpawnInterval);
        }
    }

    GameObject SpawnGun()
    {
        if (GunPickupPrefab != null && SpawnPoints.Length > 0)
        {
            Transform point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            // 只生成物体，不再添加 UI
            return Instantiate(GunPickupPrefab, point.position, Quaternion.identity);
        }
        return null;
    }

    // =========================================================
    //  特写逻辑
    // =========================================================
    IEnumerator PlayWeaponIntroSequence(GameObject target)
    {
        ShowCenterText("WEAPON SPAWNED!", Color.yellow, 0);
        Time.timeScale = 0f;

        Camera mainCam = Camera.main;
        if (mainCam == null) yield break;

        Vector3 originalPos = mainCam.transform.position;
        float originalSize = mainCam.orthographic ? mainCam.orthographicSize : mainCam.fieldOfView;

        MonoBehaviour cinemachineBrain = mainCam.GetComponent("CinemachineBrain") as MonoBehaviour;
        if (cinemachineBrain != null) cinemachineBrain.enabled = false;

        Vector3 targetPos = CalculatePrecisionCameraPosition(mainCam, target.transform.position);

        float timer = 0f;
        while (timer < ZoomDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / ZoomDuration;
            t = t * t * t * (t * (t * 6f - 15f) + 10f);
            mainCam.transform.position = Vector3.Lerp(originalPos, targetPos, t);
            if (mainCam.orthographic) mainCam.orthographicSize = Mathf.Lerp(originalSize, ZoomSize, t);
            else mainCam.fieldOfView = Mathf.Lerp(originalSize, 20f, t);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(StayDuration);

        timer = 0f;
        while (timer < ZoomDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / ZoomDuration;
            t = t * t * t * (t * (t * 6f - 15f) + 10f);
            mainCam.transform.position = Vector3.Lerp(targetPos, originalPos, t);
            if (mainCam.orthographic) mainCam.orthographicSize = Mathf.Lerp(ZoomSize, originalSize, t);
            else mainCam.fieldOfView = Mathf.Lerp(20f, originalSize, t);
            yield return null;
        }

        if (cinemachineBrain != null) cinemachineBrain.enabled = true;
        Time.timeScale = 1f;
        if (_centerText)
        {
            _centerText.gameObject.SetActive(false);
            if (_centerTextBg) _centerTextBg.SetActive(false);
        }
    }

    Vector3 CalculatePrecisionCameraPosition(Camera cam, Vector3 targetWorldPos)
    {
        Plane gameplayPlane;
        if (Mathf.Abs(cam.transform.forward.y) < 0.1f) gameplayPlane = new Plane(Vector3.back, targetWorldPos);
        else gameplayPlane = new Plane(Vector3.up, targetWorldPos);

        Ray centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        float enter;

        if (gameplayPlane.Raycast(centerRay, out enter))
        {
            Vector3 currentCenterWorldPoint = centerRay.GetPoint(enter);
            Vector3 offset = cam.transform.position - currentCenterWorldPoint;
            return targetWorldPos + offset;
        }
        return new Vector3(targetWorldPos.x, targetWorldPos.y, cam.transform.position.z);
    }

    // =========================================================
    //  UI 生成部分（已修复文字框大小）
    // =========================================================
    void ShowCenterText(string content, Color col, float duration)
    {
        if (_centerText != null)
        {
            _centerText.text = content;
            _centerText.color = col;
            _centerText.gameObject.SetActive(true);
            if (_centerTextBg) _centerTextBg.SetActive(true);

            if (duration > 0) StartCoroutine(HideTextDelay(duration));
        }
    }

    IEnumerator HideTextDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (_centerText) _centerText.gameObject.SetActive(false);
        if (_centerTextBg) _centerTextBg.SetActive(false);
    }

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
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Timer
        GameObject tObj = CreateTextObj("TimerText", canvasObj.transform, 60, Color.white, TextAnchor.LowerCenter);
        RectTransform tr = tObj.GetComponent<RectTransform>();
        tr.anchorMin = new Vector2(0.5f, 1f); tr.anchorMax = new Vector2(0.5f, 1f); tr.pivot = new Vector2(0.5f, 1f);
        tr.anchoredPosition = new Vector2(0, -30);
        _timerText = tObj.GetComponent<Text>();

        // P1 Slider
        GameObject s1 = CreateSliderObj("P1_Bar", canvasObj.transform, Color.cyan);
        RectTransform sr1 = s1.GetComponent<RectTransform>();
        sr1.anchorMin = new Vector2(0f, 1f); sr1.anchorMax = new Vector2(0f, 1f); sr1.pivot = new Vector2(0f, 1f);
        sr1.anchoredPosition = new Vector2(50, -50);
        _p1Slider = s1.GetComponent<Slider>();

        // P2 Slider
        GameObject s2 = CreateSliderObj("P2_Bar", canvasObj.transform, Color.red);
        RectTransform sr2 = s2.GetComponent<RectTransform>();
        sr2.anchorMin = new Vector2(1f, 1f); sr2.anchorMax = new Vector2(1f, 1f); sr2.pivot = new Vector2(1f, 1f);
        sr2.anchoredPosition = new Vector2(-50, -50);
        _p2Slider = s2.GetComponent<Slider>();
        _p2Slider.direction = Slider.Direction.RightToLeft;

        // --- Center Text Background ---
        GameObject bgObj = new GameObject("CenterTextBG");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.7f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.4f);
        bgRect.anchorMax = new Vector2(1, 0.6f);
        bgRect.offsetMin = Vector2.zero; bgRect.offsetMax = Vector2.zero;
        bgObj.SetActive(false);
        _centerTextBg = bgObj;

        // --- Center Text (修复：尺寸变大) ---
        GameObject wObj = CreateTextObj("CenterText", canvasObj.transform, 120, Color.white, TextAnchor.MiddleCenter);
        _centerText = wObj.GetComponent<Text>();
        // 【关键修复】强制设置文字框大小，确保能放下 PLAYER 1 WINS!
        RectTransform centerRect = wObj.GetComponent<RectTransform>();
        centerRect.sizeDelta = new Vector2(1600, 300); // 宽 1600, 高 300
        _centerText.horizontalOverflow = HorizontalWrapMode.Overflow; // 允许横向溢出
        _centerText.verticalOverflow = VerticalWrapMode.Overflow; // 允许纵向溢出
        _centerText.gameObject.SetActive(false);
    }

    GameObject CreateTextObj(string name, Transform p, int s, Color c, TextAnchor a)
    {
        GameObject g = new GameObject(name); g.transform.SetParent(p, false);
        Text t = g.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (t.font == null) t.font = Font.CreateDynamicFontFromOSFont("Arial", s);
        t.fontSize = s; t.color = c; t.alignment = a;
        // 默认尺寸，上面 CenterText 会覆盖这个
        t.rectTransform.sizeDelta = new Vector2(600, 150);
        t.raycastTarget = false;

        Outline outline = g.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

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