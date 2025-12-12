using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 简单的物体监测器
/// 当列表中的物体全部被销毁后，启用指定的GameObject
/// </summary>
public class SimpleObjectWatcher : MonoBehaviour
{
    [Header("监测设置")]
    [Tooltip("要监测的物体列表")]
    public List<GameObject> watchedObjects = new List<GameObject>();

    [Header("启用设置")]
    [Tooltip("当监测列表全部销毁后，要启用的物体")]
    public List<GameObject> objectsToEnable = new List<GameObject>();

    [Header("检测设置")]
    [Tooltip("检测间隔（秒）")]
    public float checkInterval = 0.5f;

    [Header("调试")]
    public bool showDebugInfo = true;

    private float checkTimer;
    private bool hasTriggered = false;

    void Start()
    {
        checkTimer = checkInterval;

        if (showDebugInfo)
        {
            Debug.Log($"开始监测 {watchedObjects.Count} 个物体");
        }
    }

    void Update()
    {
        if (hasTriggered) return;

        checkTimer -= Time.deltaTime;

        if (checkTimer <= 0f)
        {
            CheckObjects();
            checkTimer = checkInterval;
        }
    }

    void CheckObjects()
    {
        // 检查是否所有物体都被销毁了
        bool allDestroyed = true;

        foreach (GameObject obj in watchedObjects)
        {
            if (obj != null)
            {
                allDestroyed = false;
                break;
            }
        }

        // 如果全部销毁，启用目标物体
        if (allDestroyed && watchedObjects.Count > 0)
        {
            if (showDebugInfo)
            {
                Debug.Log("监测列表全部销毁！启用目标物体...");
            }

            foreach (GameObject obj in objectsToEnable)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    if (showDebugInfo)
                    {
                        Debug.Log($"✅ 启用了: {obj.name}");
                    }
                }
            }

            hasTriggered = true;
        }
    }
}
