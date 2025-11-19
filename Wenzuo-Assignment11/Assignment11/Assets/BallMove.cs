using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallMove : MonoBehaviour
{
    public float moveForce = 80f;      // 平时乱窜的力度
    public float centerBias = 0.35f;   // 越大越往中心挤
    public float collisionBoost = 10f; // 碰撞基础爆冲力
    public float collisionScale = 1.5f; // 根据相对速度再放大一点
    [HideInInspector]
    public Vector3 arenaCenter;        // 由 BallSpawner 设置

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 随机一个平面方向
        Vector3 randomDir = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;

        // 往圆盘中心方向
        Vector3 toCenter = arenaCenter - transform.position;
        toCenter.y = 0f;
        if (toCenter.sqrMagnitude > 0.01f)
        {
            toCenter = toCenter.normalized;
        }
        else
        {
            toCenter = Vector3.zero;
        }

        // 混合：随机 + 往中心
        Vector3 finalDir = (randomDir * (1f - centerBias) + toCenter * centerBias).normalized;

        rb.AddForce(finalDir * moveForce, ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 如果撞到的是别的球
        if (collision.rigidbody != null)
        {
            // 取第一个接触点的法线（指向自己的反方向）
            Vector3 normal = collision.contacts[0].normal;

            // 相对速度越大，冲击越大
            float relativeSpeed = collision.relativeVelocity.magnitude;

            float boost = collisionBoost + relativeSpeed * collisionScale;

            // 反向推自己一把（更夸张的弹开效果）
            rb.AddForce(-normal * boost, ForceMode.Impulse);
        }
    }
}
