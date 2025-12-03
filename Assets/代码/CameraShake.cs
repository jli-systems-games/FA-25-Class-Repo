using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("抖动设置")]
    [Tooltip("抖动持续时间（秒）")]
    public float shakeDuration = 0.2f;
    
    [Tooltip("抖动强度")]
    public float shakeMagnitude = 0.3f;
    
    [Tooltip("抖动衰减速度")]
    public float dampingSpeed = 1.0f;
    
    private Vector3 initialPosition;
    private float currentShakeDuration = 0f;
    
    void Awake()
    {
        // 记录相机初始位置
        initialPosition = transform.localPosition;
    }
    
    void Update()
    {
        if (currentShakeDuration > 0)
        {
            // 在初始位置附近随机抖动
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            
            // 减少剩余抖动时间
            currentShakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            // 抖动结束，恢复到初始位置
            currentShakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }
    
    /// <summary>
    /// 触发相机抖动
    /// </summary>
    public void Shake()
    {
        currentShakeDuration = shakeDuration;
    }
    
    /// <summary>
    /// 触发相机抖动（自定义参数）
    /// </summary>
    /// <param name="duration">持续时间</param>
    /// <param name="magnitude">抖动强度</param>
    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        currentShakeDuration = duration;
    }
}
