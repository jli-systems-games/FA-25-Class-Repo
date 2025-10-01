using UnityEngine;
using UnityEngine.UI;

public class NoiseUIController : MonoBehaviour
{
    [Header("UI 引用")]
    public Image fill;  

    [Header("外观")]
    [Range(3, 30)] public int steps = 12; 
    [Range(0f, 1f)] public float warn = 0.7f;

    [Header("平滑/迟滞")]
    [Tooltip("数值平滑速度（越大越跟手，越小越稳）")]
    public float smoothSpeed = 6f;
    [Tooltip("升一级需要超过的额外比例（相对一个 step）")]
    public float stepUpBias = 0.20f;
    [Tooltip("降一级需要低于的额外比例（相对一个 step）")]
    public float stepDownBias = 0.60f;

    float smoothV = 0f;   
    float lastStep = 0f;  

    void Reset()
    {
        fill = GetComponent<Image>();
    }

    void Update()
    {
        if (!fill || !NoiseSystem.I) return;

        float raw = Mathf.Clamp01(NoiseSystem.I.noiseLevel);

        float t = 1f - Mathf.Exp(-smoothSpeed * Time.unscaledDeltaTime);
        smoothV = Mathf.Lerp(smoothV, raw, t);

        float stepSize = 1f / Mathf.Max(1, steps);
        float candidate = Mathf.Ceil(smoothV * steps) / steps;

        float upThresh = lastStep + stepSize * stepUpBias;
        float downThresh = lastStep - stepSize * stepDownBias;

        float stepped = lastStep;
        if (candidate > lastStep && smoothV >= upThresh) stepped = candidate;
        else if (candidate < lastStep && smoothV <= downThresh) stepped = candidate;

        lastStep = stepped;

        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.fillAmount = stepped;

        Color c = (stepped < warn)
            ? Color.Lerp(Color.white, Color.yellow, Mathf.InverseLerp(0f, warn, stepped))
            : Color.Lerp(Color.yellow, Color.red, Mathf.InverseLerp(warn, 1f, stepped));
        fill.color = c;
    }
}
