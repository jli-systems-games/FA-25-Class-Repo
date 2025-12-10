using UnityEngine;

/// <summary>
/// 动物基类 - 管理生命周期、年龄、死亡
/// 具体行为由子类实现（ButterflyBehavior等）
/// </summary>
public class Animal : MonoBehaviour
{
    [Header("动物配置")]
    public AnimalData animalData;
    
    [Header("生命状态")]
    private float currentAge = 0f;
    private float deathTime;  // 死亡时间（随机）
    private bool isDead = false;
    
    [Header("资源状态")]
    private float noResourceTimer = 0f;  // 无资源计时器
    private bool hasResource = true;  // 当前是否有资源可用
    
    // 公开属性
    public float CurrentAge => currentAge;
    public bool IsDead => isDead;
    public float NoResourceTimer => noResourceTimer;
    public AnimalData AnimalData => animalData;
    
    private SpriteRenderer spriteRenderer;
    
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (animalData != null)
        {
            // 随机化死亡时间
            deathTime = Random.Range(animalData.lifespanMin, animalData.lifespanMax);
            
            // 设置贴图
            if (spriteRenderer != null && animalData.animalSprite != null)
            {
                spriteRenderer.sprite = animalData.animalSprite;
            }
            
            Debug.Log($"[Animal] {animalData.animalName} 生成，寿命: {deathTime:F1}秒");
        }
    }
    
    protected virtual void Update()
    {
        if (isDead) return;
        
        // 更新年龄
        currentAge += Time.deltaTime;
        
        // 检查自然寿命
        if (currentAge >= deathTime)
        {
            Die(true);  // 自然死亡
            return;
        }
        
        // 更新无资源计时器
        UpdateResourceTimer();
    }
    
    /// <summary>
    /// 更新无资源计时器
    /// </summary>
    private void UpdateResourceTimer()
    {
        if (animalData == null) return;
        
        if (!hasResource)
        {
            noResourceTimer += Time.deltaTime;
            
            // 超过最大无资源时间，死亡
            if (noResourceTimer >= animalData.maxTimeWithoutResource)
            {
                Debug.Log($"[Animal] {animalData.animalName} 找不到资源超过 {animalData.maxTimeWithoutResource} 秒，死亡");
                Die(false);  // 饥饿死亡
            }
        }
        else
        {
            // 重置计时器（找到资源了）
            noResourceTimer = 0f;
        }
    }
    
    /// <summary>
    /// 设置资源状态（由子类调用）
    /// </summary>
    public void SetResourceAvailable(bool available)
    {
        hasResource = available;
    }
    
    /// <summary>
    /// 动物死亡
    /// </summary>
    public void Die(bool isNaturalDeath)
    {
        if (isDead) return;
        
        isDead = true;
        
        string deathReason = isNaturalDeath ? "自然死亡" : "资源不足";
        Debug.Log($"[Animal] {animalData.animalName} {deathReason}，存活时间: {currentAge:F1}秒");
        
        // 播放死亡特效
        if (animalData.deathParticle != null)
        {
            GameObject particle = Instantiate(animalData.deathParticle, transform.position, Quaternion.identity);
            Destroy(particle, 3f);
        }
        
        // 延迟销毁（让特效播放完）
        Destroy(gameObject, 0.5f);
    }
    
    /// <summary>
    /// 触发交互特效（授粉、进食等）
    /// </summary>
    public void PlayInteractionEffect(Vector3 position)
    {
        if (animalData.interactionParticle != null)
        {
            GameObject particle = Instantiate(animalData.interactionParticle, position, Quaternion.identity);
            Destroy(particle, 2f);
        }
    }
    
    // 调试信息
    protected virtual void OnDrawGizmosSelected()
    {
        if (animalData == null) return;
        
        // 绘制检测范围
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, animalData.detectionRange);
    }
}
