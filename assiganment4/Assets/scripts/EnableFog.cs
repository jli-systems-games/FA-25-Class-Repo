using UnityEngine;
using UnityEngine.Rendering;

public class EnableFog : MonoBehaviour
{
    [Header("Color")]
    public Color fogColor = new Color32(0x16, 0x3A, 0x4F, 255); // #163A4F

    [Header("Mode")]
    public FogMode mode = FogMode.Exponential; // 也可 FogMode.Linear

    [Header("Exponential")]
    public float density = 0.04f;

    [Header("Linear")]
    public float startDistance = 20f;
    public float endDistance = 80f;

    void OnEnable()
    {
        // 如果仍是内置渲染管线（currentRenderPipeline == null），启用 RenderSettings 的雾
        if (GraphicsSettings.currentRenderPipeline == null)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogMode = mode;

            if (mode == FogMode.Exponential)
                RenderSettings.fogDensity = density;
            else
            {
                RenderSettings.fogStartDistance = startDistance;
                RenderSettings.fogEndDistance = endDistance;
            }
        }
        else
        {
            Debug.LogWarning("当前不是内置渲染管线。RenderSettings.fog 将被忽略，需要使用 SRP 的 Volume Fog。");
        }
    }
}
