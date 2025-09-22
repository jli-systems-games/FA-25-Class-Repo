using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class UnderwaterFogEffect : MonoBehaviour
{
    [Header("Assign Fog Material (M_UnderwaterFogBlit)")]
    public Material fogMaterial;

    [Header("Underwater Fog Params")]
    public Color fogColor = new Color32(0x16, 0x3A, 0x4F, 255); 
    [Range(0.0f, 1.0f)] public float fogDensity = 0.04f;
    public bool useLinear = false; 
    public float linearStart = 20f;
    public float linearEnd = 80f;

    [Header("Debug")]
    public bool showDepth = false;

    Camera cam;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode |= DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (fogMaterial == null)
        {
            Graphics.Blit(src, dst);
            return;
        }

        fogMaterial.SetColor("_FogColor", fogColor);
        fogMaterial.SetFloat("_FogDensity", fogDensity);
        fogMaterial.SetFloat("_LinearStart", linearStart);
        fogMaterial.SetFloat("_LinearEnd", linearEnd);
        fogMaterial.SetInt("_UseLinear", useLinear ? 1 : 0);
        fogMaterial.SetInt("_ShowDepth", showDepth ? 1 : 0);

        Graphics.Blit(src, dst, fogMaterial, 0);
    }
}
