using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LightFlicker : MonoBehaviour
{
    public float minAlpha = 0.6f;
    public float maxAlpha = 1.0f;
    public float flickerSpeed = 2f;
    public float randomIntensity = 0.2f;

    private SpriteRenderer sr;
    private float seed;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        seed = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float t = (Mathf.Sin((Time.time + seed) * flickerSpeed) + 1f) * 0.5f;
        float randomNoise = (Random.value - 0.5f) * randomIntensity;

        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t) + randomNoise;
        alpha = Mathf.Clamp(alpha, 0f, 1f);

        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}
