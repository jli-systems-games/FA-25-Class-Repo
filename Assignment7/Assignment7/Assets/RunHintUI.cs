using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunHintUI : MonoBehaviour
{
    public TMP_FontAsset font;
    public int fontSize = 48;
    public string hintText = "Hold SPACE to accelerate  |  R = Restart";

    RectTransform canvasRT;
    TextMeshProUGUI text;

    void Awake()
    {
        AudioHub.Ensure();
        var canvasGO = new GameObject("RunHUD_Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.anchorMin = Vector2.zero;
        canvasRT.anchorMax = Vector2.one;
        canvasRT.offsetMin = Vector2.zero;
        canvasRT.offsetMax = Vector2.zero;

        var txtGO = new GameObject("Hint", typeof(TextMeshProUGUI));
        txtGO.transform.SetParent(canvasRT, false);
        text = txtGO.GetComponent<TextMeshProUGUI>();
        text.text = hintText;
        text.font = font ? font : text.font;
        text.fontSize = fontSize;
        text.enableAutoSizing = true;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Top;
        var rt = text.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -20f);
        rt.sizeDelta = new Vector2(1400f, 120f);
    }
}
