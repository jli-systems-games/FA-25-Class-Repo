using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("射击设置")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float fireRate = 0.2f;
    public float bulletLifetime = 5f;

    [Header("调试")]
    public bool showDebug = true;

    private Camera cam;
    private float nextFireTime = 0f;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("找不到主摄像机！");
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
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
        float shootAngle = screenAngle + cameraYRotation;

        // 步骤5: 创建子弹旋转（只有子弹朝向鼠标）
        Quaternion bulletRotation = Quaternion.Euler(0, shootAngle, 0);

        // 步骤6: 生成子弹
        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, bulletRotation);

        // 添加移动脚本
        ForwardBullet bulletScript = bullet.AddComponent<ForwardBullet>();
        bulletScript.speed = bulletSpeed;

        // 自动销毁
        Destroy(bullet, bulletLifetime);

        // 调试信息
        if (showDebug)
        {
            Debug.Log($"=== 射击调试 ===");
            Debug.Log($"鼠标屏幕: ({mouseScreenPos.x}, {mouseScreenPos.y})");
            Debug.Log($"玩家屏幕: ({playerScreenPos.x}, {playerScreenPos.y})");
            Debug.Log($"射击角度: {shootAngle}度");
            Debug.Log($"子弹方向: {bulletRotation * Vector3.forward}");
            Debug.DrawRay(spawnPos, bulletRotation * Vector3.forward * 5f, Color.cyan, 1f);
        }
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