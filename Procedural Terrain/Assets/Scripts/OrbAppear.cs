using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OrbAppear : MonoBehaviour
{
    public float appearDuration = 2f;       // How long it takes to fade in
    public Light orbLight;                  // Optional: assign your Point Light
    private Material orbMat;
    private Color baseEmission;
    private float timer = 0f;

    void Start()
    {
        // Duplicate the material so we don't affect all orbs globally
        orbMat = GetComponent<Renderer>().material;

        // Get the base emission color
        if (orbMat.HasProperty("_EmissionColor"))
            baseEmission = orbMat.GetColor("_EmissionColor");
        else
            baseEmission = Color.white;

        // Start fully dark
        orbMat.SetColor("_EmissionColor", Color.black);
        if (orbLight != null)
            orbLight.intensity = 0f;
    }

    void Update()
    {
        if (timer < appearDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / appearDuration);

            // Fade in emission
            Color currentEmission = Color.Lerp(Color.black, baseEmission, t);
            orbMat.SetColor("_EmissionColor", currentEmission);

            // Fade in light if exists
            if (orbLight != null)
                orbLight.intensity = Mathf.Lerp(0f, 5f, t);
        }
    }
}
