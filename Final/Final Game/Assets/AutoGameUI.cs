using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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
    private GameObject _victoryPanel;
    private Canvas _mainCanvas;

    private SmartScalingWall _p1Wall;
    private SmartScalingWall _p2Wall;

    private Character _player1;
    private Character _player2;
    private Character _player3;
    private Character _player4;

    private float _timer;
    private bool _gameEnded = false;
    private bool _hasStartedSpawningGuns = false;
    private bool _hasShownWeaponIntro = false;

    void Start()
    {
        CreateOverlayInterface();
        FindWalls();
        FindPlayers();
        _timer = TotalGameDuration;

        Debug.Log($"🎮 游戏开始！总时长: {TotalGameDuration}秒");
    }

    void FindWalls()
    {
        SmartScalingWall[] walls = FindObjectsByType<SmartScalingWall>(FindObjectsSortMode.None);
        foreach (var wall in walls)
        {
            if (wall.OwnerID == "Player1") _p1Wall = wall;
            else if (wall.OwnerID == "Player2") _p2Wall = wall;
        }
        Debug.Log($"找到墙: P1={_p1Wall != null}, P2={_p2Wall != null}");
    }

    void FindPlayers()
    {
        Character[] allChars = FindObjectsByType<Character>(FindObjectsSortMode.None);
        Debug.Log($"场景中总共有 {allChars.Length} 个 Character");

        foreach (var c in allChars)
        {
            Debug.Log($"找到角色: {c.name}, PlayerID: {c.PlayerID}");

            if (c.PlayerID == "Player1") _player1 = c;
            else if (c.PlayerID == "Player2") _player2 = c;
            else if (c.PlayerID == "Player3") _player3 = c;
            else if (c.PlayerID == "Player4") _player4 = c;
        }

        Debug.Log($"✅ 玩家分配: P1={_player1 != null}, P2={_player2 != null}, P3={_player3 != null}, P4={_player4 != null}");
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

        // 检查队伍全灭
        CheckTeamElimination();

        // 👇 修复：改成 <= 0.1 防止跳过
        if (_timer <= 0.1f && !_gameEnded)
        {
            Debug.Log("⏰ 时间到！结束游戏");
            EndGame();
        }
    }

    void CheckTeamElimination()
    {
        // 检查蓝队（P1 + P3）
        bool blueTeamAlive = IsPlayerAlive(_player1) || IsPlayerAlive(_player3);

        // 检查红队（P2 + P4）
        bool redTeamAlive = IsPlayerAlive(_player2) || IsPlayerAlive(_player4);

        if (!blueTeamAlive && redTeamAlive)
        {
            Debug.Log("💀 蓝队全灭！红队获胜");
            EndGameWithWinner("RED", _player2, _player4);
        }
        else if (!redTeamAlive && blueTeamAlive)
        {
            Debug.Log("💀 红队全灭！蓝队获胜");
            EndGameWithWinner("BLUE", _player1, _player3);
        }
    }

    bool IsPlayerAlive(Character player)
    {
        if (player == null) return false;

        var health = player.GetComponent<Health>();
        if (health != null)
        {
            return health.CurrentHealth > 0;
        }

        return player.gameObject.activeInHierarchy;
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

        if (_p1Slider != null)
        {
            _p1Slider.maxValue = MaxWallScore;
            _p1Slider.value = (_p1Wall != null) ? _p1Wall.CurrentScore : 0;
        }

        if (_p2Slider != null)
        {
            _p2Slider.maxValue = MaxWallScore;
            _p2Slider.value = (_p2Wall != null) ? _p2Wall.CurrentScore : 0;
        }
    }

    void EndGame()
    {
        if (_gameEnded) return;

        _gameEnded = true;
        _timer = 0;

        float score1 = (_p1Wall != null) ? _p1Wall.CurrentScore : 0;
        float score2 = (_p2Wall != null) ? _p2Wall.CurrentScore : 0;

        Debug.Log($"🏁 游戏结束！P1分数: {score1}, P2分数: {score2}");

        if (score1 > score2)
        {
            EndGameWithWinner("BLUE", _player1, _player3);
        }
        else if (score2 > score1)
        {
            EndGameWithWinner("RED", _player2, _player4);
        }
        else
        {
            // 平局
            if (_timerText) _timerText.text = "GAME OVER";
            ShowCenterText("DRAW!", Color.yellow, 0);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void EndGameWithWinner(string teamName, Character winner1, Character winner2)
    {
        if (_gameEnded && _victoryPanel != null) return;

        _gameEnded = true;
        _timer = 0;

        if (_timerText) _timerText.text = "GAME OVER";

        Debug.Log($"🎉 {teamName} 队获胜！");

        StartCoroutine(ShowVictoryScreen(teamName, winner1, winner2));

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator ShowVictoryScreen(string teamName, Character winner1, Character winner2)
    {
        if (_centerTextBg) _centerTextBg.SetActive(false);
        if (_centerText) _centerText.gameObject.SetActive(false);

        CreateVictoryPanel(teamName, winner1, winner2);

        yield return null;
    }

    void CreateVictoryPanel(string teamName, Character winner1, Character winner2)
    {
        if (_mainCanvas == null)
        {
            Debug.LogError("找不到Canvas！");
            return;
        }

        _victoryPanel = new GameObject("VictoryPanel");
        _victoryPanel.transform.SetParent(_mainCanvas.transform, false);

        RectTransform panelRect = _victoryPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        Image panelBg = _victoryPanel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.85f);

        Color teamColor = (teamName == "BLUE") ? new Color(0.2f, 0.8f, 1f) : new Color(1f, 0.3f, 0.3f);

        GameObject container = new GameObject("Container");
        container.transform.SetParent(_victoryPanel.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(1200, 700);

        // "YOU WIN!" 文字
        GameObject winTextObj = new GameObject("WinText");
        winTextObj.transform.SetParent(container.transform, false);

        Text winText = winTextObj.AddComponent<Text>();
        winText.text = "YOU WIN!!!";
        winText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (winText.font == null) winText.font = Font.CreateDynamicFontFromOSFont("Arial", 100);
        winText.fontSize = 120;
        winText.fontStyle = FontStyle.Bold;
        winText.alignment = TextAnchor.MiddleCenter;
        winText.color = teamColor;

        RectTransform winTextRect = winTextObj.GetComponent<RectTransform>();
        winTextRect.anchorMin = new Vector2(0.5f, 1f);
        winTextRect.anchorMax = new Vector2(0.5f, 1f);
        winTextRect.pivot = new Vector2(0.5f, 1f);
        winTextRect.anchoredPosition = new Vector2(0, -50);
        winTextRect.sizeDelta = new Vector2(1200, 150);

        Outline outline1 = winTextObj.AddComponent<Outline>();
        outline1.effectColor = Color.black;
        outline1.effectDistance = new Vector2(5, -5);

        Shadow glow = winTextObj.AddComponent<Shadow>();
        glow.effectColor = teamColor;
        glow.effectDistance = new Vector2(0, 0);

        // 队伍名
        GameObject teamTextObj = new GameObject("TeamText");
        teamTextObj.transform.SetParent(container.transform, false);

        Text teamText = teamTextObj.AddComponent<Text>();
        teamText.text = $"TEAM {teamName} VICTORIOUS!";
        teamText.font = winText.font;
        teamText.fontSize = 50;
        teamText.fontStyle = FontStyle.Bold;
        teamText.alignment = TextAnchor.MiddleCenter;
        teamText.color = Color.white;

        RectTransform teamTextRect = teamTextObj.GetComponent<RectTransform>();
        teamTextRect.anchorMin = new Vector2(0.5f, 1f);
        teamTextRect.anchorMax = new Vector2(0.5f, 1f);
        teamTextRect.pivot = new Vector2(0.5f, 1f);
        teamTextRect.anchoredPosition = new Vector2(0, -200);
        teamTextRect.sizeDelta = new Vector2(1000, 80);

        Outline outline2 = teamTextObj.AddComponent<Outline>();
        outline2.effectColor = Color.black;
        outline2.effectDistance = new Vector2(3, -3);

        // 玩家头像
        GameObject playersContainer = new GameObject("PlayersContainer");
        playersContainer.transform.SetParent(container.transform, false);

        RectTransform playersRect = playersContainer.AddComponent<RectTransform>();
        playersRect.anchorMin = new Vector2(0.5f, 0.5f);
        playersRect.anchorMax = new Vector2(0.5f, 0.5f);
        playersRect.anchoredPosition = new Vector2(0, -50);
        playersRect.sizeDelta = new Vector2(800, 300);

        CreatePlayerAvatar(playersContainer.transform, winner1, -200, teamColor);
        CreatePlayerAvatar(playersContainer.transform, winner2, 200, teamColor);

        // 👇 添加烟花特效
        CreateFireworks(_victoryPanel.transform, teamColor);

        StartCoroutine(PulseAnimation(winTextObj.transform));
    }

    void CreatePlayerAvatar(Transform parent, Character player, float xPos, Color teamColor)
    {
        if (player == null) return;

        GameObject avatarObj = new GameObject($"Avatar_{player.PlayerID}");
        avatarObj.transform.SetParent(parent, false);

        RectTransform avatarRect = avatarObj.AddComponent<RectTransform>();
        avatarRect.anchoredPosition = new Vector2(xPos, 0);
        avatarRect.sizeDelta = new Vector2(250, 250);

        Image avatarBg = avatarObj.AddComponent<Image>();
        avatarBg.color = teamColor;
        avatarBg.sprite = CreateCircleSprite();

        Outline avatarOutline = avatarObj.AddComponent<Outline>();
        avatarOutline.effectColor = Color.white;
        avatarOutline.effectDistance = new Vector2(5, -5);

        GameObject idTextObj = new GameObject("PlayerID");
        idTextObj.transform.SetParent(avatarObj.transform, false);

        Text idText = idTextObj.AddComponent<Text>();
        idText.text = player.PlayerID.Replace("Player", "P");
        idText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (idText.font == null) idText.font = Font.CreateDynamicFontFromOSFont("Arial", 80);
        idText.fontSize = 100;
        idText.fontStyle = FontStyle.Bold;
        idText.alignment = TextAnchor.MiddleCenter;
        idText.color = Color.white;

        RectTransform idTextRect = idTextObj.GetComponent<RectTransform>();
        idTextRect.anchorMin = Vector2.zero;
        idTextRect.anchorMax = Vector2.one;
        idTextRect.sizeDelta = Vector2.zero;

        Outline idOutline = idTextObj.AddComponent<Outline>();
        idOutline.effectColor = Color.black;
        idOutline.effectDistance = new Vector2(3, -3);

        StartCoroutine(ScaleInAnimation(avatarObj.transform));
    }

    // 👇 创建烟花特效
    void CreateFireworks(Transform parent, Color teamColor)
    {
        // 创建多个烟花发射点
        for (int i = 0; i < 8; i++)
        {
            GameObject firework = new GameObject($"Firework_{i}");
            firework.transform.SetParent(parent, false);

            RectTransform rt = firework.AddComponent<RectTransform>();

            // 随机位置（屏幕边缘）
            float angle = i * 45f;
            float radius = 800f;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(100, 100);

            StartCoroutine(AnimateFirework(firework, teamColor, i * 0.2f));
        }
    }

    IEnumerator AnimateFirework(GameObject firework, Color color, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        while (true)
        {
            // 创建爆炸粒子
            for (int i = 0; i < 20; i++)
            {
                GameObject particle = new GameObject("Particle");
                particle.transform.SetParent(firework.transform, false);

                RectTransform prt = particle.AddComponent<RectTransform>();
                prt.sizeDelta = new Vector2(10, 10);
                prt.anchoredPosition = Vector2.zero;

                Image img = particle.AddComponent<Image>();
                img.color = Random.value > 0.5f ? color : Color.yellow;

                // 随机方向
                float angle = Random.Range(0f, 360f);
                float speed = Random.Range(100f, 300f);
                Vector2 velocity = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                ) * speed;

                StartCoroutine(AnimateParticle(particle, velocity));
            }

            yield return new WaitForSecondsRealtime(Random.Range(1.5f, 3f));
        }
    }

    IEnumerator AnimateParticle(GameObject particle, Vector2 velocity)
    {
        RectTransform rt = particle.GetComponent<RectTransform>();
        Image img = particle.GetComponent<Image>();

        Vector2 position = Vector2.zero;
        float lifetime = 1.5f;
        float timer = 0f;

        while (timer < lifetime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / lifetime;

            // 移动
            velocity.y -= 200f * Time.unscaledDeltaTime;  // 重力
            position += velocity * Time.unscaledDeltaTime;
            rt.anchoredPosition = position;

            // 缩小和淡出
            float scale = Mathf.Lerp(1f, 0f, t);
            rt.localScale = Vector3.one * scale;

            Color c = img.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            img.color = c;

            yield return null;
        }

        Destroy(particle);
    }

    Sprite CreateCircleSprite()
    {
        int size = 256;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                pixels[y * size + x] = (dist <= radius) ? Color.white : Color.clear;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    IEnumerator PulseAnimation(Transform target)
    {
        Vector3 originalScale = Vector3.one;

        while (true)
        {
            float timer = 0f;
            while (timer < 0.5f)
            {
                timer += Time.unscaledDeltaTime;
                float scale = Mathf.Lerp(1f, 1.2f, timer / 0.5f);
                target.localScale = originalScale * scale;
                yield return null;
            }

            timer = 0f;
            while (timer < 0.5f)
            {
                timer += Time.unscaledDeltaTime;
                float scale = Mathf.Lerp(1.2f, 1f, timer / 0.5f);
                target.localScale = originalScale * scale;
                yield return null;
            }
        }
    }

    IEnumerator ScaleInAnimation(Transform target)
    {
        target.localScale = Vector3.zero;

        float timer = 0f;
        float duration = 0.5f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            target.localScale = Vector3.one * t;
            yield return null;
        }

        target.localScale = Vector3.one;
    }

    // ========== 刷枪逻辑 ==========
    IEnumerator SpawnGunsRoutine()
    {
        while (!_gameEnded)
        {
            GameObject newGun = SpawnGun();

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
            return Instantiate(GunPickupPrefab, point.position, Quaternion.identity);
        }
        return null;
    }

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
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

        GameObject canvasObj = new GameObject("FinalOverlayCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        _mainCanvas = canvas;  // 👈 保存引用

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject tObj = CreateTextObj("TimerText", canvasObj.transform, 60, Color.white, TextAnchor.LowerCenter);
        RectTransform tr = tObj.GetComponent<RectTransform>();
        tr.anchorMin = new Vector2(0.5f, 1f); tr.anchorMax = new Vector2(0.5f, 1f); tr.pivot = new Vector2(0.5f, 1f);
        tr.anchoredPosition = new Vector2(0, -30);
        _timerText = tObj.GetComponent<Text>();

        GameObject s1 = CreateSliderObj("P1_Bar", canvasObj.transform, Color.cyan);
        RectTransform sr1 = s1.GetComponent<RectTransform>();
        sr1.anchorMin = new Vector2(0f, 1f); sr1.anchorMax = new Vector2(0f, 1f); sr1.pivot = new Vector2(0f, 1f);
        sr1.anchoredPosition = new Vector2(50, -50);
        _p1Slider = s1.GetComponent<Slider>();

        GameObject s2 = CreateSliderObj("P2_Bar", canvasObj.transform, Color.red);
        RectTransform sr2 = s2.GetComponent<RectTransform>();
        sr2.anchorMin = new Vector2(1f, 1f); sr2.anchorMax = new Vector2(1f, 1f); sr2.pivot = new Vector2(1f, 1f);
        sr2.anchoredPosition = new Vector2(-50, -50);
        _p2Slider = s2.GetComponent<Slider>();
        _p2Slider.direction = Slider.Direction.RightToLeft;

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

        GameObject wObj = CreateTextObj("CenterText", canvasObj.transform, 120, Color.white, TextAnchor.MiddleCenter);
        _centerText = wObj.GetComponent<Text>();
        RectTransform centerRect = wObj.GetComponent<RectTransform>();
        centerRect.sizeDelta = new Vector2(1600, 300);
        _centerText.horizontalOverflow = HorizontalWrapMode.Overflow;
        _centerText.verticalOverflow = VerticalWrapMode.Overflow;
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