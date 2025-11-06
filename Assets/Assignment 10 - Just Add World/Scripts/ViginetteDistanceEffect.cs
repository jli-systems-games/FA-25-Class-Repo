using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ViginetteDistanceEffect : MonoBehaviour
{
    public Transform player;        
    public Transform sphere;         
    public Volume volume; 
    public float maxDistance = 20f;  
    public float minDistance = 1f;   
    public float maxIntensity = 1f;
    public float minIntensity = 0.1f; 

    private Vignette vignette;

    void Start()
    {
        if (volume == null)
        {
            Debug.LogError("Volume not assigned!");
            return;
        }

        VolumeProfile profile = volume.profile;

        if (profile == null)
        {
            Debug.LogError("Volume has no profile assigned!");
            return;
        }

        // Try to get the vignette override
        if (!profile.TryGet<Vignette>(out vignette))
        {
            Debug.LogError("No Vignette override found in this Volume profile!");
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, sphere.position);

        float normalizedDist = Mathf.InverseLerp(maxDistance, minDistance, distance);

        float intensity = Mathf.Lerp(minIntensity, maxIntensity, normalizedDist);

        vignette.intensity.value = intensity;
    }
}
