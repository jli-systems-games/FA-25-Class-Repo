using UnityEngine;

[DefaultExecutionOrder(-500)]
public class NoiseSystem : MonoBehaviour
{
    public static NoiseSystem I;

    [Header("Noise 数值（UI 再 Clamp 到 0..1）")]
    [Tooltip("当前噪音值；调试可看，不建议运行时在 Inspector 修改")]
    public float noiseLevel = 0f;

    [Header("衰减")]
    [Tooltip("每秒衰减量（线性），0.05~0.12 比较自然")]
    public float decayPerSecond = 0.08f;

    [Header("阈值")]
    [Range(0f, 1f)] public float warningThreshold = 0.7f;
    [Range(0f, 1f)] public float criticalThreshold = 1.0f;

    // 可选：给脉冲事件监听（怪物等用）
    public System.Action<Vector3, float> OnNoisePulse;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Update()
    {
        if (noiseLevel > 0f)
            noiseLevel = Mathf.Max(0f, noiseLevel - decayPerSecond * Time.deltaTime);
    }

    /// <summary>连续叠加：传“每帧增量”（= 每秒强度 * Time.deltaTime）</summary>
    public void AddNoise(float amount)
    {
        if (amount <= 0f) return;
        noiseLevel += amount;                 // 不在这里 Clamp，UI 时再 Clamp01
    }

    /// <summary>一次性噪音脉冲（摔倒/打碎物体）</summary>
    public void Pulse(Vector3 worldPos, float strength = 1f)
    {
        noiseLevel += Mathf.Max(0f, strength);
        OnNoisePulse?.Invoke(worldPos, Mathf.Clamp01(strength));
    }

    public bool IsWarning() => Mathf.Clamp01(noiseLevel) >= warningThreshold && Mathf.Clamp01(noiseLevel) < criticalThreshold;
    public bool IsCritical() => Mathf.Clamp01(noiseLevel) >= criticalThreshold;
}
