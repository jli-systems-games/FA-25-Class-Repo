using UnityEngine;

public class NoisyObject : MonoBehaviour
{
    [Header("一次性噪音强度（比如打翻杯子）")]
    [Range(0f, 2f)] public float pulseStrength = 0.6f;

    [Header("可选：冷却，避免连续触发太频繁")]
    public float cooldown = 0.2f;
    float timer;

    void Update()
    {
        if (timer > 0f) timer -= Time.deltaTime;
    }

    public void MakeNoisePulse()
    {
        if (timer > 0f) return;
        timer = cooldown;

        if (NoiseSystem.I)
        {
            NoiseSystem.I.Pulse(transform.position, pulseStrength);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MakeNoisePulse();
        }
    }
}
