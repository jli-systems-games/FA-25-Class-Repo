using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneReloadFader : MonoBehaviour
{
    Canvas canvas;
    Image overlay;

    public static void Reload(float delay, float fadeInDuration)
    {
        var go = new GameObject("SceneReloadFader");
        DontDestroyOnLoad(go);
        var f = go.AddComponent<SceneReloadFader>();
        f.StartCoroutine(f.CoReload(delay, Mathf.Max(0.01f, fadeInDuration)));
    }

    void BuildOverlay()
    {
        canvas = new GameObject("Fader_Canvas").AddComponent<Canvas>();
        DontDestroyOnLoad(canvas.gameObject);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;
        var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 1;
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        var go = new GameObject("Overlay");
        overlay = go.AddComponent<Image>();
        overlay.raycastTarget = true;
        overlay.color = new Color(0, 0, 0, 1);
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(canvas.transform, false);
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    IEnumerator CoReload(float delay, float fadeIn)
    {
        BuildOverlay();
        float t = 0f;
        while (t < delay)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SetAlpha(1f);
        t = 0f;
        while (t < fadeIn)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(1f - t / fadeIn);
            yield return null;
        }
        SetAlpha(0f);
        Destroy(canvas.gameObject);
        Destroy(gameObject);
    }

    void SetAlpha(float a)
    {
        if (!overlay) return;
        var c = overlay.color; c.a = Mathf.Clamp01(a); overlay.color = c;
    }
}
