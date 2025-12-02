using UnityEngine;
using MoreMountains.TopDownEngine;

public class WallDamageReceiver : MonoBehaviour // 1. 删掉了 IDamageable
{
    private Character _character;
    // private Health _health; // 2. 如果不需要扣血，这个也可以不引用，看你需求

    // 引用自己的墙
    private SmartScalingWall _myWall;

    void Start()
    {
        _character = GetComponent<Character>();
        // _health = GetComponent<Health>(); 

        // 自动找到自己的墙 (保持你原有的逻辑)
        SmartScalingWall[] walls = FindObjectsByType<SmartScalingWall>(FindObjectsSortMode.None);
        foreach (var w in walls)
        {
            if (w.OwnerID == _character.PlayerID)
            {
                _myWall = w;
                break;
            }
        }
    }

    // --- 新增：被武器击中时调用 ---
    public void OnHitByDebuffWeapon(float shrinkAmount)
    {
        if (_myWall != null)
        {
            // 假设你的 SmartScalingWall 脚本里有 ShrinkWall 方法
            // 如果没有，你需要去那个脚本里加一个 public void ShrinkWall(float amount)
            _myWall.ShrinkWall(shrinkAmount);
            Debug.Log($"玩家 {_character.PlayerID} 被击中，墙壁缩短了！");
        }
    }
}