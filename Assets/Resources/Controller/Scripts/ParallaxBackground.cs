using UnityEngine;

/// <summary>
/// 视差背景管理器 - 管理多个背景图层
/// 自动为子对象设置不同的视差速度
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("相机设置")]
    [Tooltip("目标相机（如果为空则自动使用主相机）")]
    public Transform targetCamera;

    [Header("视差图层设置")]
    [Tooltip("背景图层列表（从后到前排列）")]
    public ParallaxLayer[] layers;

    private Vector3 lastCameraPosition;

    [System.Serializable]
    public class ParallaxLayer
    {
        [Tooltip("图层对象")]
        public Transform layerTransform;

        [Tooltip("视差速度 (0=完全静止，1=跟随相机)")]
        [Range(0f, 1f)]
        public float parallaxSpeed = 0.5f;

        [Tooltip("是否只在水平方向移动")]
        public bool horizontalOnly = true;

        [Tooltip("是否启用无限循环")]
        public bool infiniteLoop = false;

        [HideInInspector]
        public float textureUnitSizeX;
    }

    void Start()
    {
        // 自动查找主相机
        if (targetCamera == null)
        {
            targetCamera = Camera.main.transform;
        }

        lastCameraPosition = targetCamera.position;

        // 初始化图层
        foreach (ParallaxLayer layer in layers)
        {
            if (layer.infiniteLoop && layer.layerTransform != null)
            {
                SpriteRenderer sr = layer.layerTransform.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    Sprite sprite = sr.sprite;
                    layer.textureUnitSizeX = sprite.texture.width / sprite.pixelsPerUnit;
                }
            }
        }
    }

    void LateUpdate()
    {
        // 计算相机移动量
        Vector3 deltaMovement = targetCamera.position - lastCameraPosition;

        // 更新每个图层
        foreach (ParallaxLayer layer in layers)
        {
            if (layer.layerTransform == null) continue;

            // 应用视差效果
            Vector3 movement;
            if (layer.horizontalOnly)
            {
                movement = new Vector3(deltaMovement.x * layer.parallaxSpeed, 0, 0);
            }
            else
            {
                movement = new Vector3(
                    deltaMovement.x * layer.parallaxSpeed,
                    deltaMovement.y * layer.parallaxSpeed,
                    0
                );
            }

            layer.layerTransform.position += movement;

            // 无限循环逻辑
            if (layer.infiniteLoop && layer.textureUnitSizeX > 0)
            {
                if (Mathf.Abs(targetCamera.position.x - layer.layerTransform.position.x) >= layer.textureUnitSizeX)
                {
                    float offsetPositionX = (targetCamera.position.x - layer.layerTransform.position.x) % layer.textureUnitSizeX;
                    layer.layerTransform.position = new Vector3(
                        targetCamera.position.x + offsetPositionX,
                        layer.layerTransform.position.y,
                        layer.layerTransform.position.z
                    );
                }
            }
        }

        lastCameraPosition = targetCamera.position;
    }

    /// <summary>
    /// 在编辑器中显示调试信息
    /// </summary>
    void OnDrawGizmos()
    {
        if (layers == null) return;

        foreach (ParallaxLayer layer in layers)
        {
            if (layer.layerTransform != null)
            {
                // 用不同颜色显示不同速度的图层
                Gizmos.color = new Color(1f - layer.parallaxSpeed, layer.parallaxSpeed, 0.5f, 0.5f);
                Gizmos.DrawWireCube(layer.layerTransform.position, new Vector3(2f, 2f, 0.1f));
            }
        }
    }
}




