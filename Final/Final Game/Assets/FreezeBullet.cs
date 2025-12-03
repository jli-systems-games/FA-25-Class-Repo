using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools; // 引用工具包以检测对象池

public class FreezeBullet : MonoBehaviour
{
    [Header("冷冻弹设置")]
    public float Duration = 2.0f; // 冻结时长 2秒

    [Header("安全设置")]
    public float ActivationDelay = 0.1f; // 生成后0.1秒内不生效

    // 防止一次碰撞触发多次
    private bool _hasHit = false;
    private float _spawnTime;

    // --- 改用 OnEnable ---
    // 因为子弹是对象池复用的，Start 只会运行一次，而 OnEnable 每次发射都会运行
    void OnEnable()
    {
        _hasHit = false; // 重置碰撞状态
        _spawnTime = Time.time; // 重置出生时间
        Debug.Log($"🔧 [子弹复用] 我发射了！时间: {_spawnTime}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        // 安全检查：如果子弹刚出生，忽略碰撞
        if (Time.time - _spawnTime < ActivationDelay) return;

        // --- 侦探功能 ---
        // Debug.Log($"💥 [碰撞检查] 我撞到了: {other.name}");

        // 1. 尝试找玩家身上的冻结控制器
        var controller = other.GetComponent<PlayerFreezeController>();
        if (controller == null) controller = other.GetComponentInParent<PlayerFreezeController>();

        // 2. 如果打中了玩家
        if (controller != null)
        {
            _hasHit = true;
            Debug.Log($"❄️ [逻辑成功] 正在冻结 {other.name}...");

            controller.ApplyFreeze(Duration);

            // 【修复】正确回收子弹
            Despawn();
        }
        // 3. 如果撞到别的东西 (不是自己，不是其他子弹，不是触发器)
        else if (!other.CompareTag("Projectile") && !other.CompareTag("Player") && !other.isTrigger)
        {
            Despawn();
        }
    }

    // --- 智能销毁/回收方法 ---
    void Despawn()
    {
        // 检查自己是不是对象池里的东西
        if (GetComponent<MMPoolableObject>() != null)
        {
            gameObject.SetActive(false); // 只是隐藏，回收到池里等待下次使用
        }
        else
        {
            Destroy(gameObject); // 如果不是池子里的，才彻底销毁
        }
    }
}