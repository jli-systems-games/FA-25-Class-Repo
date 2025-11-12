using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhotoFramingFX : MonoBehaviour
{
    [Header("Zoom")]
    public float inFov = 35f;        // 取景时的FOV
    public float inDuration = 2.0f;  // 慢慢拉近 2s
    public float outDuration = 0.2f; // 回弹

    [Header("HUD Fade")]
    public CanvasGroup[] fadeGroups; // 需要淡出的UI(例如 Hint 的 CanvasGroup)
    public MaskableGraphic frameGraphic;        // 取景框（RawImage 或 Image，平时隐藏）

    Camera cam; float baseFov;

    void Awake()
    {
        cam = GetComponent<Camera>();
        baseFov = cam.fieldOfView;
        if (frameGraphic) frameGraphic.enabled = false;
    }

    public IEnumerator ZoomIn()
    {
        foreach (var g in fadeGroups) if (g) StartCoroutine(Fade(g, g.alpha, 0f, 0.15f));
        if (frameGraphic) frameGraphic.enabled = true;
        yield return StartCoroutine(FovTo(inFov, inDuration));
    }

    public IEnumerator ZoomOut()
    {
        if (frameGraphic) frameGraphic.enabled = false;
        yield return StartCoroutine(FovTo(baseFov, outDuration));
        foreach (var g in fadeGroups) if (g) StartCoroutine(Fade(g, g.alpha, 1f, 0.15f));
    }

    IEnumerator FovTo(float target, float dur)
    {
        float t = 0f, start = cam.fieldOfView;
        while (t < dur)
        {
            t += Time.deltaTime;
            cam.fieldOfView = Mathf.Lerp(start, target, t / dur);
            yield return null;
        }
        cam.fieldOfView = target;
    }

    IEnumerator Fade(CanvasGroup g, float a, float b, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            g.alpha = Mathf.Lerp(a, b, t / dur);
            yield return null;
        }
        g.alpha = b;
    }
}
