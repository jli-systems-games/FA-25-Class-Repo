using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GarageRuntimeUI : MonoBehaviour
{
    public Camera cam;
    public string runSceneName = "Run";
    [Range(0.12f, 0.5f)] public float leftWidth = 0.22f;
    [Range(0.10f, 0.5f)] public float rightWidth = 0.18f;
    [Range(-0.2f, 0.2f)] public float centerNudgeX = 0f;
    public int paletteColumns = 2;
    public Vector2 paletteCell = new Vector2(180, 180);
    public Vector2 paletteSpacing = new Vector2(12, 12);
    public float wheelTargetDiameter = 1.2f;
    public float wheelMinScale = 0.25f;
    public float wheelMaxScale = 2.0f;
    public TMP_FontAsset boldFont;

    RectTransform canvasRT, leftPanel, rightPanel, centerArea;
    RectTransform scrollViewport, scrollContent;
    BoxCollider2D buildBounds;
    GameObject currentBody;
    FrameType currentFrame = FrameType.Car;
    List<GameObject> wheels = new List<GameObject>();
    TMP_Text infoRight;
    float planeZ = 0f;
    WheelAudioMap audioMap;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        audioMap = GetComponent<WheelAudioMap>();
        LoadLibrary();
        CreateCanvas();
        CreatePanels();
        CreatePalette();
        CreateCenterWorld();
        AudioHub.Ensure();
    }

    void LoadLibrary()
    {
        if (!Data.lib.bodyCarSprite) Data.lib.bodyCarSprite = Resources.Load<Sprite>("Sprites/body_car");
        if (!Data.lib.bodyBikeSprite) Data.lib.bodyBikeSprite = Resources.Load<Sprite>("Sprites/body_bike");
        if (Data.lib.wheelItems == null) Data.lib.wheelItems = new List<WheelItemDef>();
        if (Data.lib.wheelItems.Count == 0)
        {
            var sprites = Resources.LoadAll<Sprite>("Sprites/Wheels");
            foreach (var sp in sprites)
            {
                var n = sp.name.ToLower();
                var mode = WheelColliderMode.Polygon;
                if (n.Contains("circle")) mode = WheelColliderMode.Circle;
                else if (n.Contains("box")) mode = WheelColliderMode.Box;
                AudioClip sel = null; AudioClip run = null;
                if (audioMap) audioMap.TryGetClips(sp.name, out sel, out run);
                WheelItemDef d = new WheelItemDef
                {
                    sprite = sp,
                    colliderMode = mode,
                    scale = 1f,
                    baseFriction = 0.9f,
                    isDrive = true,
                    targetAngularSpeed = -12f,
                    maxTorque = 1400f,
                    grip = 14f,
                    angularDrag = 0.05f,
                    selectClip = sel,
                    runClip = run
                };
                Data.lib.wheelItems.Add(d);
            }
        }
        else if (audioMap)
        {
            for (int i = 0; i < Data.lib.wheelItems.Count; i++)
            {
                var wi = Data.lib.wheelItems[i];
                if (wi.sprite && audioMap.TryGetClips(wi.sprite.name, out var sel, out var run))
                {
                    wi.selectClip = sel;
                    wi.runClip = run;
                    Data.lib.wheelItems[i] = wi;
                }
            }
        }
    }

    void CreateCanvas()
    {
        var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        if (!FindFirstObjectByType<EventSystem>()) new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        var c = go.GetComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
        var s = go.GetComponent<CanvasScaler>(); s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; s.referenceResolution = new Vector2(1920, 1080); s.matchWidthOrHeight = 0.5f;
        canvasRT = go.GetComponent<RectTransform>(); canvasRT.anchorMin = Vector2.zero; canvasRT.anchorMax = Vector2.one; canvasRT.offsetMin = Vector2.zero; canvasRT.offsetMax = Vector2.zero;
    }

    void ApplyTextStyle(TMP_Text t, int size, TextAlignmentOptions align)
    {
        t.font = boldFont ? boldFont : t.font;
        t.fontSize = size;
        t.enableAutoSizing = true;
        t.color = Color.black;
        t.alignment = align;
    }

    void CreatePanels()
    {
        float lMax = Mathf.Clamp01(leftWidth);
        float rMin = Mathf.Clamp01(1f - rightWidth);
        float cMin = lMax;
        float cMax = rMin;
        float n = Mathf.Clamp(centerNudgeX, -0.2f, 0.2f);
        float span = cMax - cMin;
        cMin += n * span;
        cMax += n * span;
        cMin = Mathf.Clamp01(cMin);
        cMax = Mathf.Clamp01(cMax);

        leftPanel = CreatePanel(canvasRT, new Vector2(0f, 0f), new Vector2(lMax, 1f), new Color(0, 0, 0, 0.06f));
        rightPanel = CreatePanel(canvasRT, new Vector2(rMin, 0f), new Vector2(1f, 1f), new Color(0, 0, 0, 0f));
        centerArea = CreatePanel(canvasRT, new Vector2(cMin, 0f), new Vector2(cMax, 1f), new Color(0, 0, 0, 0f));

        infoRight = CreateText(rightPanel, "Drag a frame into center, then drag wheels.\nRight click to delete.", 28, new Vector2(20, -20), new Vector2(-20, -180), TextAlignmentOptions.TopLeft);
        var startBtn = CreateButton(rightPanel, "START", new Vector2(0.15f, 140), new Vector2(0.85f, 220));
        startBtn.onClick.AddListener(OnStartRun);
        var resetBtn = CreateButton(rightPanel, "RESET", new Vector2(0.15f, 40), new Vector2(0.85f, 120));
        resetBtn.onClick.AddListener(ResetAssembly);
    }

    RectTransform CreatePanel(RectTransform parent, Vector2 min, Vector2 max, Color bg)
    {
        var go = new GameObject("Panel", typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>(); rt.anchorMin = min; rt.anchorMax = max; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = bg;
        return rt;
    }

    TMP_Text CreateText(RectTransform parent, string txt, int size, Vector2 min, Vector2 max, TextAlignmentOptions align)
    {
        var go = new GameObject("Text", typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<TextMeshProUGUI>(); t.text = txt;
        ApplyTextStyle(t, size, align);
        var rt = t.GetComponent<RectTransform>(); rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1); rt.pivot = new Vector2(0.5f, 1f); rt.offsetMin = min; rt.offsetMax = max;
        return t;
    }

    Button CreateButton(RectTransform parent, string label, Vector2 min, Vector2 max)
    {
        var go = new GameObject(label, typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>(); rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(1, 0); rt.pivot = new Vector2(0.5f, 0.5f); rt.offsetMin = min; rt.offsetMax = max;
        go.GetComponent<Image>().color = new Color(1, 1, 1, 0.22f);
        var btn = go.GetComponent<Button>();
        var txtObj = new GameObject("Label", typeof(TextMeshProUGUI));
        txtObj.transform.SetParent(go.transform, false);
        var t = txtObj.GetComponent<TextMeshProUGUI>(); t.text = label;
        ApplyTextStyle(t, 38, TextAlignmentOptions.Center);
        var trt = t.GetComponent<RectTransform>(); trt.anchorMin = new Vector2(0, 0); trt.anchorMax = new Vector2(1, 1); trt.pivot = new Vector2(0.5f, 0.5f); trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
        return btn;
    }

    void CreatePalette()
    {
        var scroll = new GameObject("ScrollView", typeof(Image), typeof(Mask), typeof(ScrollRect));
        scroll.transform.SetParent(leftPanel, false);
        var srt = scroll.GetComponent<RectTransform>(); srt.anchorMin = new Vector2(0, 0); srt.anchorMax = new Vector2(1, 1); srt.offsetMin = new Vector2(10, 10); srt.offsetMax = new Vector2(-10, -20);
        scroll.GetComponent<Image>().color = new Color(1, 1, 1, 0.06f);
        scroll.GetComponent<Mask>().showMaskGraphic = true;

        var viewport = new GameObject("Viewport", typeof(RectTransform));
        viewport.transform.SetParent(scroll.transform, false);
        var vp = viewport.GetComponent<RectTransform>(); vp.anchorMin = Vector2.zero; vp.anchorMax = Vector2.one; vp.offsetMin = Vector2.zero; vp.offsetMax = Vector2.zero;

        var content = new GameObject("Content", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        var crt = content.GetComponent<RectTransform>(); crt.anchorMin = new Vector2(0, 1); crt.anchorMax = new Vector2(1, 1); crt.pivot = new Vector2(0.5f, 1f); crt.offsetMin = new Vector2(8, 0); crt.offsetMax = new Vector2(-8, 0);

        var gl = content.GetComponent<GridLayoutGroup>();
        gl.cellSize = paletteCell; gl.spacing = paletteSpacing; gl.childAlignment = TextAnchor.UpperLeft; gl.constraint = GridLayoutGroup.Constraint.FixedColumnCount; gl.constraintCount = paletteColumns;
        var fitter = content.GetComponent<ContentSizeFitter>(); fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var sr = scroll.GetComponent<ScrollRect>(); sr.viewport = vp; sr.content = crt; sr.horizontal = false; sr.vertical = true; sr.movementType = ScrollRect.MovementType.Clamped;

        scrollViewport = vp; scrollContent = crt;

        CreateFrameItem(scrollContent, "Car Frame", Data.lib.bodyCarSprite, FrameType.Car);
        CreateFrameItem(scrollContent, "Bike Frame", Data.lib.bodyBikeSprite, FrameType.Bike);

        int added = 0;
        for (int i = 0; i < Data.lib.wheelItems.Count; i++)
        {
            var wi = Data.lib.wheelItems[i];
            if (wi.sprite)
            {
                CreateWheelItem(scrollContent, "Wheel " + (i + 1).ToString(), wi.sprite, i);
                added++;
            }
        }
        if (added == 0)
        {
            var tip = new GameObject("Tip", typeof(TextMeshProUGUI));
            tip.transform.SetParent(scrollContent, false);
            var t = tip.GetComponent<TextMeshProUGUI>(); t.text = "No wheels found in Resources/Sprites/Wheels";
            ApplyTextStyle(t, 28, TextAlignmentOptions.Left);
            var rt = t.GetComponent<RectTransform>(); rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1); rt.pivot = new Vector2(0.5f, 1f); rt.offsetMin = new Vector2(8, -40); rt.offsetMax = new Vector2(-8, -8);
        }
    }

    void CreateFrameItem(Transform parent, string label, Sprite sp, FrameType ft)
    {
        var go = new GameObject(label, typeof(Image), typeof(LayoutElement), typeof(PaletteItemDrag));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>(); img.sprite = sp; img.preserveAspect = true; img.color = Color.white;
        var le = go.GetComponent<LayoutElement>(); le.preferredWidth = paletteCell.x; le.preferredHeight = paletteCell.y;
        var drag = go.GetComponent<PaletteItemDrag>(); drag.ui = this; drag.kind = PaletteKind.Frame; drag.frameType = ft;
        var txt = new GameObject("Label", typeof(TextMeshProUGUI)); txt.transform.SetParent(go.transform, false);
        var t = txt.GetComponent<TextMeshProUGUI>(); t.text = label; ApplyTextStyle(t, 28, TextAlignmentOptions.Center);
        var rt = t.GetComponent<RectTransform>(); rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.pivot = new Vector2(0.5f, 0.5f); rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    void CreateWheelItem(Transform parent, string label, Sprite sp, int index)
    {
        var go = new GameObject(label, typeof(Image), typeof(LayoutElement), typeof(PaletteItemDrag));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>(); img.sprite = sp; img.preserveAspect = true; img.color = Color.white;
        var le = go.GetComponent<LayoutElement>(); le.preferredWidth = paletteCell.x; le.preferredHeight = paletteCell.y;
        var drag = go.GetComponent<PaletteItemDrag>(); drag.ui = this; drag.kind = PaletteKind.Wheel; drag.wheelItemIndex = index;
        var txt = new GameObject("Label", typeof(TextMeshProUGUI)); txt.transform.SetParent(go.transform, false);
        var t = txt.GetComponent<TextMeshProUGUI>(); t.text = label; ApplyTextStyle(t, 28, TextAlignmentOptions.Center);
        var rt = t.GetComponent<RectTransform>(); rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.pivot = new Vector2(0.5f, 0.5f); rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    float CamDistToPlane() { return Mathf.Abs(cam.transform.position.z - planeZ); }

    void UpdateBuildBoundsFromCenterArea()
    {
        var rect = RectTransformUtility.PixelAdjustRect(centerArea, centerArea.GetComponentInParent<Canvas>());
        float d = CamDistToPlane();
        var bl = cam.ScreenToWorldPoint(new Vector3(rect.x, rect.y, d));
        var tr = cam.ScreenToWorldPoint(new Vector3(rect.x + rect.width, rect.y + rect.height, d));
        Vector2 size = tr - bl;
        buildBounds.transform.position = new Vector3(0f, 0f, planeZ);
        buildBounds.size = size;
    }

    void CreateCenterWorld()
    {
        var go = new GameObject("BuildBounds");
        go.AddComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        buildBounds = go.AddComponent<BoxCollider2D>();
        UpdateBuildBoundsFromCenterArea();
    }

    public bool IsInBuildAreaScreen(Vector2 screenPos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(centerArea, screenPos);
    }

    Vector3 ClampToBounds(Vector3 worldPos)
    {
        var b = buildBounds.bounds;
        worldPos.x = Mathf.Clamp(worldPos.x, b.min.x, b.max.x);
        worldPos.y = Mathf.Clamp(worldPos.y, b.min.y, b.max.y);
        worldPos.z = 0f;
        return worldPos;
    }

    void LateUpdate()
    {
        if (buildBounds == null) return;
        UpdateBuildBoundsFromCenterArea();
    }

    public float GetWheelWorldScale(int wheelIndex)
    {
        var item = Data.lib.wheelItems[wheelIndex];
        var sp = item.sprite;
        float dia1 = Mathf.Max(sp.bounds.size.x, sp.bounds.size.y);
        if (dia1 <= 0.0001f) return 1f;
        float target = Mathf.Max(0.01f, wheelTargetDiameter);
        float s = target / dia1;
        s *= Mathf.Max(0.0001f, item.scale);
        s = Mathf.Clamp(s, wheelMinScale, wheelMaxScale);
        return s;
    }

    public void PlaceBody(Vector3 worldPos, FrameType ft)
    {
        worldPos = ClampToBounds(worldPos);
        if (currentBody) { Destroy(currentBody); foreach (var w in wheels) if (w) Destroy(w); wheels.Clear(); }
        var sp = ft == FrameType.Car ? Data.lib.bodyCarSprite : Data.lib.bodyBikeSprite;
        currentBody = new GameObject("Body", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(PlacedDrag));
        var sr = currentBody.GetComponent<SpriteRenderer>(); sr.sprite = sp; sr.sortingOrder = 10;
        currentBody.transform.position = worldPos;
        var bc = currentBody.GetComponent<BoxCollider2D>(); var b = sr.bounds; bc.size = b.size; bc.isTrigger = true;
        var pd = currentBody.GetComponent<PlacedDrag>(); pd.cam = cam; pd.bounds = buildBounds; pd.onDelete = null;
        currentFrame = ft;
    }

    public void PlaceWheel(Vector3 worldPos, int wheelIndex)
    {
        if (!currentBody) return;
        worldPos = ClampToBounds(worldPos);
        var item = Data.lib.wheelItems[wheelIndex];
        var go = new GameObject("Wheel", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(PlacedDrag), typeof(WheelMeta));
        var sr = go.GetComponent<SpriteRenderer>(); sr.sprite = item.sprite; sr.sortingOrder = 11;
        float s = GetWheelWorldScale(wheelIndex);
        go.transform.localScale = Vector3.one * s;
        go.transform.position = worldPos;
        var bc = go.GetComponent<BoxCollider2D>(); var b = sr.bounds; bc.size = b.size; bc.isTrigger = true;
        go.GetComponent<WheelMeta>().itemIndex = wheelIndex;
        go.transform.SetParent(currentBody.transform, true);
        var pd = go.GetComponent<PlacedDrag>(); pd.cam = cam; pd.bounds = buildBounds; pd.onDelete = () => { wheels.Remove(go); Destroy(go); };
        wheels.Add(go);
        if (item.selectClip) AudioHub.Play2D(item.selectClip, 1f);
    }

    void ResetAssembly()
    {
        if (currentBody) Destroy(currentBody);
        foreach (var w in wheels) if (w) Destroy(w);
        wheels.Clear();
        Data.ResetVehicle();
        currentFrame = FrameType.Car;
    }

    void OnStartRun()
    {
        if (!currentBody) return;
        var v = Data.vehicle;
        v.wheels = new List<WheelDef>();
        v.frameType = currentFrame;
        v.bodyMass = currentFrame == FrameType.Car ? 6f : 4f;
        v.centerOfMassOffset = new Vector2(0, -0.25f);
        foreach (var w in wheels)
        {
            if (!w) continue;
            Vector2 local = currentBody.transform.InverseTransformPoint(w.transform.position);
            int idx = w.GetComponent<WheelMeta>().itemIndex;
            float sc = w.transform.lossyScale.x;
            v.wheels.Add(new WheelDef { localPos = local, itemIndex = idx, scale = sc });
        }
        Data.vehicle = v;
        SceneManager.LoadScene(runSceneName);
    }
}

public enum PaletteKind { Frame, Wheel }
public class WheelMeta : MonoBehaviour { public int itemIndex; }
