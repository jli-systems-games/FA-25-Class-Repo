using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UObj = UnityEngine.Object;

public class HUDInventoryUI : MonoBehaviour
{
    public static HUDInventoryUI Instance { get; private set; }

    public Sprite moneyIcon;
    public Sprite bottleIcon;

    public Color moneyColor = new Color(1f, 0.85f, 0.2f);
    public Color waterColor = new Color(0.2f, 0.6f, 1f);
    public Color energyColor = new Color(0.4f, 1f, 0.5f);

    public float drainSeconds = 15f;
    public int uiScale = 1;

    public CrewController crew;
    [SerializeField] private Font uiFont;

    float money = 100, water = 100, energy = 100;
    float drainPerSec;
    Slider moneyBar, waterBar, energyBar;
    Canvas canvas;
    RectTransform root;
    bool sleepTriggered;
    float lastDeathTime;

    void Awake()
    {
        Instance = this;

#if UNITY_6000_0_OR_NEWER
        if (uiFont == null) uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
#else
        if (uiFont == null) uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
#endif
        if (uiFont == null)
        {
            try { uiFont = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Helvetica", "Microsoft YaHei", "Noto Sans CJK SC" }, 16); }
            catch { }
        }

#if UNITY_2023_1_OR_NEWER
        if (EventSystem.current == null && UObj.FindFirstObjectByType<EventSystem>() == null)
#else
        if (EventSystem.current == null && UObj.FindObjectOfType<EventSystem>() == null)
#endif
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        if (!crew) crew = UObj.FindFirstObjectByType<CrewController>();

        BuildUI();
        drainPerSec = 100f / Mathf.Max(0.1f, drainSeconds);
    }

    void Update()
    {
        money = Mathf.Max(0, money - drainPerSec * Time.deltaTime);
        water = Mathf.Max(0, water - drainPerSec * Time.deltaTime);
        energy = Mathf.Max(0, energy - drainPerSec * Time.deltaTime);

        if (crew && crew.IsDancing) energy = Mathf.Min(100f, energy + 15f * Time.deltaTime);

        if (crew)
        {
            if ((money <= 0f || water <= 0f) && !crew.IsDead && Time.time - lastDeathTime > 0.1f)
            {
                crew.TriggerDeath();
                lastDeathTime = Time.time;
            }
            if (!sleepTriggered && energy <= 0f)
            {
                crew.TriggerSleep();
                sleepTriggered = true;
            }
            else if (sleepTriggered && energy > 5f)
            {
                sleepTriggered = false;
            }
        }

        UpdateBars();
    }

    public void RefillMoneyFull() { money = 100; UpdateBars(); }
    public void AddWater(float v) { water = Mathf.Clamp(water + v, 0, 100); UpdateBars(); }
    public void AddEnergy(float v) { energy = Mathf.Clamp(energy + v, 0, 100); UpdateBars(); }

    void BuildUI()
    {
        canvas = new GameObject("HUD_Canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 1;

        root = new GameObject("HUD_Root").AddComponent<RectTransform>();
        root.SetParent(canvas.transform, false);
        root.anchorMin = new Vector2(0, 1);
        root.anchorMax = new Vector2(0, 1);
        root.pivot = new Vector2(0, 1);
        root.anchoredPosition = new Vector2(20, -20);
        root.localScale = Vector3.one * Mathf.Clamp(uiScale, 1, 4);

        moneyBar = CreateBar("Money", moneyColor, new Vector2(300, 24), 0);
        waterBar = CreateBar("Water", waterColor, new Vector2(300, 24), 34);
        energyBar = CreateBar("Energy", energyColor, new Vector2(300, 24), 68);

        var invRoot = new GameObject("Inventory").AddComponent<RectTransform>();
        invRoot.SetParent(root, false);
        invRoot.anchorMin = invRoot.anchorMax = new Vector2(0, 1);
        invRoot.pivot = new Vector2(0, 1);
        invRoot.anchoredPosition = new Vector2(340, 0);
        invRoot.sizeDelta = new Vector2(200, 100);

        CreateDragSpawner(invRoot, "MoneyItem", moneyIcon, 0, GiveItemType.Money);
        CreateDragSpawner(invRoot, "BottleItem", bottleIcon, 90, GiveItemType.Bottle);

        UpdateBars();
    }

    Slider CreateBar(string label, Color color, Vector2 size, float yOffset)
    {
        var go = new GameObject(label);
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(root, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(0, -yOffset);
        rt.sizeDelta = size;

        var bgGO = new GameObject("BG");
        var bg = bgGO.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.35f);
        var bgRt = bgGO.GetComponent<RectTransform>();
        bgRt.SetParent(rt, false);
        bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;

        var slider = go.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;
        slider.minValue = 0; slider.maxValue = 100; slider.value = 100;

        var fillArea = new GameObject("FillArea").AddComponent<RectTransform>();
        fillArea.SetParent(rt, false);
        fillArea.anchorMin = new Vector2(0, 0); fillArea.anchorMax = new Vector2(1, 1);
        fillArea.offsetMin = new Vector2(2, 2); fillArea.offsetMax = new Vector2(-2, -2);

        var fill = new GameObject("Fill").AddComponent<Image>();
        fill.color = color;
        var fillRt = fill.GetComponent<RectTransform>();
        fillRt.SetParent(fillArea, false);
        fillRt.anchorMin = new Vector2(0, 0); fillRt.anchorMax = new Vector2(1, 1);
        fillRt.offsetMin = Vector2.zero; fillRt.offsetMax = Vector2.zero;
        slider.fillRect = fillRt;

        var textGO = new GameObject("Label");
        var txt = textGO.AddComponent<Text>();
        txt.font = uiFont; txt.fontSize = 16; txt.alignment = TextAnchor.MiddleLeft; txt.color = Color.white;
        txt.text = label + ": 100";
        var textRt = textGO.GetComponent<RectTransform>();
        textRt.SetParent(rt, false);
        textRt.anchorMin = new Vector2(0, 0);
        textRt.anchorMax = new Vector2(1, 1);
        textRt.offsetMin = new Vector2(8, 0);
        textRt.offsetMax = new Vector2(0, 0);

        return slider;
    }

    void CreateDragSpawner(RectTransform parent, string name, Sprite icon, float xOffset, GiveItemType type)
    {
        var go = new GameObject(name);
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(xOffset, 0);
        rt.sizeDelta = new Vector2(72, 72);

        var img = go.AddComponent<Image>(); img.sprite = icon; img.color = Color.white;

        var spawner = go.AddComponent<UIDragSpawner>();
        spawner.itemType = type;
        spawner.icon = icon;
    }

    void UpdateBars()
    {
        if (moneyBar) moneyBar.value = money;
        if (waterBar) waterBar.value = water;
        if (energyBar) energyBar.value = energy;

        SetBarText(moneyBar, "Money", money);
        SetBarText(waterBar, "Water", water);
        SetBarText(energyBar, "Energy", energy);
    }

    void SetBarText(Slider s, string label, float val)
    {
        if (!s) return;
        var t = s.transform.Find("Label")?.GetComponent<Text>();
        if (t) t.text = $"{label}: {val:0}";
    }
}

public class UIDragSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GiveItemType itemType;
    public Sprite icon;

    RectTransform dragIcon;
    Canvas canvas;
    Camera cam;

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvas = GetComponentInParent<Canvas>();
        if (!canvas) return;
        cam = Camera.main;
        dragIcon = new GameObject("DragIcon").AddComponent<RectTransform>();
        dragIcon.SetParent(canvas.transform, false);
        dragIcon.sizeDelta = new Vector2(64, 64);
        var img = dragIcon.gameObject.AddComponent<Image>();
        img.sprite = icon; img.raycastTarget = false;
        dragIcon.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon) dragIcon.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon) Destroy(dragIcon.gameObject);
        if (!cam) cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out var hit, 500f))
        {
            var crew = hit.collider.GetComponentInParent<CrewController>();
            if (crew) crew.GiveItem(itemType);
        }
    }
}

public enum GiveItemType { Money, Bottle }
