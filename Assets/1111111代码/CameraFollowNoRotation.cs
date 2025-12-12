using UnityEngine;

public class CameraFollowNoRotation : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform player;

    [Header("固定位置")]
    public float fixedY = 6.6f;

    [Header("偏移")]
    public float xOffset = 0f;
    public float zOffset = -11.7f; // Z轴相对于玩家的偏移

    [Header("Z轴边界限制")]
    public float minZ = -44f;
    public float maxZ = 18f;

    [Header("X轴边界限制")]
    public float minX = -88f;
    public float maxX = 4.3f;

    [Header("跟随平滑度")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;

    // 记录摄像机初始旋转
    private Quaternion initialRotation;

    void Start()
    {
        // 保存摄像机初始朝向
        initialRotation = transform.rotation;

        // 如果没有指定player，尝试自动查找
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player1");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 计算目标X位置
        float targetX = player.position.x + xOffset;

        // 限制X轴在边界内
        targetX = Mathf.Clamp(targetX, minX, maxX);

        // 计算目标Z位置（跟随玩家 + 偏移）
        float targetZ = player.position.z + zOffset;

        // 限制Z轴在边界内
        targetZ = Mathf.Clamp(targetZ, minZ, maxZ);

        // 平滑移动X轴和Z轴
        float smoothedX = Mathf.Lerp(transform.position.x, targetX, smoothSpeed);
        float smoothedZ = Mathf.Lerp(transform.position.z, targetZ, smoothSpeed);

        // X和Z跟随，Y固定
        transform.position = new Vector3(smoothedX, fixedY, smoothedZ);

        // 始终保持初始旋转角度
        transform.rotation = initialRotation;
    }
}