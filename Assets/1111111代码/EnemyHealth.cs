using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("生命值设置")]
    [Tooltip("需要被击中几次才销毁")]
    public int maxHitCount = 3;
    private int currentHitCount = 0;

    [Header("UI设置")]
    [Tooltip("显示生命值的Slider")]
    public Slider healthSlider;

    [Header("销毁设置")]
    [Tooltip("是否同时销毁子弹")]
    public bool destroyBullet = true;

    [Tooltip("销毁前的延迟时间（秒）")]
    public float destroyDelay = 0f;

    [Header("调试")]
    [Tooltip("显示详细调试信息")]
    public bool showDebugInfo = true;

    void Start()
    {
        currentHitCount = 0;

        // 检查Slider设置
        if (healthSlider != null)
        {
            // 确保Slider的范围是0-1
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;

            if (showDebugInfo)
            {
                Debug.Log($"[{gameObject.name}] Slider设置: Min={healthSlider.minValue}, Max={healthSlider.maxValue}");
            }
        }
        else if (showDebugInfo)
        {
            Debug.LogWarning($"[{gameObject.name}] 未设置Health Slider！");
        }

        UpdateHealthBar();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            HandleBulletHit(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            HandleBulletHit(other.gameObject);
        }
    }

    void HandleBulletHit(GameObject bullet)
    {
        currentHitCount++;

        if (showDebugInfo)
        {
            Debug.Log($"[{gameObject.name}] 被击中！当前次数: {currentHitCount}/{maxHitCount}");
        }

        // 更新生命条
        UpdateHealthBar();

        // 销毁子弹
        if (destroyBullet)
        {
            Destroy(bullet);
        }

        // 检查是否达到销毁条件
        if (currentHitCount >= maxHitCount)
        {
            if (showDebugInfo)
            {
                Debug.Log($"[{gameObject.name}] 已被击中 {maxHitCount} 次，即将销毁");
            }
            Destroy(gameObject, destroyDelay);
        }
    }

    // 更新生命条显示
    void UpdateHealthBar()
    {
        if (healthSlider == null) return;

        // 计算剩余生命值百分比 (0.0 到 1.0)
        float healthPercent = (float)(maxHitCount - currentHitCount) / maxHitCount;

        // 设置Slider的值
        healthSlider.value = healthPercent;

        if (showDebugInfo)
        {
            Debug.Log($"[{gameObject.name}] 生命条更新: " +
                     $"剩余={maxHitCount - currentHitCount}/{maxHitCount}, " +
                     $"百分比={healthPercent:F2}, " +
                     $"Slider.value={healthSlider.value:F2}");
        }
    }

    // 获取当前生命值信息
    public int GetCurrentHitCount()
    {
        return currentHitCount;
    }

    public int GetRemainingHits()
    {
        return maxHitCount - currentHitCount;
    }

    // 可以被外部调用来直接造成伤害
    public void TakeDamage(int damageCount = 1)
    {
        currentHitCount += damageCount;

        if (showDebugInfo)
        {
            Debug.Log($"[{gameObject.name}] 受到伤害！当前次数: {currentHitCount}/{maxHitCount}");
        }

        UpdateHealthBar();

        if (currentHitCount >= maxHitCount)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}