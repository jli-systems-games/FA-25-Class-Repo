using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GlitchEffect : MonoBehaviour
{
    public float intensity = 0.9f;
    public float blockSize = 0.08f;
    public float colorSplit = 2f;
    public float lines = 0.5f;

    Material mat;
    float timeLeft;

    void OnEnable()
    {
        var sh = Shader.Find("Hidden/GlitchSimple");
        if (sh != null) mat = new Material(sh);
    }

    void OnDisable()
    {
        if (mat != null) { DestroyImmediate(mat); mat = null; }
        timeLeft = 0f;
    }

    public void Trigger(float duration)
    {
        timeLeft = Mathf.Max(timeLeft, duration);
    }

    void Update()
    {
        if (timeLeft > 0f) timeLeft -= Time.unscaledDeltaTime;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (mat == null || timeLeft <= 0f)
        {
            Graphics.Blit(src, dst);
            return;
        }
        float t = Mathf.Clamp01(timeLeft);
        mat.SetFloat("_Intensity", intensity * t);
        mat.SetFloat("_BlockSize", blockSize);
        mat.SetFloat("_ColorSplit", colorSplit);
        mat.SetFloat("_Lines", lines);
        mat.SetFloat("_TimeSeed", Time.unscaledTime);
        Graphics.Blit(src, dst, mat);
    }
}
