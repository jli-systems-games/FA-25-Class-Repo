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
        SmartScalingWall[] walls = FindObjectsByType<SmartScalingWall>(FindObjectsSortMode.None);
        foreach (var w in walls)
        {
            if (w.OwnerID == OwnerID)
            {
                BoundWall = w;
                Debug.Log($"<color=cyan>✔ {name} ({OwnerID}) 成功绑定墙壁: {w.name}</color>");
                return;
            }
        }
        Debug.LogError($"<color=red>✘ {name} ({OwnerID}) 未找到对应的墙壁！请检查 OwnerID 设置。</color>");
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