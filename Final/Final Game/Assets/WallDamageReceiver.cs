using UnityEngine;
using MoreMountains.TopDownEngine;

public class WallDamageReceiver : MonoBehaviour
{
    [Header("状态监控")]
    public string OwnerID;
    public SmartScalingWall BoundWall;

    private Character _character;

    void Start()
    {
        _character = GetComponent<Character>();
        if (_character == null) return;

        OwnerID = _character.PlayerID;
        FindMyWall();
    }

    void FindMyWall()
    {
        // 查找场景里所有的 SmartScalingWall
        SmartScalingWall[] walls = FindObjectsByType<SmartScalingWall>(FindObjectsSortMode.None);

        foreach (var w in walls)
        {
            // --- 核心修改 ---
            // 只要墙的 Owner 是我，或者墙的 Partner 是我，都算作“我的墙”
            // 这样 P1/P3 会绑定同一个墙，P2/P4 会绑定同一个墙
            if (w.OwnerID == OwnerID || w.PartnerID == OwnerID)
            {
                BoundWall = w;
                Debug.Log($"<color=cyan>✔ {name} ({OwnerID}) 成功绑定墙壁: {w.name} (Owner:{w.OwnerID}, Partner:{w.PartnerID})</color>");
                return;
            }
        }

        Debug.LogError($"<color=red>✘ {name} ({OwnerID}) 未找到对应的墙壁！请检查 SmartScalingWall 上的 OwnerID 和 PartnerID 是否设置正确。</color>");
    }

    public void OnHitByDebuffWeapon(float shrinkAmount)
    {
        if (BoundWall != null)
        {
            BoundWall.ShrinkWall(shrinkAmount);
            // 播放一个简单的反馈日志
            Debug.Log($"<color=orange>⚡ {name} 被击中！墙壁缩短了 {shrinkAmount}</color>");
        }
    }
}