using UnityEngine;

/// <summary>
/// 地面行走AI - 兔子、狐狸、松鼠等地面动物
/// 适配2D侧视图游戏（XY平面），只左右翻转贴图，不旋转
/// </summary>
public class WalkingAI : MonoBehaviour
{
    [Header("行走范围")]
    [SerializeField] private float minX = -10f;  // 最小X坐标
    [SerializeField] private float maxX = 10f;   // 最大X坐标
    [SerializeField] private float groundY = 0f;  // 地面高度（Y坐标）
    
    [Header("行走参数")]
    [SerializeField] private float moveSpeed = 2f;  // 移动速度
    [SerializeField] private float arrivalDistance = 0.3f;  // 到达目标点的距离阈值
    
    [Header("目标切换")]
    [SerializeField] private float minTargetChangeTime = 2f;  // 最小目标切换时间
    [SerializeField] private float maxTargetChangeTime = 5f;  // 最大目标切换时间
    
    [Header("停顿设置")]
    [SerializeField] private bool enablePause = true;  // 是否启用随机停顿
    [SerializeField] private float minPauseTime = 1f;  // 最小停顿时间
    [SerializeField] private float maxPauseTime = 3f;  // 最大停顿时间
    [SerializeField] private float pauseChance = 0.3f;  // 到达目标后停顿的概率（0-1）
    
    private Vector3 currentTarget;
    private float targetChangeTimer;
    private float nextTargetChangeTime;
    
    private bool isPaused = false;
    private float pauseTimer = 0f;
    private float pauseDuration = 0f;
    
    [Header("翻转设置")]
    [SerializeField] private bool flipByScale = true;  // 使用Scale翻转整个物体（适合有子物体的情况）
    
    [Header("调试")]
    [SerializeField] private bool showFlipDebug = false;  // 显示翻转调试信息
    
