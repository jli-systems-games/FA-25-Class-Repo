using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class BuildTicketCheckLayout : Editor
{
    [MenuItem("Tools/UI/Build TicketCheck Layout")]
    public static void Build()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (!canvas)
        {
            var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }
        if (!Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>())
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        GameObject gameRoot = GameObject.Find("GameRoot");
        if (!gameRoot) gameRoot = new GameObject("GameRoot");
        var gm = gameRoot.GetComponent<GameManager>();
        if (!gm) gm = gameRoot.AddComponent<GameManager>();

        GameObject EnsureGO(string name, Transform parent, params System.Type[] comps)
        {
            var t = parent ? parent.Find(name) : null;
            GameObject go = t ? t.gameObject : new GameObject(name, comps);
            if (!t) go.transform.SetParent(parent, false);
            return go;
        }
        RectTransform RT(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            if (!rt) rt = go.AddComponent<RectTransform>();
            return rt;
        }
        TMP_Text TMP(GameObject go, string text, int size, TextAlignmentOptions align)
        {
            var tmp = go.GetComponent<TMP_Text>();
            if (!tmp) tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = align;
            tmp.enableAutoSizing = false;
            return tmp;
        }
        Image IMG(GameObject go)
        {
            var img = go.GetComponent<Image>();
            if (!img) img = go.AddComponent<Image>();
            return img;
        }
        Button BTN(GameObject go)
        {
            var b = go.GetComponent<Button>();
            if (!b) b = go.AddComponent<Button>();
            if (!go.GetComponent<Image>()) go.AddComponent<Image>();
            return b;
        }
        void SetRect(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 anchoredPos)
        {
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size; rt.anchoredPosition = anchoredPos;
        }
        GameObject textWithParent(string name, Transform parent, string text, int size, TextAlignmentOptions align)
        {
            var go = EnsureGO(name, parent);
            TMP(go, text, size, align);
            return go;
        }
        GameObject buttonWithText(string name, Transform parent, string label)
        {
            var go = EnsureGO(name, parent);
            BTN(go);
            var txtGo = EnsureGO("Text", go.transform);
            TMP(txtGo, label, 28, TextAlignmentOptions.Center);
            var txtRT = RT(txtGo);
            txtRT.anchorMin = Vector2.zero; txtRT.anchorMax = Vector2.one; txtRT.offsetMin = Vector2.zero; txtRT.offsetMax = Vector2.zero;
            return go;
        }

        var TopBar = EnsureGO("TopBar", canvas.transform, typeof(Image));
        SetRect(RT(TopBar), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 120), new Vector2(0, -60));
        IMG(TopBar).color = new Color32(30, 43, 58, 180);

        var TimerGroup = EnsureGO("TimerGroup", TopBar.transform);
        SetRect(RT(TimerGroup), new Vector2(0, 1), new Vector2(0, 1), new Vector2(200, 80), new Vector2(120, -40));
        var TimerText = textWithParent("TimerText", TimerGroup.transform, "5.0", 36, TextAlignmentOptions.MidlineLeft);
        RT(TimerText).sizeDelta = new Vector2(200, 80);

        var Passenger = EnsureGO("Passenger", TopBar.transform);
        SetRect(RT(Passenger), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(96, 96), new Vector2(0, -60));
        var PassengerImage = IMG(Passenger);

        var ScoreGroup = EnsureGO("ScoreGroup", TopBar.transform);
        SetRect(RT(ScoreGroup), new Vector2(1, 1), new Vector2(1, 1), new Vector2(200, 80), new Vector2(-120, -40));
        var ScoreText = textWithParent("ScoreText", ScoreGroup.transform, "0", 36, TextAlignmentOptions.MidlineRight);
        RT(ScoreText).sizeDelta = new Vector2(200, 80);

        var DeskTop = EnsureGO("DeskTop", canvas.transform, typeof(Image));
        SetRect(RT(DeskTop), new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 420), new Vector2(0, 210));
        IMG(DeskTop).color = new Color32(58, 79, 106, 220);

        var DividerTop = EnsureGO("DividerTop", DeskTop.transform, typeof(Image));
        SetRect(RT(DividerTop), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 2), new Vector2(0, -1));
        IMG(DividerTop).color = new Color32(0, 0, 0, 90);

        var BottomPanel = EnsureGO("BottomPanel", DeskTop.transform);
        var bprt = RT(BottomPanel);
        bprt.anchorMin = Vector2.zero; bprt.anchorMax = Vector2.one; bprt.offsetMin = Vector2.zero; bprt.offsetMax = Vector2.zero;

        var TicketPanel = EnsureGO("TicketPanel", BottomPanel.transform, typeof(Image));
        SetRect(RT(TicketPanel), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(700, 360), new Vector2(380, 0));
        IMG(TicketPanel).color = new Color32(245, 237, 226, 255);

        var TicketBorder = EnsureGO("TicketBorder", TicketPanel.transform, typeof(Image));
        SetRect(RT(TicketBorder), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(660, 320), new Vector2(0, 0));
        IMG(TicketBorder).color = new Color32(217, 205, 190, 255);

        var DateText = textWithParent("DateText", TicketBorder.transform, "Date: 2025-01-01", 36, TextAlignmentOptions.MidlineLeft);
        var StampText = textWithParent("StampText", TicketBorder.transform, "Stamp: Valid", 36, TextAlignmentOptions.MidlineLeft);
        var DestinationText = textWithParent("DestinationText", TicketBorder.transform, "Destination: Tokyo", 36, TextAlignmentOptions.MidlineLeft);
        SetRect(RT(DateText), new Vector2(0, 1), new Vector2(1, 1), new Vector2(-40, 60), new Vector2(20, -40));
        SetRect(RT(StampText), new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(-40, 60), new Vector2(20, 0));
        SetRect(RT(DestinationText), new Vector2(0, 0), new Vector2(1, 0), new Vector2(-40, 60), new Vector2(20, 40));

        var ActionPanel = EnsureGO("ActionPanel", BottomPanel.transform, typeof(Image));
        SetRect(RT(ActionPanel), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(600, 360), new Vector2(-380, 0));
        IMG(ActionPanel).color = new Color32(231, 231, 231, 60);

        var Row1 = EnsureGO("Row1", ActionPanel.transform);
        SetRect(RT(Row1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(560, 80), new Vector2(0, -60));

        var DenyButton = buttonWithText("DenyButton", Row1.transform, "DENY");
        SetRect(RT(DenyButton), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(240, 64), new Vector2(120, 0));
        DenyButton.GetComponent<Image>().color = new Color32(185, 65, 65, 255);

        var ApproveButton = buttonWithText("ApproveButton", Row1.transform, "APPROVE");
        SetRect(RT(ApproveButton), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(240, 64), new Vector2(-120, 0));
        ApproveButton.GetComponent<Image>().color = new Color32(62, 142, 90, 255);

        var Row2 = EnsureGO("Row2", ActionPanel.transform);
        SetRect(RT(Row2), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(560, 80), new Vector2(0, 60));

        var SendToText = textWithParent("SendToText", Row2.transform, "A / B / C", 28, TextAlignmentOptions.Center);
        SetRect(RT(SendToText), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(200, 40), new Vector2(0, -40));

        var PlatformA = buttonWithText("PlatformA", Row2.transform, "A");
        var PlatformB = buttonWithText("PlatformB", Row2.transform, "B");
        var PlatformC = buttonWithText("PlatformC", Row2.transform, "C");
        SetRect(RT(PlatformA), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(160, 56), new Vector2(100, 0));
        SetRect(RT(PlatformB), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(160, 56), new Vector2(0, 0));
        SetRect(RT(PlatformC), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(160, 56), new Vector2(-100, 0));
        PlatformA.GetComponent<Image>().color = new Color32(217, 217, 217, 255);
        PlatformB.GetComponent<Image>().color = new Color32(217, 217, 217, 255);
        PlatformC.GetComponent<Image>().color = new Color32(217, 217, 217, 255);

        var HintText = textWithParent("HintText", canvas.transform, "Decide within 5 seconds", 28, TextAlignmentOptions.Center);
        SetRect(RT(HintText), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(800, 40), new Vector2(0, 140));

        var PlatformMapText = textWithParent("Map", canvas.transform, "PLATFORM A ¡ª TOKYO\nPLATFORM B ¡ª BERLIN\nPLATFORM C ¡ª PARIS", 28, TextAlignmentOptions.TopLeft);
        SetRect(RT(PlatformMapText), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(520, 96), new Vector2(0, 220));

        var ReferencePanel = EnsureGO("ReferencePanel", canvas.transform, typeof(Image));
        SetRect(RT(ReferencePanel), new Vector2(1, 1), new Vector2(1, 1), new Vector2(360, 260), new Vector2(-240, -200));
        IMG(ReferencePanel).color = new Color32(200, 200, 200, 160);
        var Today = textWithParent("Today", ReferencePanel.transform, "Today: 2025-01-01", 24, TextAlignmentOptions.TopLeft);
        var ValidDest = textWithParent("ValidDest", ReferencePanel.transform, "- Tokyo\n- Berlin\n- Paris", 20, TextAlignmentOptions.TopLeft);
        var Stamp = textWithParent("Stamp", ReferencePanel.transform, "Stamp Rule: must be \"Valid\"", 20, TextAlignmentOptions.TopLeft);
        SetRect(RT(Today), new Vector2(0, 1), new Vector2(1, 1), new Vector2(-24, 40), new Vector2(12, -20));
        SetRect(RT(ValidDest), new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(-24, 120), new Vector2(12, 0));
        SetRect(RT(Stamp), new Vector2(0, 0), new Vector2(1, 0), new Vector2(-24, 60), new Vector2(12, 30));

        gm.passengerPortrait = PassengerImage;
        gm.timerText = TimerText.GetComponent<TMP_Text>();
        gm.scoreText = ScoreText.GetComponent<TMP_Text>();
        gm.destinationText = DestinationText.GetComponent<TMP_Text>();
        gm.dateText = DateText.GetComponent<TMP_Text>();
        gm.stampText = StampText.GetComponent<TMP_Text>();
        gm.approveButton = ApproveButton.GetComponent<Button>();
        gm.denyButton = DenyButton.GetComponent<Button>();
        gm.platformAButton = PlatformA.GetComponent<Button>();
        gm.platformBButton = PlatformB.GetComponent<Button>();
        gm.platformCButton = PlatformC.GetComponent<Button>();
        gm.platformRow = Row2;
        gm.hintText = HintText.GetComponent<TMP_Text>();
        gm.platformMapText = PlatformMapText.GetComponent<TMP_Text>();
        gm.referencePanel = ReferencePanel;
        gm.todayRefText = Today.GetComponent<TMP_Text>();
        gm.validDestText = ValidDest.GetComponent<TMP_Text>();
        gm.stampRuleText = Stamp.GetComponent<TMP_Text>();

        UnityEventTools.AddPersistentListener(DenyButton.GetComponent<Button>().onClick, gm.OnClickDeny);
        UnityEventTools.AddPersistentListener(ApproveButton.GetComponent<Button>().onClick, gm.OnClickApprove);
        UnityAction<string> a = gm.OnClickPlatform;
        UnityEventTools.AddStringPersistentListener(PlatformA.GetComponent<Button>().onClick, a, "A");
        UnityEventTools.AddStringPersistentListener(PlatformB.GetComponent<Button>().onClick, a, "B");
        UnityEventTools.AddStringPersistentListener(PlatformC.GetComponent<Button>().onClick, a, "C");

        Selection.activeGameObject = gameRoot;
        EditorGUIUtility.PingObject(gameRoot);
    }
}
