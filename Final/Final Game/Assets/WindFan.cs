using UnityEngine;
using MoreMountains.TopDownEngine;

public class WindFan : MonoBehaviour
{
    [Header("开关状态")]
    public bool IsOn = false;

    [Header("风力设置")]
    [Range(0, 200)] // 加上这个，你就可以在面板上拖动滑动条了
    [Tooltip("风推人的力度，数字越大推得越远")]
    public float WindForce = 50f;

    [Tooltip("风的方向 (黄色箭头指向)")]
    public Vector3 LocalWindDirection = Vector3.forward;

    [Header("视觉效果")]
    public Transform FanBlades;
    public float RotationSpeed = 800f;

    private void Update()
    {
        // 扇叶旋转
        if (IsOn && FanBlades != null)
        {
            FanBlades.Rotate(0, 0, RotationSpeed * Time.deltaTime); // 注意：根据你的模型轴向，可能需要改这里
        }
    }

    // 按钮调用的开关
    public void ToggleFan()
    {
        IsOn = !IsOn;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsOn) return;

        // 计算风的世界方向
        Vector3 finalWindDir = transform.TransformDirection(LocalWindDirection).normalized;

        // TDE 角色
        TopDownController3D tdeController = other.GetComponent<TopDownController3D>();
        if (tdeController != null)
        {
            // 给角色施加力
            tdeController.AddForce(finalWindDir * WindForce * Time.deltaTime);
        }
        // 普通刚体 (如推箱子)
        else if (other.attachedRigidbody != null && !other.attachedRigidbody.isKinematic)
        {
            other.attachedRigidbody.AddForce(finalWindDir * WindForce, ForceMode.Force);
        }
    }

    // --- 新增：可视化辅助线 ---
    // 这个函数会让我在 Scene 窗口里看到风向，方便调节
    private void OnDrawGizmos()
    {
        Gizmos.color = IsOn ? Color.green : Color.red;

        // 画出风扇的范围框 (根据 Collider 大小)
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
        }

        // 画出风向箭头
        Vector3 direction = transform.TransformDirection(LocalWindDirection).normalized;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, direction * 3f);
        Gizmos.DrawSphere(transform.position + direction * 3f, 0.2f);
    }
}