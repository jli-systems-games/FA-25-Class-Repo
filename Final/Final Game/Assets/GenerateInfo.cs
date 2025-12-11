using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class InfoScreenGenerator : MonoBehaviour
{
    [Header("返回场景")]
    public string ReturnSceneName = "StartScreen";

    private GameObject canvas;

    [ContextMenu("生成Info界面")]
    public void GenerateUI()
    {
        // 清理
        var old = GameObject.Find("InfoCanvas");
        if (old != null)
        {
            if (Application.isPlaying) Destroy(old);
            else DestroyImmediate(old);
        }

        // EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // Canvas
        canvas = new GameObject("InfoCanvas");
        var c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;

        var cs = canvas.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);

        canvas.AddComponent<GraphicRaycaster>();

        // 橙色背景
        var bg = new GameObject("BG");
        bg.transform.SetParent(canvas.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(1f, 0.5f, 0f);
        var bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;

        // 返回按钮管理器
        canvas.AddComponent<BackButtonManager>();
        var manager = canvas.GetComponent<BackButtonManager>();
        manager.returnSceneName = ReturnSceneName;

        // 返回按钮
        var backBtn = MakeBackButton();
        manager.backButton = backBtn;

        // 内容
        MakeContent();

        Debug.Log("✅ Info界面生成完毕！");
    }

    Button MakeBackButton()
    {
        var obj = new GameObject("BackBtn");
        obj.transform.SetParent(canvas.transform, false);

        var rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(30, -30);
        rt.sizeDelta = new Vector2(120, 120);

        var img = obj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        var btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;

        // X 符号
        var txtObj = new GameObject("Text");
        txtObj.transform.SetParent(obj.transform, false);
        var txtRt = txtObj.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;

        var txt = txtObj.AddComponent<Text>();
        txt.text = "✖";
        txt.fontSize = 80;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.font = font;

        return btn;
    }

    void MakeContent()
    {
        var content = new GameObject("Content");
        content.transform.SetParent(canvas.transform, false);
        var crt = content.AddComponent<RectTransform>();
        crt.anchorMin = new Vector2(0.5f, 0.5f);
        crt.anchorMax = new Vector2(0.5f, 0.5f);
        crt.sizeDelta = new Vector2(1600, 900);

        // 标题
        MakeText(content.transform, "HOW TO PLAY",
            new Vector2(0, 380), new Vector2(1600, 100), 70);

        // 规则
        string rules =
            "★ OBJECTIVE ★\n" +
            "Collect stones & build your wall. Highest wall wins!\n\n" +
            "★ COMBAT TIPS ★\n" +
            "• Use Weapons to debuff enemies\n" +
            "• Throw Stones to FREEZE enemies\n" +
            "• Steal from enemy walls!";
        MakeText(content.transform, rules,
            new Vector2(0, 150), new Vector2(1400, 300), 36);

        // 蓝队
        string blue =
            "TEAM BLUE\n\n" +
            "P1 - Keyboard: WASD • F • E\n" +
            "P3 - Gamepad 1: Stick • □ • ×";
        MakeText(content.transform, blue,
            new Vector2(-400, -200), new Vector2(700, 400), 30);

        // 红队
        string red =
            "TEAM RED\n\n" +
            "P2 - Numpad: 1235 • 6 • Enter\n" +
            "P4 - Gamepad 2: Stick • □ • ×";
        MakeText(content.transform, red,
            new Vector2(400, -200), new Vector2(700, 400), 30);
    }

    void MakeText(Transform parent, string content, Vector2 pos, Vector2 size, int fontSize)
    {
        var obj = new GameObject("Text");
        obj.transform.SetParent(parent, false);

        var rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        var txt = obj.AddComponent<Text>();
        txt.text = content;
        txt.fontSize = fontSize;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.font = font;
    }
}

// 👇 返回按钮管理器
public class BackButtonManager : MonoBehaviour
{
    public Button backButton;
    public string returnSceneName;

    void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClick);
            Debug.Log("✅ 返回按钮已绑定");
        }
    }

    void OnBackClick()
    {
        Debug.Log("🔙 返回: " + returnSceneName);
        SceneManager.LoadScene(returnSceneName);
    }
}