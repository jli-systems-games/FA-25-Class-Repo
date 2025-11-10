using UnityEngine;

public class FogController : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;                // Toggle fog on/off
    public FogMode fogMode = FogMode.Exponential;// Linear or Exponential
    [ColorUsage(true, true)]
    public Color fogColor = new Color(0.3f, 0.25f, 0.4f); // Soft purple-gray
    [Range(0.001f, 0.1f)]
    public float fogDensity = 0.02f;             // Exponential mode only
    public float linearStart = 30f;              // Linear start distance
    public float linearEnd = 120f;               // Linear end distance

    [Header("Dynamic Fade Settings (optional)")]
    public bool fadeFog = false;                 // Enable gradual fade
    public float fadeSpeed = 0.5f;               // How fast fog fades in/out
    public float targetDensity = 0.05f;          // Target density for fade
    private float initialDensity;

    void Start()
    {
        // Initialize fog settings
        RenderSettings.fog = enableFog;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogColor = fogColor;

        if (fogMode == FogMode.Linear)
        {
            RenderSettings.fogStartDistance = linearStart;
            RenderSettings.fogEndDistance = linearEnd;
        }
        else
        {
            RenderSettings.fogDensity = fogDensity;
        }

        initialDensity = fogDensity;
    }

    void Update()
    {
        if (!enableFog) return;

        // Optional dynamic fade in/out
        if (fadeFog)
        {
            fogDensity = Mathf.Lerp(fogDensity, targetDensity, Time.deltaTime * fadeSpeed);
            RenderSettings.fogDensity = fogDensity;
        }
    }

    // Example public method to trigger a fog fade (e.g. when entering an area)
    public void SetFogDensity(float newDensity)
    {
        targetDensity = Mathf.Clamp(newDensity, 0f, 0.2f);
        fadeFog = true;
    }
}
