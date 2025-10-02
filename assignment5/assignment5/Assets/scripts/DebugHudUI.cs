using UnityEngine;
using UnityEngine.UI;

public class DebugHudUI : MonoBehaviour
{
    [Header("开关")]
    public bool show = true;
    public KeyCode toggleKey = KeyCode.F1;

    Canvas _canvas;
    Text _text;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 如果没有 Canvas，就动态创建一个覆盖层
        _canvas = new GameObject("~DebugHUD_Canvas").AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 9999; // 永远在最上层
        _canvas.gameObject.AddComponent<CanvasScaler>();
        _canvas.gameObject.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(_canvas.gameObject);

        var go = new GameObject("Text");
        go.transform.SetParent(_canvas.transform, false);
        _text = go.AddComponent<Text>();
        _text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        _text.fontSize = 16;
        _text.alignment = TextAnchor.UpperLeft;
        _text.color = Color.white;

        var rt = _text.rectTransform;
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(10, -10);
        rt.sizeDelta = new Vector2(620, 260);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) show = !show;
        if (!_text) return;

        _canvas.enabled = show;

        // 读取 GameFlow/NoiseSystem 的实时状态
        var gf = GameFlow.I;
        var ns = NoiseSystem.I;

        string win = gf && gf.winUI ? gf.winUI.alpha.ToString("F2") : "null";
        string over = gf && gf.gameOverUI ? gf.gameOverUI.alpha.ToString("F2") : "null";
        string noise = ns ? ns.noiseLevel.ToString("F2") : "-";

        _text.text =
            "[HUD] Build-safe overlay\n" +
            "GF Alive: " + (gf ? "true" : "false") + "\n" +
            "Ended: " + (gf ? gf.GetType().GetField("ended", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(gf) : "N/A") + "    " +
            "TimeScale: " + Time.timeScale.ToString("F2") + "\n" +
            "WinUI alpha: " + win + "   OverUI alpha: " + over + "\n" +
            "Noise: " + noise + "\n" +
            "Hotkeys: K=GameOver, L=Win, R=Reload, F1=Toggle HUD";
    }
}
