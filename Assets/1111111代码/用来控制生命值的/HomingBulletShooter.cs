using UnityEngine;

/// <summary>
/// 追踪子弹发射器
/// 定期向玩家发射会追踪的子弹
/// </summary>
public class HomingBulletShooter : MonoBehaviour
{
    [Header("子弹设置")]
    [Tooltip("追踪子弹预制体")]
    public GameObject homingBulletPrefab;

    [Header("发射设置")]
    [Tooltip("发射间隔（秒）")]
    public float fireInterval = 3f;

    [Tooltip("子弹初始速度")]
    public float initialSpeed = 5f;

    [Tooltip("子弹追踪速度")]
    public float trackingSpeed = 8f;

    [Tooltip("子弹旋转速度（追踪时转向的速度）")]
    public float rotationSpeed = 200f;

    [Header("目标设置")]
    [Tooltip("要追踪的目标（通常是玩家）")]
    public Transform playerTarget;

    [Tooltip("如果未指定目标，自动查找Player标签")]
    public bool autoFindPlayer = true;

    [Header("生成设置")]
    [Tooltip("子弹生成高度偏移")]
    public float spawnHeight = 0.5f;

    [Header("控制设置")]
    [Tooltip("开始发射前的延迟")]
    public float startDelay = 0f;

    [Tooltip("是否启用发射")]
    public bool enableFiring = true;

    [Header("调试")]
    public bool showDebugInfo = true;

    private float fireTimer;
    private bool hasStarted = false;

    void Start()
    {
        fireTimer = fireInterval;

        // 查找玩家
        if (playerTarget == null && autoFindPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player1");
            if (player != null)
            {
                playerTarget = player.transform;
            }
            else if (showDebugInfo)
            {
                Debug.LogWarning("未找到Player1标签的物体！");
            }
        }

        // 延迟启动
        if (startDelay > 0)
        {
            Invoke("ActivateFiring", startDelay);
        }
        else
        {
            hasStarted = true;
        }
    }

    void Update()
    {
        if (!hasStarted || !enableFiring) return;

        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            FireHomingBullet();
            fireTimer = fireInterval;
        }
    }

    void FireHomingBullet()
    {
        if (homingBulletPrefab == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("未设置追踪子弹预制体！");
            }
            return;
        }

        if (playerTarget == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("没有追踪目标！");
            }
            return;
        }

        // 计算发射位置
        Vector3 spawnPosition = transform.position + Vector3.up * spawnHeight;

        // 计算朝向玩家的方向
        Vector3 directionToPlayer = (playerTarget.position - spawnPosition).normalized;

        // 生成子弹
        GameObject bullet = Instantiate(homingBulletPrefab, spawnPosition, Quaternion.LookRotation(directionToPlayer));

        // 配置追踪子弹组件
        HomingBullet homingScript = bullet.GetComponent<HomingBullet>();
        if (homingScript == null)
        {
            homingScript = bullet.AddComponent<HomingBullet>();
        }

        homingScript.target = playerTarget;
        homingScript.trackingSpeed = trackingSpeed;
        homingScript.rotationSpeed = rotationSpeed;
        homingScript.showDebugInfo = showDebugInfo;

        // 给予初始速度
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = directionToPlayer * initialSpeed;
        }

        if (showDebugInfo)
        {
            Debug.Log($"发射追踪子弹，目标: {playerTarget.name}");
        }
    }

    void ActivateFiring()
    {
        hasStarted = true;
    }

    // 公开方法：启用/禁用发射
    public void SetFiringEnabled(bool enabled)
    {
        enableFiring = enabled;
    }

    // 公开方法：立即发射
    public void FireImmediately()
    {
        FireHomingBullet();
    }

    // 公开方法：设置目标
    public void SetTarget(Transform target)
    {
        playerTarget = target;
    }
}
