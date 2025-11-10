using UnityEngine;

public class FogController : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;               
    public FogMode fogMode = FogMode.Exponential;
    [ColorUsage(true, true)]
    public Color fogColor = new Color(0.3f, 0.25f, 0.4f); 
    [Range(0.001f, 0.1f)]
    public float fogDensity = 0.02f;             
    public float linearStart = 30f;            
    public float linearEnd = 120f;               

    [Header("Dynamic Fade Settings (optional)")]
    public bool fadeFog = false;                 
    public float fadeSpeed = 0.5f;               
    public float targetDensity = 0.05f;          
    private float initialDensity;

    void Start()
    {
       
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

       
        if (fadeFog)
        {
            fogDensity = Mathf.Lerp(fogDensity, targetDensity, Time.deltaTime * fadeSpeed);
            RenderSettings.fogDensity = fogDensity;
        }
    }

    
    public void SetFogDensity(float newDensity)
    {
        targetDensity = Mathf.Clamp(newDensity, 0f, 0.2f);
        fadeFog = true;
    }
}
