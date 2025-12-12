using UnityEngine;
using System.Collections.Generic;

public class ObjectListWatcher : MonoBehaviour
{
    [Header("监测设置")]
    [Tooltip("要监测的物体列表")]
    public List<GameObject> watchedObjects = new List<GameObject>();

    [Header("激活设置")]
    [Tooltip("当监测列表全部销毁后，要激活的物体列表")]
    public List<GameObject> objectsToEnable = new List<GameObject>();

    [Header("禁用设置")]
    [Tooltip("当监测列表全部销毁后，要禁用的物体列表")]
    public List<GameObject> objectsToDisable = new List<GameObject>();

    [Header("生成设置")]
    [Tooltip("当监测物体被销毁时，在其位置生成的prefab")]
    public GameObject spawnPrefab;

    [Tooltip("是否在物体销毁时生成prefab")]
    public bool spawnOnDestroy = true;

    [Tooltip("生成位置偏移")]
    public Vector3 spawnOffset = Vector3.zero;

    [Header("检测设置")]
    [Tooltip("检测间隔（秒）")]
    public float checkInterval = 0.5f;

    [Tooltip("开始检测前的延迟")]
    public float startDelay = 0f;

    [Header("调试信息")]
    [Tooltip("是否显示调试信息")]
    public bool showDebugInfo = true;

    private float checkTimer;
    private bool hasTriggered = false;
    private bool isActive = false;

    // 用于记录上一帧每个物体的存活状态和位置
    private Dictionary<GameObject, Vector3> lastKnownPositions = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        checkTimer = checkInterval;

        // 记录所有物体的初始位置
        foreach (GameObject obj in watchedObjects)
        {
            if (obj != null)
            {
                lastKnownPositions[obj] = obj.transform.position;
            }
        }

        if (startDelay > 0)
        {
            Invoke("ActivateWatcher", startDelay);
        }
        else
        {
            isActive = true;
        }
    }

    void Update()
    {
        if (!isActive || hasTriggered) return;

        // 更新存活物体的位置
        foreach (GameObject obj in watchedObjects)
        {
            if (obj != null)
            {
                lastKnownPositions[obj] = obj.transform.position;
            }
        }

        checkTimer -= Time.deltaTime;

        if (checkTimer <= 0f)
        {
            CheckWatchedObjects();
            checkTimer = checkInterval;
        }
    }

    void CheckWatchedObjects()
    {
        // 检查监测列表中是否还有存活的物体
        bool allDestroyed = true;
        int destroyedCount = 0;

        // 创建一个临时列表来记录新发现被销毁的物体
        List<GameObject> newlyDestroyed = new List<GameObject>();

        foreach (GameObject obj in watchedObjects)
        {
            if (obj != null)
            {
                allDestroyed = false;
            }
            else
            {
                destroyedCount++;

                // 如果这个物体之前存在位置记录，说明是新销毁的
                if (lastKnownPositions.ContainsKey(obj))
                {
                    newlyDestroyed.Add(obj);
                }
            }
        }

        // 处理新销毁的物体
        if (spawnOnDestroy && spawnPrefab != null)
        {
            foreach (GameObject destroyedObj in newlyDestroyed)
            {
                Vector3 spawnPosition = lastKnownPositions[destroyedObj] + spawnOffset;
                GameObject spawned = Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);

                if (showDebugInfo)
                {
                    Debug.Log($"物体 {destroyedObj} 被销毁，在位置 {spawnPosition} 生成了 {spawnPrefab.name}");
                }

                // 从位置记录中移除
                lastKnownPositions.Remove(destroyedObj);
            }
        }

        if (showDebugInfo)
        {
            Debug.Log($"监测列表状态: {destroyedCount}/{watchedObjects.Count} 已销毁");
        }

        // 如果全部销毁，触发激活/禁用
        if (allDestroyed && watchedObjects.Count > 0)
        {
            TriggerListChanges();
        }
    }

    void TriggerListChanges()
    {
        hasTriggered = true;

        if (showDebugInfo)
        {
            Debug.Log("监测列表全部销毁！触发列表变更...");
        }

        // 激活目标列表
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                if (showDebugInfo)
                {
                    Debug.Log($"激活物体: {obj.name}");
                }
            }
        }

        // 禁用目标列表
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                if (showDebugInfo)
                {
                    Debug.Log($"禁用物体: {obj.name}");
                }
            }
        }
    }

    void ActivateWatcher()
    {
        isActive = true;
        if (showDebugInfo)
        {
            Debug.Log("开始监测物体列表");
        }
    }

    // 公开方法：手动添加监测物体
    public void AddWatchedObject(GameObject obj)
    {
        if (!watchedObjects.Contains(obj))
        {
            watchedObjects.Add(obj);
            if (obj != null)
            {
                lastKnownPositions[obj] = obj.transform.position;
            }
        }
    }

    // 公开方法：手动移除监测物体
    public void RemoveWatchedObject(GameObject obj)
    {
        watchedObjects.Remove(obj);
        lastKnownPositions.Remove(obj);
    }

    // 公开方法：重置监测器
    public void ResetWatcher()
    {
        hasTriggered = false;
        checkTimer = checkInterval;
        lastKnownPositions.Clear();

        // 重新记录所有物体的位置
        foreach (GameObject obj in watchedObjects)
        {
            if (obj != null)
            {
                lastKnownPositions[obj] = obj.transform.position;
            }
        }
    }

    // 公开方法：立即检查
    public void CheckNow()
    {
        CheckWatchedObjects();
    }

    // 公开方法：获取剩余物体数量
    public int GetRemainingObjectCount()
    {
        int count = 0;
        foreach (GameObject obj in watchedObjects)
        {
            if (obj != null)
            {
                count++;
            }
        }
        return count;
    }
}