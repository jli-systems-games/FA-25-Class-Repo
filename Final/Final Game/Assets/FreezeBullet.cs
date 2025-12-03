using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class FreezeBullet : MonoBehaviour
{
    [Header("冷冻弹设置")]
    public float Duration = 2.0f; // 冻结时长 2秒

    // 我们需要用 DamageOnTouch 来获取 Owner (即使伤害为0)
    // 这是 TDE 传递“谁开了枪”的标准方式
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

        // --- 1. 获取被击中者的冻结控制器 ---
        var hitController = other.GetComponent<PlayerFreezeController>();
        if (hitController == null) hitController = other.GetComponentInParent<PlayerFreezeController>();

        // --- 2. 身份核查 (防止误伤自己) ---
        // 使用 DamageOnTouch 里的 Owner 信息
        if (_damageOnTouch != null && _damageOnTouch.Owner != null)
        {
            GameObject ownerObj = _damageOnTouch.Owner;

            // 检查撞到的物体是否属于 Owner (包括 Owner 本身，或 Owner 的手臂、枪模型)
            if (other.gameObject == ownerObj || other.transform.IsChildOf(ownerObj.transform))
            {
                // 是自己人，直接 Return 忽略本次碰撞
                return;
            }
        }

        // --- 3. 命中敌人逻辑 ---
        if (hitController != null)
        {
            _hasHit = true;
            Debug.Log($"❄️ [命中] 成功冻结目标: {other.name}");

            hitController.ApplyFreeze(Duration);
            Despawn();
        }
        // --- 4. 撞墙逻辑 ---
        // 排除自己(Layer check)、排除Player(Tag check)、排除Trigger
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