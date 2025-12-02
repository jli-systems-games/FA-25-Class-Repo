using UnityEngine;
using MoreMountains.TopDownEngine;

public class WeaponAutoDestroy : MonoBehaviour
{
    [Header("强制销毁设置")]
    public float DestroyDelay = 0.2f; // 延迟0.2秒，防止最后一发子弹还没射出去枪就没了

    private Weapon _weapon;

    void Start()
    {
        _weapon = GetComponent<Weapon>();
    }

    void Update()
    {
        if (_weapon == null) return;

        // 只有当这是一个“基于弹匣”的武器时才检查
        if (_weapon.MagazineBased)
        {
            // 如果当前弹匣里的子弹 <= 0
            if (_weapon.CurrentAmmoLoaded <= 0)
            {
                // 这一步是为了防止引擎还在尝试“自动换弹”或者卡在别的状态
                // 我们直接切断它的念想，销毁物体
                Destroy(this.gameObject, DestroyDelay);
            }
        }
    }
}