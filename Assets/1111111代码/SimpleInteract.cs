using UnityEngine;
using UnityEngine.Events; // 引入事件命名空间

public class SimpleInteractor : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("你要检测的目标物体")]
    public Transform targetObject; // 目标物体

    [Tooltip("触发距离 (默认20)")]
    public float detectRange = 20f; // 检测距离

    [Header("按F后发生的事情")]
    public UnityEvent onInteract; // 在这里拖入你想启用的一系列东西

    void Update()
    {
        // 1. 如果没有设置目标，直接跳过，防止报错
        if (targetObject == null) return;

        // 2. 计算当前物体和目标物体的距离
        float distance = Vector3.Distance(transform.position, targetObject.position);

        // 3. 判断：如果距离小于设定值 并且 按下了F键
        if (distance <= detectRange && Input.GetKeyDown(KeyCode.F))
        {
            // 4. 执行所有你在编辑器里绑定的事件
            Debug.Log("交互成功！启用了一系列东西。");
            onInteract.Invoke();
        }
    }

    // --- 辅助功能：在编辑器里画个圈，方便你看范围 ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}