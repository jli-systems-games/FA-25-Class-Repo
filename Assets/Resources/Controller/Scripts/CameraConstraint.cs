using UnityEngine;

/// <summary>
/// 相机约束 - 确保相机视野完全在 Collider2D 范围内
/// 只需要设置一个 Collider2D，相机不会超出其边界
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraConstraint : MonoBehaviour
{
    [Header("边界设置")]
    [Tooltip("边界 Collider（可包含子对象中的多个 Collider）")]
    public GameObject boundaryObject;

    private Camera cam;
    private float minX, maxX, minY, maxY;
    private bool boundsCalculated = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        CalculateBounds();
    }

    void LateUpdate()
    {
        if (!boundsCalculated) return;

        // 限制相机位置
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    /// <summary>
    /// 计算边界
    /// </summary>
    void CalculateBounds()
    {
        if (boundaryObject == null)
        {
            Debug.LogWarning("CameraConstraint: 未设置 boundaryObject！");
            return;
        }

        // 获取所有 Collider2D
        Collider2D[] colliders = boundaryObject.GetComponentsInChildren<Collider2D>();
        
        if (colliders.Length == 0)
        {
            Debug.LogWarning("CameraConstraint: 未找到任何 Collider2D！");
            return;
        }

        // 计算所有 Collider 的合并边界
        Bounds combinedBounds = colliders[0].bounds;
        for (int i = 1; i < colliders.Length; i++)
        {
            combinedBounds.Encapsulate(colliders[i].bounds);
        }

        // 计算相机尺寸
        float cameraHeight = cam.orthographicSize * 2f;
        float cameraWidth = cameraHeight * cam.aspect;

        // 计算相机中心的移动边界（确保相机视野不超出 Collider）
        minX = combinedBounds.min.x + cameraWidth / 2f;
        maxX = combinedBounds.max.x - cameraWidth / 2f;
        minY = combinedBounds.min.y + cameraHeight / 2f;
        maxY = combinedBounds.max.y - cameraHeight / 2f;

        // 如果边界太小，设置为中心点
        if (minX > maxX)
        {
            float centerX = (combinedBounds.min.x + combinedBounds.max.x) / 2f;
            minX = maxX = centerX;
        }
        if (minY > maxY)
        {
            float centerY = (combinedBounds.min.y + combinedBounds.max.y) / 2f;
            minY = maxY = centerY;
        }

        boundsCalculated = true;

        Debug.Log($"相机边界设置完成 ({colliders.Length} 个 Collider):");
        Debug.Log($"  Collider 范围: X({combinedBounds.min.x:F2} ~ {combinedBounds.max.x:F2}), Y({combinedBounds.min.y:F2} ~ {combinedBounds.max.y:F2})");
        Debug.Log($"  相机尺寸: 宽={cameraWidth:F2}, 高={cameraHeight:F2}");
        Debug.Log($"  相机中心限制: X({minX:F2} ~ {maxX:F2}), Y({minY:F2} ~ {maxY:F2})");
    }

    /// <summary>
    /// 在编辑器中显示边界
    /// </summary>
    void OnDrawGizmos()
    {
        if (boundaryObject == null) return;

        Collider2D[] colliders = boundaryObject.GetComponentsInChildren<Collider2D>();
        if (colliders.Length == 0) return;

        // 计算合并边界
        Bounds combinedBounds = colliders[0].bounds;
        for (int i = 1; i < colliders.Length; i++)
        {
            combinedBounds.Encapsulate(colliders[i].bounds);
        }

        // 绘制 Collider 边界（绿色）
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(combinedBounds.center, combinedBounds.size);

        // 计算并绘制相机中心边界（黄色）
        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            float cameraHeight = camera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * camera.aspect;

            float drawMinX = combinedBounds.min.x + cameraWidth / 2f;
            float drawMaxX = combinedBounds.max.x - cameraWidth / 2f;
            float drawMinY = combinedBounds.min.y + cameraHeight / 2f;
            float drawMaxY = combinedBounds.max.y - cameraHeight / 2f;

            if (drawMinX <= drawMaxX && drawMinY <= drawMaxY)
            {
                Gizmos.color = Color.yellow;
                Vector3 center = new Vector3((drawMinX + drawMaxX) / 2f, (drawMinY + drawMaxY) / 2f, transform.position.z);
                Vector3 size = new Vector3(drawMaxX - drawMinX, drawMaxY - drawMinY, 0.1f);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}