    private bool lastFlipState = false;  // 上一帧的翻转状态
    
    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        // 只在需要时获取SpriteRenderer
        if (!flipByScale)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }
        
        // 确保在地面上（2D侧视图，只锁定Y）
        Vector3 pos = transform.position;
        pos.y = groundY;
        pos.z = 0f;  // 2D游戏，Z轴固定为0
        transform.position = pos;
        
        // 确保没有旋转
        transform.rotation = Quaternion.identity;
        
        // 初始化第一个目标点
        SetNewRandomTarget();
    }
    
    private void Update()
    {
        // 如果正在停顿
        if (isPaused)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                isPaused = false;
                SetNewRandomTarget();
            }
            return;
        }
        
        // 向目标移动
        MoveTowardsTarget();
        
        // 检查是否到达目标或超时
        float distanceToTarget = Mathf.Abs(transform.position.x - currentTarget.x);
        targetChangeTimer += Time.deltaTime;
        
        if (distanceToTarget < arrivalDistance || targetChangeTimer >= nextTargetChangeTime)
        {
            // 到达目标后，可能停顿
            if (enablePause && Random.value < pauseChance)
            {
                StartPause();
            }
            else
            {
                SetNewRandomTarget();
            }
        }
    }
    
    /// <summary>
    /// 向目标移动（2D侧视图，只在X轴移动）
    /// </summary>
    private void MoveTowardsTarget()
    {
        // 计算方向（只在X轴）
        float direction = Mathf.Sign(currentTarget.x - transform.position.x);
        
        // 移动（只改变X，Y和Z锁定）
        Vector3 newPos = transform.position;
        newPos.x += direction * moveSpeed * Time.deltaTime;
        newPos.y = groundY;  // 锁定Y轴在地面
        newPos.z = 0f;       // 2D游戏，Z轴固定为0
        transform.position = newPos;
        
        // 🎯 关键：翻转兔子
        if (flipByScale)
        {
            // 方法1：翻转整个物体（包括所有子物体）
            Vector3 scale = transform.localScale;
            
            if (direction < 0)
            {
                scale.x = -Mathf.Abs(scale.x);  // 向左走，X轴负数（镜像翻转）
            }
            else if (direction > 0)
            {
                scale.x = Mathf.Abs(scale.x);   // 向右走，X轴正数（正常）
            }
            
            // 检测翻转变化
            bool currentFlipState = scale.x < 0;
            if (showFlipDebug && currentFlipState != lastFlipState)
            {
                Debug.Log($"[WalkingAI] {gameObject.name} 翻转整个物体！scale.x = {scale.x}");
                lastFlipState = currentFlipState;
            }
            
            transform.localScale = scale;
        }
        else if (spriteRenderer != null)
        {
            // 方法2：只翻转贴图（原来的方法）
            if (direction < 0)
            {
                spriteRenderer.flipX = true;  // 向左走，翻转贴图
            }
            else if (direction > 0)
            {
                spriteRenderer.flipX = false; // 向右走，不翻转
            }
            
            // 调试：检测翻转变化
            if (showFlipDebug && spriteRenderer.flipX != lastFlipState)
            {
                Debug.Log($"[WalkingAI] {gameObject.name} 翻转贴图！flipX = {spriteRenderer.flipX}");
                lastFlipState = spriteRenderer.flipX;
            }
            
            // 🆕 如果贴图默认朝左，取消下面的注释，注释掉上面的代码
            // if (direction < 0)
            // {
            //     spriteRenderer.flipX = false;  // 向左走，不翻转（因为默认朝左）
            // }
            // else if (direction > 0)
            // {
            //     spriteRenderer.flipX = true;   // 向右走，翻转
            // }
        }
        
        // 确保物体不旋转
        transform.rotation = Quaternion.identity;
    }
    
    /// <summary>
    /// 设置新的随机目标点（2D侧视图，只随机X坐标）
    /// </summary>
    private void SetNewRandomTarget()
    {
        // 在X轴范围内随机选择一个点
        float randomX = Random.Range(minX, maxX);
        
        currentTarget = new Vector3(randomX, groundY, 0f);
        
        // 重置计时器
        targetChangeTimer = 0f;
        nextTargetChangeTime = Random.Range(minTargetChangeTime, maxTargetChangeTime);
        
        Debug.Log($"[WalkingAI] {gameObject.name} 新目标: X={randomX:F1}");
    }
    
    /// <summary>
    /// 开始停顿
    /// </summary>
    private void StartPause()
    {
        isPaused = true;
        pauseTimer = 0f;
        pauseDuration = Random.Range(minPauseTime, maxPauseTime);
        
        Debug.Log($"[WalkingAI] {gameObject.name} 停顿 {pauseDuration:F1} 秒");
    }
    
    /// <summary>
    /// 设置行走范围（动态调整）
    /// </summary>
    public void SetWalkRange(float min, float max)
    {
        minX = min;
        maxX = max;
    }
    
    /// <summary>
    /// 设置移动速度（动态调整）
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    
    // 调试可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制行走范围（2D侧视图，X轴的一条线）
        Gizmos.color = Color.green;
        Vector3 leftPoint = new Vector3(minX, groundY, 0f);
        Vector3 rightPoint = new Vector3(maxX, groundY, 0f);
        Gizmos.DrawLine(leftPoint, rightPoint);
        
        // 绘制范围边界
        Gizmos.DrawWireSphere(leftPoint, 0.3f);
        Gizmos.DrawWireSphere(rightPoint, 0.3f);
        
        // 绘制当前目标
        if (Application.isPlaying)
        {
            Gizmos.color = isPaused ? Color.yellow : Color.red;
            Gizmos.DrawWireSphere(currentTarget, 0.3f);
            Gizmos.DrawLine(transform.position, currentTarget);
            
            // 显示状态文字
            #if UNITY_EDITOR
            string status = isPaused ? $"停顿中 {pauseTimer:F1}/{pauseDuration:F1}s" : "移动中";
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, status);
            #endif
        }
    }
}
