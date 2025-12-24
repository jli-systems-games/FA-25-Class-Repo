using UnityEngine;

public class CameraShakePerlin : MonoBehaviour
{
    public DriveSmoothGrounded driver; // 可空：自动 FindObjectOfType
    [Header("Perlin 基础抖动（随速度）")]
    public float ampBase = 0.02f;
    public float ampAtMaxSpeed = 0.07f;
    public float freq = 8f;

    [Header("Kick（冲过起伏或按R）")]
    public float kickAmp = 0.3f;
    public float kickDur = 0.15f;

    Transform camT; Vector3 local0; float t; float kickT; float seedX, seedY;

    void Awake()
    {
        camT = transform; local0 = camT.localPosition;
        if (!driver) driver = FindObjectOfType<DriveSmoothGrounded>();
        seedX = Random.value * 100f; seedY = Random.value * 200f;
    }

    public void Kick(float amp, float dur)
    {
        kickAmp = amp; kickDur = dur; kickT = dur;
    }

    void LateUpdate()
    {
        t += Time.deltaTime;
        float k = 0f; if (driver) k = Mathf.Clamp01(driver.Speed / driver.maxSpeed);
        float amp = Mathf.Lerp(ampBase, ampAtMaxSpeed, k);

        // Perlin 偏移
        float nx = (Mathf.PerlinNoise(seedX, t * freq) - 0.5f) * 2f;
        float ny = (Mathf.PerlinNoise(seedY, t * freq) - 0.5f) * 2f;
        Vector3 perlin = new Vector3(nx, ny, 0f) * amp;

        // Kick 衰减
        if (kickT > 0f)
        {
            float f = kickT / kickDur;
            perlin += Random.insideUnitSphere * (kickAmp * f * f);
            kickT -= Time.deltaTime;
        }

        camT.localPosition = local0 + perlin;
    }
}
