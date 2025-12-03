using UnityEngine;

public class KeepCanvasRotation : MonoBehaviour
{
    [Header("跟随目标设置")]
    [Tooltip("Canvas要跟随的目标物体")]
    public Transform targetToFollow;

    [Header("销毁设置")]
    [Tooltip("当目标被销毁时，是否同时销毁Canvas")]
    public bool destroyWithTarget = true;

    [Tooltip("目标被销毁后，Canvas延迟销毁的时间（秒）")]
    public float destroyDelay = 0f;

    [Header("固定设置")]
    [Tooltip("Canvas固定的Y轴高度")]
    public float fixedYPosition = 11.2f;

    [Tooltip("Canvas固定的旋转")]
    public Vector3 fixedRotation = Vector3.zero;

    [Tooltip("开始时记录当前Y位置")]
    public bool useCurrentYOnStart = true;

    [Header("位置偏移")]
    [Tooltip("相对于目标的XZ平面偏移")]
    public Vector2 xzOffset = Vector2.zero; // x代表X轴偏移，y代表Z轴偏移

    [Header("平滑设置")]
    [Tooltip("是否使用平滑移动")]
    public bool smoothPosition = false;

    [Tooltip("位置平滑速度")]
    public float positionSpeed = 5f;

    void Start()
    {
        // 如果设置为使用当前Y位置，则记录初始高度
        if (useCurrentYOnStart)
        {
            fixedYPosition = transform.position.y;
        }

        // 设置初始旋转
        transform.rotation = Quaternion.Euler(fixedRotation);
    }

    void LateUpdate()
    {
        // 检测目标是否被销毁
        if (destroyWithTarget && targetToFollow == null)
        {
            Destroy(gameObject, destroyDelay);
            return; // 目标已销毁，不再更新位置
        }

        // LateUpdate确保在目标物体移动之后再更新
        UpdatePosition();

        // 确保旋转始终保持固定
        transform.rotation = Quaternion.Euler(fixedRotation);
    }

    void UpdatePosition()
    {
        if (targetToFollow != null)
        {
            // 只跟随目标的X和Z位置，Y保持固定
            Vector3 targetPosition = new Vector3(
                targetToFollow.position.x + xzOffset.x,
                fixedYPosition,
                targetToFollow.position.z + xzOffset.y
            );

            if (smoothPosition)
            {
                // 平滑移动
                transform.position = Vector3.Lerp(transform.position, targetPosition, positionSpeed * Time.deltaTime);
            }
            else
            {
                // 直接设置位置
                transform.position = targetPosition;
            }
        }
    }

    // 公开方法：设置跟随目标
    public void SetFollowTarget(Transform target)
    {
        targetToFollow = target;
    }

    // 公开方法：设置固定Y位置
    public void SetFixedYPosition(float yPos)
    {
        fixedYPosition = yPos;
    }

    // 公开方法：设置固定旋转
    public void SetFixedRotation(Vector3 rotation)
    {
        fixedRotation = rotation;
        transform.rotation = Quaternion.Euler(fixedRotation);
    }

    // 公开方法：设置XZ偏移
    public void SetXZOffset(Vector2 offset)
    {
        xzOffset = offset;
    }

    // 在编辑器中可视化
    void OnDrawGizmosSelected()
    {
        if (targetToFollow != null)
        {
            Gizmos.color = Color.green;

            // 绘制目标位置
            Vector3 targetPos = new Vector3(
                targetToFollow.position.x + xzOffset.x,
                fixedYPosition,
                targetToFollow.position.z + xzOffset.y
            );

            Gizmos.DrawWireSphere(targetPos, 0.3f);

            // 绘制从目标物体到Canvas位置的连线
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(targetToFollow.position, targetPos);
        }
    }
}