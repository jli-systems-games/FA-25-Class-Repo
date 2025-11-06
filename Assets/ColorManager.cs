using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public Material[] colorThemes;
    public LightManager lightManager;
    public int CurrentThemeIndex { get; private set; } = 0;
    private Renderer[] renderTargets;

    void Start()
    {
        renderTargets = FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ApplyColor();
    }

    public void NextColor()
    {
        CurrentThemeIndex = (CurrentThemeIndex + 1) % colorThemes.Length;
        ApplyColor();
        lightManager.RefreshLights();
    }

    void ApplyColor()
    {
        foreach (Renderer r in renderTargets)
        {
            if (r.CompareTag("ColorChange"))
                r.material = colorThemes[CurrentThemeIndex];
        }
        RenderSettings.skybox = colorThemes[CurrentThemeIndex];
        DynamicGI.UpdateEnvironment();
    }
}
