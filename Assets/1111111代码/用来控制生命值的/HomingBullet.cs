using UnityEngine;

/// <summary>
/// 追踪子弹
/// 不断追踪目标，碰到Building标签会被销毁
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class HomingBullet : MonoBehaviour
{
    [Header("追踪目标")]
    [Tooltip("要追踪的目标")]
    public Transform target;

    [Header("追踪设置")]
    [Tooltip("追踪速度")]
    public float trackingSpeed = 8f;

    [Tooltip("旋转速度（转向速度）")]
    public float rotationSpeed = 200f;

    [Tooltip("最小追踪距离（距离目标小于此值时停止加速）")]
    public float minTrackingDistance = 1f;

    [Header("调试")]
    public bool showDebugInfo = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 配置 Rigidbody
        if (rb != null)
        {
            rb.useGravity = false;  // 不受重力影响
            rb.linearDamping = 0f;   // 无空气阻力
            rb.angularDamping = 0.5f; // 轻微的角阻力，让旋转更平滑
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // 只允许Y轴旋转
        }

        // 自动查找玩家（如果没有指定目标）
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player1");
            if (player != null)
            {
                target = player.transform;
            }
            else if (showDebugInfo)
            {
                Debug.LogWarning("追踪子弹未找到目标！");
            }
        }
    }

    void FixedUpdate()
    {
        if (target == null || rb == null) return;

        // 计算到目标的方向
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // 旋转朝向目标
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        // 如果距离目标还有一定距离，持续加速
        if (distanceToTarget > minTrackingDistance)
        {
            // 施加朝向目标的力
            Vector3 force = transform.forward * trackingSpeed;
            rb.linearVelocity = force;
        }

        if (showDebugInfo)
        {
            Debug.DrawRay(transform.position, directionToTarget * 2f, Color.red);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (showDebugInfo)
        {
            Debug.Log($"追踪子弹碰撞: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        }

        // 碰到 Building 标签的物体，销毁自己
        if (collision.gameObject.CompareTag("Building"))
        {
            if (showDebugInfo)
            {
                Debug.Log("追踪子弹碰到Building，销毁！");
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (showDebugInfo)
        {
            Debug.Log($"追踪子弹触发: {other.gameObject.name}, Tag: {other.gameObject.tag}");
        }

        // 碰到 Building 标签的物体，销毁自己
        if (other.CompareTag("Building"))
        {
            if (showDebugInfo)
            {
                Debug.Log("追踪子弹碰到Building触发器，销毁！");
            }
            Destroy(gameObject);
        }
    }

    // 公开方法：设置目标
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
