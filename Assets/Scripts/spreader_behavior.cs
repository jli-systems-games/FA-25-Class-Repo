using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 通用传播者行为 - 蝴蝶、松鼠、鸟类等
/// 寻找需要传播的植物，触发繁殖
/// </summary>
public class SpreaderBehavior : Animal
{
    [Header("搜索参数")]
    [SerializeField] private float searchInterval = 1.5f;  // 搜索间隔
    private float searchTimer = 0f;
    
    [Header("传播参数")]
    [SerializeField] private int spreadMultiplier = 1;  // 传播倍率（每次传播生成几株）
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;
    
    // 状态机
    private enum SpreaderState
    {
        Searching,    // 搜索目标植物
        MovingTo,     // 移动到目标
        Spreading     // 传播中
    }
    
    private SpreaderState currentState = SpreaderState.Searching;
    private Plant targetPlant = null;
    private float spreadTimer = 0f;
    
    // 已传播的植物（避免重复）
    private HashSet<Plant> spreadPlants = new HashSet<Plant>();
    
    private FlyingAI flyingAI;
    
    protected override void Awake()
    {
        base.Awake();
        
        flyingAI = GetComponent<FlyingAI>();
        
        // 从 AnimalData 读取传播倍率（如果有）
        if (animalData != null && animalData.pollinationMultiplier > 0)
        {
            spreadMultiplier = animalData.pollinationMultiplier;
        }
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (IsDead) return;
        
        switch (currentState)
        {
            case SpreaderState.Searching:
                UpdateSearching();
                break;
                
            case SpreaderState.MovingTo:
                UpdateMovingTo();
                break;
                
            case SpreaderState.Spreading:
                UpdateSpreading();
                break;
        }
    }
    
    /// <summary>
    /// 搜索状态
    /// </summary>
    private void UpdateSearching()
    {
        searchTimer += Time.deltaTime;
        
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            Plant foundPlant = SearchNearbySpreadablePlant();
            
            SetResourceAvailable(foundPlant != null);
        }
        
        // 启用随机移动
        if (flyingAI != null)
        {
            flyingAI.enabled = true;
        }
    }
    
    /// <summary>
    /// 移动到目标
    /// </summary>
    private void UpdateMovingTo()
    {
        // 检查目标是否有效
        if (targetPlant == null || targetPlant.IsDead || !targetPlant.IsMature)
        {
            if (showDebugInfo) Debug.Log("[Spreader] 目标植物消失");
            targetPlant = null;
            currentState = SpreaderState.Searching;
            SetResourceAvailable(false);
            return;
        }
        
        SetResourceAvailable(true);
        
        // 禁用随机移动
        if (flyingAI != null)
        {
            flyingAI.enabled = false;
        }
        
        // 移动到目标
        Vector3 targetPosition = targetPlant.GetPollinationPoint();
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * animalData.moveSpeed * Time.deltaTime;
        
        // 旋转朝向
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
            currentState = SpreaderState.Spreading;
            spreadTimer = 0f;
            if (showDebugInfo) Debug.Log($"[Spreader] 到达 {targetPlant.plantData.plantName}");
        }
    }
    
    /// <summary>
    /// 传播中
    /// </summary>
    private void UpdateSpreading()
    {
        if (targetPlant == null || targetPlant.IsDead)
        {
            if (showDebugInfo) Debug.Log("[Spreader] 传播中断");
            targetPlant = null;
            currentState = SpreaderState.Searching;
            SetResourceAvailable(false);
            return;
        }
        
        spreadTimer += Time.deltaTime;
        
        if (spreadTimer >= animalData.interactionTime)
        {
            // 触发传播
            SpreadPlant(targetPlant);
            
            // 标记已传播
            spreadPlants.Add(targetPlant);
            
            // 继续搜索
            targetPlant = null;
            currentState = SpreaderState.Searching;
        }
    }
    
    /// <summary>
    /// 搜索附近可传播的植物
    /// 匹配条件：成熟 + 需要的植物类型 + RequireAnimal繁殖模式
    /// </summary>
    private Plant SearchNearbySpreadablePlant()
    {
        var allPlants = EcosystemManager.Instance.GetAllPlants();
        
        Plant closestPlant = null;
        float closestDistance = animalData.detectionRange;
        
        foreach (var plant in allPlants)
        {
            if (plant == null || plant.IsDead || !plant.IsMature) continue;
            
            // 检查是否需要动物传播
            if (plant.plantData.spreadMode != SpreadMode.RequireAnimal) continue;
            
            // 检查植物类型是否匹配（AnimalData里配置的requiredPlantType）
            if ((plant.plantData.plantType & animalData.requiredPlantType) == 0) continue;
            
            // 跳过已传播的
            if (spreadPlants.Contains(plant)) continue;
            
            float distance = Vector3.Distance(transform.position, plant.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlant = plant;
            }
        }
        
        if (closestPlant != null)
        {
            targetPlant = closestPlant;
            currentState = SpreaderState.MovingTo;
            if (showDebugInfo) Debug.Log($"[Spreader] 发现可传播植物 {targetPlant.plantData.plantName}");
        }
        
        return closestPlant;
    }
    
    /// <summary>
    /// 传播植物（生成幼苗）
    /// </summary>
    private void SpreadPlant(Plant plant)
    {
        // 播放特效
        PlayInteractionEffect(plant.GetPollinationPoint());
        
        // 确定生成的Prefab
        GameObject prefabToSpawn = plant.plantData.offspringPrefab != null ? 
            plant.plantData.offspringPrefab : plant.plantData.plantPrefab;
        
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[Spreader] {plant.plantData.plantName} 没有繁殖Prefab");
            return;
        }
        
        int successCount = 0;
        
        // 生成多株（根据倍率）
        for (int i = 0; i < spreadMultiplier; i++)
        {
            // 检查植物总数上限
            if (EcosystemManager.Instance.CurrentPlantCount >= EcosystemManager.Instance.MaxPlantCount)
            {
                if (showDebugInfo) Debug.LogWarning($"[Spreader] 植物数量已达上限，已生成 {successCount}/{spreadMultiplier}");
                break;
            }
            
            // 在植物附近随机位置生成
            float spreadRange = plant.plantData.spreadRange > 0 ? plant.plantData.spreadRange : 2f;
            Vector2 randomOffset = Random.insideUnitCircle * spreadRange;
            Vector3 spawnPosition = plant.transform.position + new Vector3(randomOffset.x, 0f, randomOffset.y);
            spawnPosition.y = plant.transform.position.y;
            
            GameObject offspring = Instantiate(
                prefabToSpawn,
                spawnPosition,
                Quaternion.identity,
                plant.transform.parent
            );
            
            successCount++;
        }
        
        if (showDebugInfo) Debug.Log($"[Spreader] 传播成功！{plant.plantData.plantName} 生成 {successCount} 株幼苗");
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // 绘制目标连线
        if (targetPlant != null && Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetPlant.transform.position);
            Gizmos.DrawWireSphere(targetPlant.transform.position, 0.5f);
        }
        
        #if UNITY_EDITOR
        if (Application.isPlaying && animalData != null)
        {
            string info = $"State: {currentState}\n" +
                         $"Target: {(targetPlant != null ? targetPlant.plantData.plantName : "None")}\n" +
                         $"Age: {CurrentAge:F1}/{animalData.lifespanMax:F1}s\n" +
                         $"NoResource: {NoResourceTimer:F1}s";
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, info);
        }
        #endif
    }
}
