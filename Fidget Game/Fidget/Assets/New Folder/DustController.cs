using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DustController : MonoBehaviour
{
    public DriveSmoothGrounded driver;
    public float startAtSpeed = 2.0f; // 低速不出尘
    public float maxSpeed = 14f;
    public float maxRate = 35f;       // 比之前更低，避免遮挡视野
    public float maxStartSpeed = 1.8f;

    ParticleSystem ps; ParticleSystem.EmissionModule em; ParticleSystem.MainModule main;

    void Awake() { ps = GetComponent<ParticleSystem>(); em = ps.emission; main = ps.main; }

    void LateUpdate()
    {
        if (!driver) return;
        float v = driver.Speed;
        float k = Mathf.InverseLerp(startAtSpeed, maxSpeed, v);
        k = Mathf.Clamp01(k);

        em.rateOverTime = maxRate * k;
        main.startSpeed = Mathf.Lerp(0.1f, maxStartSpeed, k);
    }
}
