using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 蝴蝶授粉行为 - 寻找显花植物并授粉
/// 继承Animal基类，获得生命周期管理
/// </summary>
public class ButterflyBehavior : Animal
{
    [Header("搜索参数")]
    [SerializeField] private float searchInterval = 1f;  // 搜索间隔（秒）
    private float searchTimer = 0f;

    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;

    // 状态
    private enum ButterflyState
    {
        SearchingFlower,  // 搜索花
        FlyingToFlower,   // 飞向花
        Pollinating       // 授粉中
    }

    private ButterflyState currentState = ButterflyState.SearchingFlower;
    private Plant targetFlower = null;
    private float pollinationTimer = 0f;

    // 已授粉的花（避免重复授粉）
    private HashSet<Plant> pollinatedFlowers = new HashSet<Plant>();

    private FlyingAI flyingAI;

    protected override void Awake()
    {
        base.Awake();  // 调用Animal的Awake

        flyingAI = GetComponent<FlyingAI>();
        if (flyingAI == null)
        {
            Debug.LogError("[ButterflyBehavior] 缺少 FlyingAI 组件！");
        }
    }

    protected override void Update()
    {
        base.Update();  // 调用Animal的Update（处理生命周期）

        if (IsDead) return;

        switch (currentState)
        {
            case ButterflyState.SearchingFlower:
                UpdateSearching();
                break;

            case ButterflyState.FlyingToFlower:
                UpdateFlyingToFlower();
                break;

            case ButterflyState.Pollinating:
                UpdatePollinating();
                break;
        }
    }

    /// <summary>
    /// 搜索花的状态
    /// </summary>
    private void UpdateSearching()
    {
        // 定期搜索附近的花
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            Plant foundFlower = SearchNearbyFlower();

            // 告诉基类是否找到资源
            SetResourceAvailable(foundFlower != null);
        }

