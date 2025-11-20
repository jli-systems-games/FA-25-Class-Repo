using UnityEngine;
using MoreMountains.TopDownEngine;

public class WeaponSpeedModifier : MonoBehaviour
{
    [Header("移动速度倍率")]
    [Range(0.1f, 2f)]
    public float SpeedMultiplier = 0.5f; // 默认0.5，即半速

    private CharacterMovement _movement;
    private bool _initialized = false;

    void Update()
    {
        // 1. 如果还没有找到玩家（或者玩家丢了），就尝试去找
        if (_movement == null)
        {
            _movement = GetComponentInParent<CharacterMovement>();

            // 如果找到了，标记一下，并打印日志方便调试
            if (_movement != null && !_initialized)
            {
                _initialized = true;
                // Debug.Log("成功连接到玩家: " + _movement.name + "，开始减速！");
            }
        }

        // 2. 如果找到了玩家，就死死地把速度按住
        if (_movement != null)
        {
            _movement.MovementSpeedMultiplier = SpeedMultiplier;
        }
    }

    void OnDisable()
    {
        // 当武器被收起、扔掉或销毁时，把速度恢复正常
        // 注意：这里必须检查 _movement 是否还存在，防止报错
        if (_movement != null)
        {
            _movement.MovementSpeedMultiplier = 1f;
        }

        // 重置状态，以便下次拿出来时重新寻找
        _movement = null;
        _initialized = false;
    }
}