using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHitCounter : MonoBehaviour
{
    [Header("子弹设置")]
    [Tooltip("是否销毁击中的子弹")]
    public bool destroyBulletOnHit = true;

    [Header("调试设置")]
    [Tooltip("是否显示调试信息")]
    public bool showDebugInfo = true;

    [Header("UI引用")]
    [Tooltip("场景中的LifeUIDisplay组件（可选，自动查找）")]
    public LifeUIDisplay lifeUIDisplay;

    [Header("无敌特效设置")]
    [Tooltip("无敌时生成的预制体")]
    public GameObject invincibleEffectPrefab;

    [Tooltip("预制体生成的父物体（通常是玩家自己）")]
    public Transform effectParent;

    private GameObject currentInvincibleEffect; // 当前生成的无敌特效实例

    [Header("击中生成设置")]
    [Tooltip("被击中时生成的预制体")]
    public GameObject hitSpawnPrefab;

    [Tooltip("在这个物体的子物体下生成")]
    public Transform spawnParent;

    [Tooltip("生成的预制体存活时间（秒），0表示不自动销毁")]
    public float spawnLifetime = 2f;

    [Header("死亡设置")]
    [Tooltip("死亡时生成的预制体")]
    public GameObject deathEffectPrefab;

    [Tooltip("死亡特效生成的位置（不指定则在玩家位置）")]
    public Transform deathEffectSpawnPoint;

    [Tooltip("死亡后等待多少秒跳转场景")]
    public float deathSceneDelay = 1f;

    private bool isDead = false;

    void Start()
    {
        // 如果没有手动指定，尝试自动查找场景中的LifeUIDisplay
        if (lifeUIDisplay == null)
        {
            lifeUIDisplay = FindObjectOfType<LifeUIDisplay>();
        }

        if (lifeUIDisplay == null && showDebugInfo)
        {
            Debug.LogWarning("未找到LifeUIDisplay组件！");
        }

        // 如果没有指定父物体，默认使用自己
        if (effectParent == null)
        {
            effectParent = transform;
        }
    }

    void Update()
    {
        // 持续检测生命值是否为0（即使不是被击中导致的）
        if (!isDead && !LifeManager.Instance.IsAlive())
        {
            OnPlayerDeath();
        }

        // 根据无敌状态生成或销毁特效
        if (PlayerAbilityManager.Instance.IsInvincible)
        {
            // 如果处于无敌状态但还没有生成特效，就生成
            if (currentInvincibleEffect == null && invincibleEffectPrefab != null)
            {
                currentInvincibleEffect = Instantiate(invincibleEffectPrefab, effectParent.position, effectParent.rotation, effectParent);

                if (showDebugInfo)
                {
                    Debug.Log("✨ 生成无敌特效");
                }
            }
        }
        else
        {
            // 如果不处于无敌状态但特效还在，就销毁
            if (currentInvincibleEffect != null)
            {
                Destroy(currentInvincibleEffect);
                currentInvincibleEffect = null;

                if (showDebugInfo)
                {
                    Debug.Log("❌ 销毁无敌特效");
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemybullet"))
        {
            RegisterHit(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemybullet"))
        {
            RegisterHit(other.gameObject);
        }
    }

    void RegisterHit(GameObject bullet)
    {
        if (showDebugInfo)
        {
            Debug.Log($"========== 检测到子弹击中 ==========");
            Debug.Log($"无敌状态: {PlayerAbilityManager.Instance.IsInvincible}");
        }

        // 检查是否处于无敌状态
        if (PlayerAbilityManager.Instance.IsInvincible)
        {
            if (showDebugInfo)
            {
                Debug.Log($"⭐ {gameObject.name} 处于无敌状态，忽略伤害！不扣血，不生成受击特效！");
            }

            // 即使无敌，也销毁子弹
            if (destroyBulletOnHit)
            {
                Destroy(bullet);
            }

            return; // 无敌时不扣血，直接返回
        }

        if (showDebugInfo)
        {
            Debug.Log($"❌ {gameObject.name} 被敌人子弹击中！开始扣血...");
        }

        // 减少生命值
        LifeManager.Instance.LoseLife();

        // 更新UI显示
        if (lifeUIDisplay != null)
        {
            lifeUIDisplay.UpdateLifeDisplay();
        }

        // 生成击中特效
        SpawnHitEffect();

        // 检查是否死亡（会在 Update 中统一处理）
        // 这里不需要再调用 OnPlayerDeath()

        // 销毁子弹
        if (destroyBulletOnHit)
        {
            Destroy(bullet);
        }
    }

    /// <summary>
    /// 在指定物体下生成预制体
    /// </summary>
    void SpawnHitEffect()
    {
        if (hitSpawnPrefab == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("未设置击中生成预制体！");
            }
            return;
        }

        if (spawnParent == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("未设置生成父物体！");
            }
            return;
        }

        // 在父物体的位置生成（作为子物体）
        GameObject spawnedObject = Instantiate(hitSpawnPrefab, spawnParent.position, spawnParent.rotation, spawnParent);

        if (showDebugInfo)
        {
            Debug.Log($"在 {spawnParent.name} 下生成了 {hitSpawnPrefab.name}");
        }

        // 如果设置了生存时间，自动销毁
        if (spawnLifetime > 0)
        {
            Destroy(spawnedObject, spawnLifetime);
        }
    }

    void OnPlayerDeath()
    {
        if (isDead) return; // 防止重复调用

        isDead = true;

        if (showDebugInfo)
        {
            Debug.Log("💀 玩家死亡！");
        }

        // 生成死亡特效
        if (deathEffectPrefab != null)
        {
            Vector3 spawnPosition = deathEffectSpawnPoint != null
                ? deathEffectSpawnPoint.position
                : transform.position;

            GameObject deathEffect = Instantiate(deathEffectPrefab, spawnPosition, Quaternion.identity);

            if (showDebugInfo)
            {
                Debug.Log($"✨ 生成死亡特效: {deathEffectPrefab.name}");
            }

            // 死亡特效不自动销毁，会跟随场景切换
        }

        // 启动跳转协程
        StartCoroutine(DeathSceneTransition());
    }

    IEnumerator DeathSceneTransition()
    {
        if (showDebugInfo)
        {
            Debug.Log($"⏳ 等待 {deathSceneDelay} 秒后跳转到 Lose 场景...");
        }

        // 等待指定时间
        yield return new WaitForSeconds(deathSceneDelay);

        if (showDebugInfo)
        {
            Debug.Log("🎬 跳转到 Lose 场景");
        }

        // 跳转到 Lose 场景
        SceneManager.LoadScene("Lose");
    }

    void OnDestroy()
    {
        // 当脚本被销毁时，确保清理无敌特效
        if (currentInvincibleEffect != null)
        {
            Destroy(currentInvincibleEffect);
        }
    }

    // 公开方法：获取当前生命值
    public int GetCurrentLives()
    {
        return LifeManager.Instance.GetCurrentLives();
    }
}