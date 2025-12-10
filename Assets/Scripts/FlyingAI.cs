using UnityEngine;

/// <summary>
/// 飞行AI - 在指定范围内随机飞行
/// </summary>
public class FlyingAI : MonoBehaviour
{
    [Header("飞行范围")]
    [SerializeField] private Vector2 flyAreaCenter = Vector2.zero;  // 飞行区域中心
    [SerializeField] private Vector2 flyAreaSize = new Vector2(10f, 10f);  // 飞行区域大小
    
    [Header("飞行参数")]
    [SerializeField] private float moveSpeed = 2f;  // 移动速度
    [SerializeField] private float rotationSpeed = 5f;  // 转向速度
    [SerializeField] private float arrivalDistance = 0.5f;  // 到达目标点的距离阈值
    
    [Header("目标切换")]
    [SerializeField] private float minTargetChangeTime = 2f;  // 最小目标切换时间
    [SerializeField] private float maxTargetChangeTime = 5f;  // 最大目标切换时间
    
    private Vector3 currentTarget;
    private float targetChangeTimer;
    private float nextTargetChangeTime;
    
    private void Start()
    {
        // 初始化第一个目标点
        SetNewRandomTarget();
    }
    
    private void Update()
    {
        // 向目标移动
        MoveTowardsTarget();
        
        // 检查是否到达目标或超时
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget);
        targetChangeTimer += Time.deltaTime;
        
        if (distanceToTarget < arrivalDistance || targetChangeTimer >= nextTargetChangeTime)
        {
            SetNewRandomTarget();
        }
    }
    
    /// <summary>
    /// 向目标移动
    /// </summary>
    private void MoveTowardsTarget()
    {
        // 计算方向
        Vector3 direction = (currentTarget - transform.position).normalized;
        
        // 移动
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // 旋转朝向目标（2D平面）
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// 设置新的随机目标点
    /// </summary>
    private void SetNewRandomTarget()
    {
        // 在飞行区域内随机选择一个点
        float randomX = Random.Range(flyAreaCenter.x - flyAreaSize.x / 2, flyAreaCenter.x + flyAreaSize.x / 2);
        float randomY = Random.Range(flyAreaCenter.y - flyAreaSize.y / 2, flyAreaCenter.y + flyAreaSize.y / 2);
        
        currentTarget = new Vector3(randomX, randomY, transform.position.z);
        
        // 重置计时器
        targetChangeTimer = 0f;
        nextTargetChangeTime = Random.Range(minTargetChangeTime, maxTargetChangeTime);
        
        Debug.Log($"[FlyingAI] {gameObject.name} 新目标: {currentTarget}");
    }
    
    /// <summary>
    /// 设置飞行区域（动态调整）
    /// </summary>
    public void SetFlyArea(Vector2 center, Vector2 size)
    {
        flyAreaCenter = center;
        flyAreaSize = size;
    }
    
    // 调试可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制飞行区域
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(flyAreaCenter, flyAreaSize);
        
        // 绘制当前目标
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTarget, 0.3f);
            Gizmos.DrawLine(transform.position, currentTarget);
        }
    }
}
