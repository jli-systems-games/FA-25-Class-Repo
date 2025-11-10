using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OrbAppear : MonoBehaviour
{
    public float appearDuration = 2f;       
    public Light orbLight;
    private Material orbMat;
    private Color baseEmission;
    private float timer = 0f;

    void Start()
    {
        
        orbMat = GetComponent<Renderer>().material;

        
        if (orbMat.HasProperty("_EmissionColor"))
            baseEmission = orbMat.GetColor("_EmissionColor");
        else
            baseEmission = Color.white;

        
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

            
            Color currentEmission = Color.Lerp(Color.black, baseEmission, t);
            orbMat.SetColor("_EmissionColor", currentEmission);

            
            if (orbLight != null)
                orbLight.intensity = Mathf.Lerp(0f, 5f, t);
        }
    }
}
