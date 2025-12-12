using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("射击设置")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float fireRate = 2f;
    public float bulletLifetime = 5f;

    [Header("自动发射设置")]
    [Tooltip("是否持续自动发射")]
    public bool autoFire = true;
    [Tooltip("游戏开始后多少秒才开始发射")]
    public float shootDelayTime = 4f;

    [Header("爱丽丝技能设置")]
    [Tooltip("连发时的射击间隔（秒）")]
    public float burstShotInterval = 0.1f;

    [Tooltip("连发时的角度偏移（度）")]
    public float burstAngleSpread = 5f;

    [Header("调试")]
    public bool showDebug = false;

    private Camera cam;
    private float nextFireTime = 0f;
    private float gameStartTime;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("找不到主摄像机！");
        }

        // 记录游戏开始时间
        gameStartTime = Time.time;
    }

    void Update()
    {
        // 检查是否已经过了延迟时间
        if (Time.time < gameStartTime + shootDelayTime)
        {
            return;
        }

        // 获取修改后的发射间隔
        float modifiedFireRate = PlayerAbilityManager.Instance.GetModifiedFireRate(fireRate);

        // 持续自动发射
        if (autoFire && Time.time >= nextFireTime)
        {
            // 检查是否有爱丽丝技能激活
            if (PlayerAbilityManager.Instance.IsAliceActive)
            {
                // 连发模式
                int extraShots = PlayerAbilityManager.Instance.GetAliceExtraShots();
                StartCoroutine(BurstShoot(1 + extraShots)); // 基础1发 + 额外发数
            }
            else
            {
                // 普通射击
                Shoot();
            }

            nextFireTime = Time.time + modifiedFireRate;
        }
    }

    /// <summary>
    /// 连发射击协程
    /// </summary>
    System.Collections.IEnumerator BurstShoot(int shotCount)
    {
        for (int i = 0; i < shotCount; i++)
        {
            // 计算角度偏移（让子弹稍微散开）
            float angleOffset = 0f;
            if (shotCount > 1)
            {
                // 将子弹分散在一个范围内
                float spreadRange = burstAngleSpread * (shotCount - 1);
                angleOffset = -spreadRange / 2f + (spreadRange / (shotCount - 1)) * i;
            }

            Shoot(angleOffset);

            if (i < shotCount - 1) // 最后一发不需要等待
            {
                yield return new WaitForSeconds(burstShotInterval);
            }
        }
    }

    /// <summary>
    /// 射击方法
    /// </summary>
    void Shoot(float angleOffset = 0f)
    {
        if (bulletPrefab == null || cam == null) return;

        // 步骤1: 获取鼠标的屏幕坐标
        Vector3 mouseScreenPos = Input.mousePosition;

        // 步骤2: 获取玩家的屏幕坐标
        Vector3 playerScreenPos = cam.WorldToScreenPoint(transform.position);

        // 步骤3: 计算屏幕空间的相对位置
        float deltaX = mouseScreenPos.x - playerScreenPos.x;
        float deltaY = mouseScreenPos.y - playerScreenPos.y;

        // 步骤4: 计算射击角度
        float screenAngle = Mathf.Atan2(deltaX, deltaY) * Mathf.Rad2Deg;
        float cameraYRotation = cam.transform.eulerAngles.y;
        float shootAngle = screenAngle + cameraYRotation + angleOffset; // 添加偏移

        // 步骤5: 创建子弹旋转
        Quaternion bulletRotation = Quaternion.Euler(0, shootAngle, 0);

        // 步骤6: 生成子弹
        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, bulletRotation);

        // 应用子弹大小加成
        float sizeMultiplier = PlayerAbilityManager.Instance.GetModifiedBulletSize(1f);
        bullet.transform.localScale *= sizeMultiplier;

        // 添加移动脚本
        ForwardBullet bulletScript = bullet.AddComponent<ForwardBullet>();

        // 应用子弹速度加成
        float modifiedSpeed = PlayerAbilityManager.Instance.GetModifiedBulletSpeed(bulletSpeed);
        bulletScript.speed = modifiedSpeed;

        // 自动销毁
        Destroy(bullet, bulletLifetime);

        // 调试信息
        if (showDebug)
        {
            Debug.Log($"=== 射击调试 ===");
            Debug.Log($"鼠标屏幕: ({mouseScreenPos.x}, {mouseScreenPos.y})");
            Debug.Log($"玩家屏幕: ({playerScreenPos.x}, {playerScreenPos.y})");
            Debug.Log($"射击角度: {shootAngle}度 (偏移: {angleOffset}度)");
            Debug.Log($"子弹速度: {modifiedSpeed} (基础: {bulletSpeed})");
            Debug.Log($"子弹大小: {sizeMultiplier}x");
            Debug.DrawRay(spawnPos, bulletRotation * Vector3.forward * 5f, Color.cyan, 1f);
        }
    }

    /// <summary>
    /// 手动触发射击（可以被外部调用）
    /// </summary>
    public void ManualShoot()
    {
        Shoot();
    }
}

// 子弹向前飞行
public class ForwardBullet : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}