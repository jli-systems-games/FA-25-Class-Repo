using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AutoHUDTopRight : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(1920, 1080);
    public int fontSize = 26;
    public float lineSpacing = -6f;
    public Vector2 margin = new Vector2(24, 24);

    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (!canvas)
        {
            GameObject cObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = cObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = cObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.matchWidthOrHeight = 0.5f;
        }

        if (!FindObjectOfType<EventSystem>())
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        GameObject panel = new GameObject("HUD_TopRight");
        panel.transform.SetParent(canvas.transform, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-margin.x, -margin.y);
        rt.sizeDelta = new Vector2(480, 200);

        var layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperRight;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.spacing = 2;

        var fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        MakeLine(panel.transform, "Press R to Restart");
        MakeLine(panel.transform, "Q, W, E, R to move");
        MakeLine(panel.transform, "Be Chill Here");
    }

    void MakeLine(Transform parent, string text)
    {
        GameObject go = new GameObject(text, typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.enableWordWrapping = false;
        tmp.alignment = TextAlignmentOptions.TopRight;
        tmp.raycastTarget = false;
        tmp.color = new Color(1, 1, 1, 0.92f);
        tmp.lineSpacing = lineSpacing;
        tmp.margin = new Vector4(6, 0, 0, 0);
        var rect = tmp.rectTransform;
        rect.sizeDelta = new Vector2(480, 40);
    }
}