        // 启用随机飞行
        if (flyingAI != null)
        {
            flyingAI.enabled = true;
        }
    }

    /// <summary>
    /// 飞向花的状态
    /// </summary>
    private void UpdateFlyingToFlower()
    {
        // 检查目标花是否还有效
        if (targetFlower == null || targetFlower.IsDead)
        {
            if (showDebugInfo) Debug.Log("[Butterfly] 目标花消失，重新搜索");
            targetFlower = null;
            currentState = ButterflyState.SearchingFlower;
            SetResourceAvailable(false);  // 失去资源
            return;
        }

        // 通知基类有资源
        SetResourceAvailable(true);

        // 禁用随机飞行，手动控制
        if (flyingAI != null)
        {
            flyingAI.enabled = false;
        }

        // 飞向目标花的授粉点（而不是根部）
        Vector3 targetPosition = targetFlower.GetPollinationPoint();
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * animalData.moveSpeed * Time.deltaTime;

        // 旋转朝向花
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, animalData.rotationSpeed * Time.deltaTime);
        }

        // 检查是否到达
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < animalData.arrivalDistance)
        {
            // 开始授粉
            currentState = ButterflyState.Pollinating;
            pollinationTimer = 0f;
            if (showDebugInfo) Debug.Log($"[Butterfly] 到达花朵 {targetFlower.plantData.plantName}，开始授粉");
        }
    }

    /// <summary>
    /// 授粉状态
    /// </summary>
    private void UpdatePollinating()
    {
        // 检查目标花是否还有效
        if (targetFlower == null || targetFlower.IsDead)
        {
            if (showDebugInfo) Debug.Log("[Butterfly] 授粉中断，花消失");
            targetFlower = null;
            currentState = ButterflyState.SearchingFlower;
            SetResourceAvailable(false);
            return;
        }

        // 累积授粉时间
        pollinationTimer += Time.deltaTime;

        // 授粉完成
        if (pollinationTimer >= animalData.interactionTime)
        {
            PollinateFlower();

            // 标记为已授粉
            pollinatedFlowers.Add(targetFlower);

            // 继续搜索下一朵花
            targetFlower = null;
            currentState = ButterflyState.SearchingFlower;
        }
    }

    /// <summary>
    /// 搜索附近未授粉的成熟花
    /// </summary>
    private Plant SearchNearbyFlower()
    {
        var allPlants = EcosystemManager.Instance.GetAllPlants();

        Plant closestFlower = null;
        float closestDistance = animalData.detectionRange;

        foreach (var plant in allPlants)
        {
            // 检查是否是成熟的显花植物
            if (plant != null && !plant.IsDead && plant.IsMature &&
                (plant.plantData.plantType & PlantType.Flowering) != 0)
            {
                // 跳过已授粉的花
                if (pollinatedFlowers.Contains(plant))
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, plant.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFlower = plant;
                }
            }
        }

        if (closestFlower != null)
        {
            targetFlower = closestFlower;
            currentState = ButterflyState.FlyingToFlower;
            if (showDebugInfo) Debug.Log($"[Butterfly] 发现成熟花朵 {targetFlower.plantData.plantName}，距离 {closestDistance:F1}m");
        }

        return closestFlower;
    }

    /// <summary>
    /// 授粉（触发花的繁殖）
    /// </summary>
    private void PollinateFlower()
    {
        if (targetFlower == null) return;

        // 播放授粉特效
        PlayInteractionEffect(targetFlower.GetPollinationPoint());

        // 检查植物是否有繁殖能力
        if (targetFlower.plantData.spreadMode == SpreadMode.None)
        {
            if (showDebugInfo) Debug.LogWarning($"[Butterfly] {targetFlower.plantData.plantName} 没有繁殖能力");
            return;
        }

        // 强制触发繁殖（次数由AnimalData的pollinationMultiplier决定）
        for (int i = 0; i < animalData.pollinationMultiplier; i++)
        {
            TriggerFlowerReproduction(targetFlower);
        }

        if (showDebugInfo) Debug.Log($"[Butterfly] 授粉成功！{targetFlower.plantData.plantName} 繁殖 {animalData.pollinationMultiplier} 次");
    }

    /// <summary>
    /// 触发花的繁殖（使用反射调用Plant的私有方法）
    /// </summary>
    private void TriggerFlowerReproduction(Plant flower)
    {
        // 确定要生成的Prefab
        GameObject prefabToSpawn = flower.plantData.offspringPrefab != null ?
            flower.plantData.offspringPrefab : flower.plantData.plantPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[Butterfly] {flower.plantData.plantName} 没有设置繁殖Prefab");
            return;
        }

        // 检查植物总数上限
        if (EcosystemManager.Instance.CurrentPlantCount >= EcosystemManager.Instance.MaxPlantCount)
        {
            if (showDebugInfo) Debug.LogWarning("[Butterfly] 植物数量已达上限，无法授粉繁殖");
            return;
        }

        // 在花的附近生成新植物
        Vector2 randomOffset = Random.insideUnitCircle * (flower.plantData.spreadRange > 0 ? flower.plantData.spreadRange : 1f);
        Vector3 spawnPosition = flower.transform.position + new Vector3(randomOffset.x, 0f, randomOffset.y);
        spawnPosition.y = flower.transform.position.y;

        GameObject offspring = Instantiate(
            prefabToSpawn,
            spawnPosition,
            Quaternion.identity,
            flower.transform.parent
        );

        if (showDebugInfo) Debug.Log($"[Butterfly] 授粉繁殖：{flower.plantData.plantName} 在 {spawnPosition} 生成幼苗");
    }

    // 调试可视化（覆盖基类实现，添加蝴蝶特定的可视化）
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();  // 绘制检测范围

        // 绘制目标花的授粉点和路径
        if (targetFlower != null && Application.isPlaying)
        {
            Vector3 pollinationPoint = targetFlower.GetPollinationPoint();

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, pollinationPoint);
            Gizmos.DrawWireSphere(pollinationPoint, 0.3f);

            // 绘制授粉点标记
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pollinationPoint, 0.1f);
        }

        // 显示状态文字
#if UNITY_EDITOR
        if (Application.isPlaying && animalData != null)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f,
                $"State: {currentState}\nAge: {CurrentAge:F1}/{animalData.lifespanMax:F1}s\nNoFlower: {NoResourceTimer:F1}/{animalData.maxTimeWithoutResource:F1}s\nPollination: {pollinationTimer:F1}s");
        }
#endif
    }
}