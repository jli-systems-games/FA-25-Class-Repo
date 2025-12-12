using UnityEngine;

public class CircularBulletShooter : MonoBehaviour
{
    [Header("子弹设置")]
    [Tooltip("子弹预制体")]
    public GameObject bulletPrefab;

    [Tooltip("子弹发射速度")]
    public float bulletSpeed = 10f;

    [Header("环形发射设置")]
    [Tooltip("一圈发射多少颗子弹")]
    public int bulletsPerCircle = 12;

    [Tooltip("发射间隔（秒）")]
    public float fireInterval = 1f;

    [Header("旋转设置")]
    [Tooltip("每次发射时旋转的角度")]
    public float rotationPerFire = 15f;

    [Tooltip("是否持续旋转（每帧旋转）")]
    public bool continuousRotation = false;

    [Tooltip("持续旋转速度（度/秒）")]
    public float continuousRotationSpeed = 30f;

    [Header("发射位置设置")]
    [Tooltip("子弹发射的半径（距离中心点的距离）")]
    public float fireRadius = 1f;

    [Tooltip("子弹发射的高度偏移（相对于物体中心）")]
    public float heightOffset = 0f;

    [Header("控制设置")]
    [Tooltip("开始发射前的延迟时间")]
    public float startDelay = 0f;

    [Tooltip("是否启用发射")]
    public bool enableFiring = true;

    private float fireTimer;
    private float currentRotationAngle = 0f;
    private bool hasStarted = false;

    void Start()
    {
        fireTimer = fireInterval;

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

        // 持续旋转
        if (continuousRotation)
        {
            currentRotationAngle += continuousRotationSpeed * Time.deltaTime;
        }

        // 发射计时
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            FireCircularPattern();
            fireTimer = fireInterval;
        }
    }

    // 发射一圈子弹
    void FireCircularPattern()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("子弹预制体未设置！");
            return;
        }

        // 计算每颗子弹之间的角度
        float angleStep = 360f / bulletsPerCircle;

        for (int i = 0; i < bulletsPerCircle; i++)
        {
            // 计算当前子弹的角度（加上旋转偏移）
            float angle = (angleStep * i + currentRotationAngle) * Mathf.Deg2Rad;

            // 计算发射位置
            Vector3 spawnOffset = new Vector3(
                Mathf.Cos(angle) * fireRadius,
                heightOffset,
                Mathf.Sin(angle) * fireRadius
            );
            Vector3 spawnPosition = transform.position + spawnOffset;

            // 计算发射方向（从中心指向外）
            Vector3 fireDirection = spawnOffset.normalized;
            fireDirection.y = 0; // 保持在水平面

            // 生成子弹
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

            // 让子弹朝向发射方向
            if (fireDirection != Vector3.zero)
            {
                bullet.transform.rotation = Quaternion.LookRotation(fireDirection);
            }

            // 给子弹添加移动脚本
            SimpleBulletMovement movement = bullet.GetComponent<SimpleBulletMovement>();
            if (movement == null)
            {
                movement = bullet.AddComponent<SimpleBulletMovement>();
            }
            movement.direction = fireDirection;
            movement.speed = bulletSpeed;
        }

        // 每次发射后旋转
        if (!continuousRotation)
        {
            currentRotationAngle += rotationPerFire;
        }

        // 保持角度在0-360范围内
        if (currentRotationAngle >= 360f)
        {
            currentRotationAngle -= 360f;
        }
    }

    // 激活发射
    void ActivateFiring()
    {
        hasStarted = true;
    }

    // 公开方法：启用/禁用发射
    public void SetFiringEnabled(bool enabled)
    {
        enableFiring = enabled;
    }

    // 公开方法：立即发射一圈
    public void FireImmediately()
    {
        FireCircularPattern();
    }

    // 公开方法：设置旋转角度
    public void SetRotationAngle(float rotation)
    {
        currentRotationAngle = rotation;
    }

    // 在编辑器中可视化发射范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // 绘制发射半径
        Vector3 center = transform.position + Vector3.up * heightOffset;

        // 绘制圆圈
        int segments = 32;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = angleStep * i * Mathf.Deg2Rad;
            float angle2 = angleStep * (i + 1) * Mathf.Deg2Rad;

            Vector3 point1 = center + new Vector3(
                Mathf.Cos(angle1) * fireRadius,
                0,
                Mathf.Sin(angle1) * fireRadius
            );

            Vector3 point2 = center + new Vector3(
                Mathf.Cos(angle2) * fireRadius,
                0,
                Mathf.Sin(angle2) * fireRadius
            );

            Gizmos.DrawLine(point1, point2);
        }

        // 绘制子弹发射点
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            float bulletAngleStep = 360f / bulletsPerCircle;

            for (int i = 0; i < bulletsPerCircle; i++)
            {
                float angle = (bulletAngleStep * i + currentRotationAngle) * Mathf.Deg2Rad;
                Vector3 point = center + new Vector3(
                    Mathf.Cos(angle) * fireRadius,
                    0,
                    Mathf.Sin(angle) * fireRadius
                );

                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}