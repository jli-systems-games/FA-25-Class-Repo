using UnityEngine;

public class AimAtPlayerShooter : MonoBehaviour
{
    [Header("子弹设置")]
    [Tooltip("子弹预制体")]
    public GameObject bulletPrefab;

    [Tooltip("子弹发射速度")]
    public float bulletSpeed = 10f;

    [Header("目标设置")]
    [Tooltip("要瞄准的目标（通常是玩家）")]
    public Transform playerTarget;

    [Tooltip("如果未指定目标，自动查找Player标签")]
    public bool autoFindPlayer = true;

    [Header("发射设置")]
    [Tooltip("发射间隔（秒）")]
    public float fireInterval = 2f;

    [Tooltip("随机发射偏差（度）- 设置越大越不准")]
    public float aimInaccuracy = 10f;

    [Tooltip("子弹生成高度")]
    public float spawnHeight = 0.5f;

    [Header("控制设置")]
    [Tooltip("开始发射前的延迟")]
    public float startDelay = 0f;

    [Tooltip("是否启用发射")]
    public bool enableFiring = true;

    private float fireTimer;
    private bool hasStarted = false;

    void Start()
    {
        fireTimer = fireInterval;

        // 查找玩家（只在开始时查找一次）
        if (playerTarget == null && autoFindPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player1");
            if (player != null)
            {
                playerTarget = player.transform;
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

        // 发射计时
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            FireAtPlayer();
            fireTimer = fireInterval;
        }
    }

    void FireAtPlayer()
    {
        if (bulletPrefab == null || playerTarget == null) return;

        // 计算发射位置
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += spawnHeight;

        // 计算基础发射方向（只在发射时检测一次）
        Vector3 fireDirection = (playerTarget.position - spawnPosition).normalized;

        // 添加随机偏差
        float randomAngleY = Random.Range(-aimInaccuracy, aimInaccuracy);
        fireDirection = Quaternion.Euler(0, randomAngleY, 0) * fireDirection;

        // 生成子弹
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(fireDirection));

        // 给子弹添加移动脚本
        SimpleBulletMovement movement = bullet.GetComponent<SimpleBulletMovement>();
        if (movement == null)
        {
            movement = bullet.AddComponent<SimpleBulletMovement>();
        }
        movement.direction = fireDirection;
        movement.speed = bulletSpeed;
        movement.destroyOnBuilding = true; // 确保碰到Building会销毁
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
        FireAtPlayer();
    }

    // 公开方法：设置目标
    public void SetTarget(Transform target)
    {
        playerTarget = target;
    }
}