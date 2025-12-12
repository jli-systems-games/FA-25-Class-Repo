using UnityEngine;

/// <summary>
/// 视差前景效果 - 前景移动速度大于1，创造景深感
/// 越靠近相机的物体移动越快
/// </summary>
public class ParallaxForeground : MonoBehaviour
{
    [Header("视差前景设置")]
    [Tooltip("视差速度倍数 (大于1，前景移动比相机快)")]
    [Range(1.0f, 3.0f)]
    public float parallaxSpeed = 1.5f;

    [Tooltip("目标相机（留空则自动使用主相机）")]
    public Transform targetCamera;

    [Tooltip("是否只在水平方向应用视差")]
    public bool horizontalOnly = true;

    [Header("外观设置")]
    [Tooltip("前景排序层级")]
    public string sortingLayerName = "Foreground";

    [Tooltip("排序顺序")]
    public int sortingOrder = 10;

    [Header("无限循环（可选）")]
    [Tooltip("启用无限循环背景")]
    public bool infiniteLoop = false;

    private Vector3 lastCameraPosition;
    private SpriteRenderer spriteRenderer;
    private float spriteWidth;

    void Start()
    {
        // 自动获取主相机
        if (targetCamera == null)
        {
            targetCamera = Camera.main.transform;
        }

        lastCameraPosition = targetCamera.position;

        // 设置精灵渲染器
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 设置排序层
            spriteRenderer.sortingLayerName = sortingLayerName;
            spriteRenderer.sortingOrder = sortingOrder;

            // 计算精灵宽度（用于无限循环）
            if (infiniteLoop && spriteRenderer.sprite != null)
            {
                spriteWidth = spriteRenderer.bounds.size.x;
            }
        }
    }

    void LateUpdate()
    {
        // 计算相机移动量
        Vector3 deltaMovement = targetCamera.position - lastCameraPosition;

        // 应用视差效果（前景反向移动，速度更快）
        Vector3 parallaxMovement;
        if (horizontalOnly)
        {
            parallaxMovement = new Vector3(-deltaMovement.x * parallaxSpeed, 0, 0);
        }
        else
        {
            parallaxMovement = new Vector3(
                -deltaMovement.x * parallaxSpeed,
                -deltaMovement.y * parallaxSpeed,
                0
            );
        }

        transform.position += parallaxMovement;

        // 更新上一帧位置
        lastCameraPosition = targetCamera.position;

        // 处理无限循环
        if (infiniteLoop && spriteWidth > 0)
        {
            HandleInfiniteLoop();
        }
    }

    /// <summary>
    /// 处理无限循环逻辑
    /// </summary>
    private void HandleInfiniteLoop()
    {
        float distance = targetCamera.position.x - transform.position.x;
        
        if (Mathf.Abs(distance) >= spriteWidth)
        {
            float offset = distance % spriteWidth;
            transform.position = new Vector3(
                targetCamera.position.x - offset,
                transform.position.y,
                transform.position.z
            );
        }
    }

    /// <summary>
    /// 运行时更新视差速度
    /// </summary>
    public void SetParallaxSpeed(float newSpeed)
    {
        parallaxSpeed = Mathf.Max(1.0f, newSpeed);
    }

    /// <summary>
    /// 在编辑器中显示调试信息
    /// </summary>
    private void OnDrawGizmos()
    {
        // 用颜色表示速度（越快越红）
        float normalizedSpeed = (parallaxSpeed - 1f) / 2f; // 将1-3映射到0-1
        Gizmos.color = new Color(normalizedSpeed, 1f - normalizedSpeed, 0.5f, 0.5f);
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 1f, 0.1f));
    }
}

