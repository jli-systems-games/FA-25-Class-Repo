using UnityEngine;
using UnityEngine.UI; // 添加UI命名空间

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

    void Start()
    {
        currentHitCount = 0;
        UpdateHealthBar();
    }

    void OnCollisionEnter(Collision collision)
    {
        // 检测碰撞物体是否带有 Bullet 标签
        if (collision.gameObject.CompareTag("Bullet"))
        {
            HandleBulletHit(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 检测触发物体是否带有 Bullet 标签
        if (other.CompareTag("Bullet"))
        {
            HandleBulletHit(other.gameObject);
        }
    }

    void HandleBulletHit(GameObject bullet)
    {
        // 增加击中计数
        currentHitCount++;
        Debug.Log($"{gameObject.name} 被击中！当前次数: {currentHitCount}/{maxHitCount}");

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
            Debug.Log($"{gameObject.name} 已被击中 {maxHitCount} 次，即将销毁");
            Destroy(gameObject, destroyDelay);
        }
    }

    // 更新生命条显示
    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            // 计算剩余生命值百分比
            float healthPercent = (float)(maxHitCount - currentHitCount) / maxHitCount;
            healthSlider.value = healthPercent;
        }
    }

    // 获取当前生命值信息（可选的辅助方法）
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
        Debug.Log($"{gameObject.name} 受到伤害！当前次数: {currentHitCount}/{maxHitCount}");

        // 更新生命条
        UpdateHealthBar();

        if (currentHitCount >= maxHitCount)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}