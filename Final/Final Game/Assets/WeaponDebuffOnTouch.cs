using UnityEngine;

public class DebuffBullet : MonoBehaviour
{
    [Header("惩罚设置")]
    public float ShrinkSize = 1.0f; // 每次击中让墙缩短多少

    // 强制只生效一次，防止一颗子弹瞬间判定多次
    private bool _hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        // 1. 尝试从碰撞体本身，或者它的父物体里找接收器
        // (有时候碰撞体挂在模型的子骨骼上，而脚本在根节点，这样能顺藤摸瓜找到)
        WallDamageReceiver receiver = other.GetComponent<WallDamageReceiver>();
        if (receiver == null) receiver = other.GetComponentInParent<WallDamageReceiver>();

        // 2. 如果找到了接收器 (说明打中人了)
        if (receiver != null)
        {
            _hasHit = true;
            Debug.Log($"<color=green>🔫 子弹击中了 {receiver.name}，正在施加惩罚...</color>");

            // 调用对方身上的缩墙方法
            receiver.OnHitByDebuffWeapon(ShrinkSize);

            // 任务完成，销毁子弹
            Destroy(gameObject);
        }
        // 3. 撞到其他东西 (比如障碍物、墙壁)
        // 排除 Player 和 Projectile 标签，防止子弹刚生成就被自己人撞没了
        else if (!other.CompareTag("Player") && !other.CompareTag("Projectile"))
        {
            // 只有撞到实体障碍物(非Trigger)才销毁
            // 这样防止撞到其他的逻辑判定区(比如摄像机触发区)就消失了
            if (!other.isTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}