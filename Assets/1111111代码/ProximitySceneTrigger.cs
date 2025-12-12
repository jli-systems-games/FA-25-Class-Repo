using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ProximitySceneTrigger : MonoBehaviour
{
    [Header("检测设置")]
    [Tooltip("要检测的目标物体（通常是玩家）")]
    public Transform targetObject;

    [Tooltip("触发距离")]
    public float triggerDistance = 3f;

    [Header("UI设置")]
    [Tooltip("当玩家靠近时要显示的UI物体列表")]
    public List<GameObject> uiObjectsToShow = new List<GameObject>();

    [Header("场景切换设置")]
    [Tooltip("要切换到的场景名称")]
    public string targetSceneName;

    [Tooltip("按键设置")]
    public KeyCode interactKey = KeyCode.F;

    [Header("调试设置")]
    [Tooltip("是否显示调试信息")]
    public bool showDebugInfo = true;

    [Tooltip("在Scene视图中显示触发范围")]
    public bool showGizmos = true;

    private bool isPlayerInRange = false;

    void Start()
    {
        // 初始时隐藏所有UI物体
        foreach (GameObject obj in uiObjectsToShow)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // 如果没有指定目标，尝试查找玩家
        if (targetObject == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player1");
            if (player != null)
            {
                targetObject = player.transform;
            }
            else if (showDebugInfo)
            {
                Debug.LogWarning("未找到目标物体，请在Inspector中设置或给玩家添加'Player1'标签");
            }
        }
    }

    void Update()
    {
        if (targetObject == null) return;

        // 计算距离
        float distance = Vector3.Distance(transform.position, targetObject.position);

        // 检查是否在触发范围内
        if (distance <= triggerDistance)
        {
            if (!isPlayerInRange)
            {
                // 玩家进入范围
                OnPlayerEnterRange();
            }

            // 在范围内时检测按键
            if (Input.GetKeyDown(interactKey))
            {
                SwitchScene();
            }
        }
        else
        {
            if (isPlayerInRange)
            {
                // 玩家离开范围
                OnPlayerExitRange();
            }
        }
    }

    void OnPlayerEnterRange()
    {
        isPlayerInRange = true;

        if (showDebugInfo)
        {
            Debug.Log($"玩家进入触发范围！按 {interactKey} 切换场景");
        }

        // 显示UI物体
        foreach (GameObject obj in uiObjectsToShow)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    void OnPlayerExitRange()
    {
        isPlayerInRange = false;

        if (showDebugInfo)
        {
            Debug.Log("玩家离开触发范围");
        }

        // 隐藏UI物体
        foreach (GameObject obj in uiObjectsToShow)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    void SwitchScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("未设置目标场景名称！");
            return;
        }

        if (showDebugInfo)
        {
            Debug.Log($"切换到场景: {targetSceneName}");
        }

        SceneManager.LoadScene(targetSceneName);
    }

    // 公开方法：手动设置目标物体
    public void SetTargetObject(Transform target)
    {
        targetObject = target;
    }

    // 公开方法：手动设置触发距离
    public void SetTriggerDistance(float distance)
    {
        triggerDistance = distance;
    }

    // 公开方法：手动触发场景切换
    public void TriggerSceneSwitch()
    {
        SwitchScene();
    }

    // 公开方法：检查玩家是否在范围内
    public bool IsPlayerInRange()
    {
        return isPlayerInRange;
    }

    // 在Scene视图中可视化触发范围
    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // 绘制触发范围
        Gizmos.color = isPlayerInRange ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);

        // 如果在游戏运行中且目标存在，绘制连线
        if (Application.isPlaying && targetObject != null)
        {
            float distance = Vector3.Distance(transform.position, targetObject.position);
            Gizmos.color = distance <= triggerDistance ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, targetObject.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // 当选中时绘制更清晰的范围
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, triggerDistance);
    }
}