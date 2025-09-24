using UnityEngine;

public class DayNightController : MonoBehaviour
{
    [Header("Sun")]
    public Light sun;                 // 拖 Directional Light 自己
    public float rotateSpeed = 2f;    // 自动缓慢转动（度/秒）

    [Header("W key nudge")]
    public float wKeyStep = 1.5f;     // 每次 W 轻推太阳多少度

    [Header("Ambient")]
    public Gradient ambientColor;     // 颜色随时间的渐变（0=午夜，1=正午）
    public AnimationCurve ambientIntensity = AnimationCurve.Linear(0, 0.2f, 1, 1f);

    float time01 = 0.5f;              // 0..1 昼夜时间归一化（0=午夜 0.5=日出/日落 1=正午）

    void Reset() { sun = GetComponent<Light>(); }

    void Update()
    {
        // 自动流转
        time01 = Mathf.Repeat(time01 + rotateSpeed / 360f * Time.deltaTime, 1f);

        // 按 W 键微推进（长按也会稳定推进）
        if (Input.GetKey(KeyCode.W)) time01 = Mathf.Repeat(time01 + (wKeyStep / 360f) * Time.deltaTime, 1f);

        // 驱动太阳角度（把 0..1 映射成 -90..+270）
        float angle = Mathf.Lerp(-90f, 270f, time01);
        if (sun) sun.transform.rotation = Quaternion.Euler(angle, 45f, 0f);

        // 环境光
        if (ambientColor != null)
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = ambientColor.Evaluate(time01);
            RenderSettings.ambientIntensity = ambientIntensity.Evaluate(time01);
        }
    }
}
