using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class SlowBullet : MonoBehaviour
{
    [Header("减速弹设置")]
    public float Duration = 3.0f;     // 减速持续 3秒
    [Range(0.1f, 1f)]
    public float SlowFactor = 0.5f;   // 0.5 = 减速一倍 (变为50%速度)

    // 必须依赖 DamageOnTouch 来识别谁开了枪
    private DamageOnTouch _damageOnTouch;
    private bool _hasHit = false;

    void Awake()
    {
        _damageOnTouch = GetComponent<DamageOnTouch>();
    }

    void OnEnable()
    {
        _hasHit = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        // --- 1. 尝试获取对方的减速控制器 ---
        var hitSlowController = other.GetComponent<PlayerSlowController>();
        if (hitSlowController == null) hitSlowController = other.GetComponentInParent<PlayerSlowController>();

        // --- 2. 身份核查 (防止误伤自己) ---
        // 这一步至关重要，哪怕子弹没伤害，也得防止它刚出生就撞到枪口被销毁
        if (_damageOnTouch != null && _damageOnTouch.Owner != null)
        {
            GameObject ownerObj = _damageOnTouch.Owner;

            // 检查撞到的东西是不是主人的身体部位
            if (other.gameObject == ownerObj || other.transform.IsChildOf(ownerObj.transform))
            {
                // 是自己人，什么都别做，让子弹穿过去
                return;
            }
        }

        // --- 3. 命中敌人逻辑 ---
        if (hitSlowController != null)
        {
            _hasHit = true;
            // Debug.Log($"🐢 [命中] 减速目标: {other.name}");

            // 施加减速
            hitSlowController.ApplySlow(Duration, SlowFactor);

            // 销毁子弹
            Despawn();
        }
        // --- 4. 撞墙逻辑 ---
        // 排除自己(Bullet层)、排除玩家(Tag防止漏网)、排除Trigger区
        else if (other.gameObject.layer != gameObject.layer && !other.CompareTag("Player") && !other.isTrigger)
        {
            Despawn();
        }
    }

    void Despawn()
    {
        if (GetComponent<MMPoolableObject>() != null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}