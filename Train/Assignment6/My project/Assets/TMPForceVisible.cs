using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TMPForceVisible : MonoBehaviour
{
    public Camera targetCamera;
    public bool applyOnAwake = false;

    void Awake()
    {
        if (applyOnAwake) ForceFix();
    }

    public void ForceFix()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        var texts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (var t in texts)
        {
            var go = t.gameObject;
            go.SetActive(true);
            var p = go.transform;
            while (p != null)
            {
                var cg = p.GetComponent<CanvasGroup>();
                if (cg) cg.alpha = 1f;
                p = p.parent;
            }
            t.color = new Color(1, 1, 1, 1);
            t.enableVertexGradient = false;
            t.fontSize = Mathf.Max(24, t.fontSize);
            t.enableAutoSizing = false;
            t.overflowMode = TextOverflowModes.Overflow;

            var rt = t.rectTransform;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
            rt.anchoredPosition3D = new Vector3(0, 0, 0);
            if (rt.sizeDelta.magnitude < 10f) rt.sizeDelta = new Vector2(600, 200);

            var c = t.GetComponentInParent<Canvas>(true);
            if (c)
            {
                c.renderMode = RenderMode.ScreenSpaceCamera;
                c.worldCamera = targetCamera;
                var scaler = c.GetComponent<CanvasScaler>();
                if (!scaler) scaler = c.gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }
        }
    }
}
