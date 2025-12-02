using UnityEngine;

public class DebuffBullet : MonoBehaviour
{
    public float ShrinkSize = 1.0f; //这里设置“一个小石头的长度”

    private void OnTriggerEnter(Collider other)
    {
        // 尝试获取碰到的人身上的接收器
        var receiver = other.GetComponent<WallDamageReceiver>();

        if (receiver != null)
        {
            // 触发缩墙
            receiver.OnHitByDebuffWeapon(ShrinkSize);
            // 销毁子弹
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player"))
        {
            // 碰到墙壁消失，但不触发效果 (根据需要调整)
            Destroy(gameObject);
        }
    }
}