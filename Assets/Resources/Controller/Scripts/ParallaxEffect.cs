using UnityEngine;

/// <summary>
/// 视差滚动效果 - 创造多图层深度感
/// 距离越远的背景移动越慢，距离越近的前景移动越快
/// </summary>
public class ParallaxEffect : MonoBehaviour
{
    [Header("视差设置")]
    [Tooltip("视差速度因子 (0-1之间，越小移动越慢，越接近1移动越快)")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    [Tooltip("是否只在水平方向应用视差")]
    public bool horizontalOnly = true;

    [Tooltip("是否启用无限循环背景")]
    public bool infiniteLoop = false;

    [Header("自动设置（可选）")]
    [Tooltip("相机（如果为空则自动查找主相机）")]
    public Transform cameraTransform;

    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;

    void Start()
    {
        // 如果没有设置相机，自动使用主相机
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        lastCameraPosition = cameraTransform.position;

        // 如果启用无限循环，计算纹理大小
        if (infiniteLoop)
        {
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        }
    }

    void LateUpdate()
    {
        // 计算相机移动距离
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // 根据视差因子移动背景
        if (horizontalOnly)
        {
            // 只在水平方向应用视差
            transform.position += new Vector3(deltaMovement.x * parallaxFactor, 0, 0);
        }
        else
        {
            // 在水平和垂直方向都应用视差
            transform.position += new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * parallaxFactor, 0);
        }

        // 更新上一帧相机位置
        lastCameraPosition = cameraTransform.position;

        // 如果启用无限循环
        if (infiniteLoop)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y, transform.position.z);
            }
        }
    }
}




