using UnityEngine;

public class WanderingObject : MonoBehaviour
{
    [Header("移动区域边界")]
    public float minX = -132f;
    public float maxX = 48f;
    public float minZ = -63.7f;
    public float maxZ = 52f;
    
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f; // 转向速度
    
    [Header("方向改变设置")]
    public float directionChangeInterval = 3f; // 多久随机改变一次方向
    public float minDirectionChangeTime = 2f; // 最小方向改变间隔
    public float maxDirectionChangeTime = 5f; // 最大方向改变间隔
    
    private Vector3 moveDirection;
    private float directionChangeTimer;
    private float nextDirectionChangeTime;
    
    void Start()
    {
        // 初始化随机移动方向
        ChooseRandomDirection();
        nextDirectionChangeTime = Random.Range(minDirectionChangeTime, maxDirectionChangeTime);
    }
    
    void Update()
    {
        // 移动
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        
        // 检查边界
        CheckBoundaries();
        
        // 定时随机改变方向
        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer >= nextDirectionChangeTime)
        {
            ChooseRandomDirection();
            directionChangeTimer = 0f;
            nextDirectionChangeTime = Random.Range(minDirectionChangeTime, maxDirectionChangeTime);
        }
        
        // 让物体朝向移动方向（可选）
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    void CheckBoundaries()
    {
        Vector3 pos = transform.position;
        bool hitBoundary = false;
        
        // 检查X轴边界
        if (pos.x < minX)
        {
            pos.x = minX;
            moveDirection.x = Mathf.Abs(moveDirection.x); // 反弹，确保向右
            hitBoundary = true;
        }
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            moveDirection.x = -Mathf.Abs(moveDirection.x); // 反弹，确保向左
            hitBoundary = true;
        }
        
        // 检查Z轴边界
        if (pos.z < minZ)
        {
            pos.z = minZ;
            moveDirection.z = Mathf.Abs(moveDirection.z); // 反弹，确保向前
            hitBoundary = true;
        }
        else if (pos.z > maxZ)
        {
            pos.z = maxZ;
            moveDirection.z = -Mathf.Abs(moveDirection.z); // 反弹，确保向后
            hitBoundary = true;
        }
        
        transform.position = pos;
        
        // 如果碰到边界，重新标准化方向
        if (hitBoundary)
        {
            moveDirection.y = 0;
            moveDirection.Normalize();
        }
    }
    
    void ChooseRandomDirection()
    {
        // 随机选择一个新方向（在XZ平面上）
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);
        moveDirection = new Vector3(randomX, 0, randomZ).normalized;
    }
    
    // 碰撞检测 - 碰到障碍物后反弹
    void OnCollisionEnter(Collision collision)
    {
        // 获取碰撞法线
        if (collision.contacts.Length > 0)
        {
            Vector3 normal = collision.contacts[0].normal;
            
            // 根据法线反射移动方向
            moveDirection = Vector3.Reflect(moveDirection, normal);
            moveDirection.y = 0; // 保持在水平面
            moveDirection.Normalize();
            
            // 稍微随机化反射方向，让运动更自然
            float randomAngle = Random.Range(-30f, 30f);
            moveDirection = Quaternion.Euler(0, randomAngle, 0) * moveDirection;
        }
    }
    
    // 触发器检测（如果障碍物使用Trigger）
    void OnTriggerEnter(Collider other)
    {
        // 如果碰到的是障碍物，随机改变方向
        ChooseRandomDirection();
    }
    
    // 可视化边界（在Scene视图中）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        // 绘制边界框
        Vector3 center = new Vector3((minX + maxX) / 2f, transform.position.y, (minZ + maxZ) / 2f);
        Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
        
        Gizmos.DrawWireCube(center, size);
        
        // 绘制移动方向
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, moveDirection * 3f);
        }
    }
}
