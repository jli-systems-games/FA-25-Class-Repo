using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 草食动物行为 - 兔子、鹿等
/// 寻找植物并啃食
/// </summary>
public class HerbivoreBehavior : Animal
{
    [Header("搜索参数")]
    [SerializeField] private float searchInterval = 1.5f;
    private float searchTimer = 0f;
    
    [Header("进食参数")]
    [SerializeField] private bool consumePlant = true;  // 是否吃掉整株植物
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;
    
    // 状态机
    private enum HerbivoreState
    {
        Searching,    // 搜索食物
        MovingTo,     // 移动到食物
        Eating        // 进食中
    }
    
    private HerbivoreState currentState = HerbivoreState.Searching;
    private Plant targetPlant = null;
    private float eatTimer = 0f;
    
    // 已吃过的植物（避免重复吃残骸）
    private HashSet<Plant> eatenPlants = new HashSet<Plant>();
    
    private FlyingAI flyingAI;  // 如果是飞行动物（鸟）
    
    protected override void Awake()
    {
        base.Awake();
        
        flyingAI = GetComponent<FlyingAI>();
        
        // 从 AnimalData 读取配置
        if (animalData != null)
        {
            consumePlant = animalData.consumesPlants;
        }
        
        if (animalData != null && 
            animalData.dietType != DietType.Herbivore && 
            animalData.dietType != DietType.Omnivore)
        {
            Debug.LogWarning($"[Herbivore] {animalData.animalName} 不是草食/杂食动物，但挂载了HerbivoreBehavior！");
        }
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (IsDead) return;
        
        switch (currentState)
        {
            case HerbivoreState.Searching:
                UpdateSearching();
                break;
                
            case HerbivoreState.MovingTo:
                UpdateMovingTo();
                break;
                
            case HerbivoreState.Eating:
                UpdateEating();
                break;
        }
    }
    
    /// <summary>
    /// 搜索食物
    /// </summary>
    private void UpdateSearching()
    {
        searchTimer += Time.deltaTime;
        
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            Plant foundPlant = SearchNearbyFood();
            
            SetResourceAvailable(foundPlant != null);
        }
        
        // 启用随机移动
        if (flyingAI != null)
        {
            flyingAI.enabled = true;
        }
    }
    
    /// <summary>
    /// 移动到食物
    /// </summary>
    private void UpdateMovingTo()
    {
        // 检查植物是否有效
        if (targetPlant == null || targetPlant.IsDead)
        {
            if (showDebugInfo) Debug.Log("[Herbivore] 食物消失，重新搜索");
            targetPlant = null;
            currentState = HerbivoreState.Searching;
            SetResourceAvailable(false);
            return;
        }
        
        SetResourceAvailable(true);
        
        // 禁用随机移动
        if (flyingAI != null)
        {
            flyingAI.enabled = false;
        }
        
        // 移动到目标植物
        Vector3 targetPosition = targetPlant.transform.position;
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
            currentState = HerbivoreState.Eating;
            eatTimer = 0f;
            if (showDebugInfo) Debug.Log($"[Herbivore] 到达 {targetPlant.plantData.plantName}，开始进食");
        }
    }
    
    /// <summary>
    /// 进食中
    /// </summary>
    private void UpdateEating()
    {
        if (targetPlant == null || targetPlant.IsDead)
        {
            if (showDebugInfo) Debug.Log("[Herbivore] 进食中断");
            targetPlant = null;
            currentState = HerbivoreState.Searching;
            SetResourceAvailable(false);
            return;
        }
        
        eatTimer += Time.deltaTime;
        
        if (eatTimer >= animalData.interactionTime)
        {
            // 进食完成
            EatPlant(targetPlant);
            
            // 标记已吃
            eatenPlants.Add(targetPlant);
            
            // 继续搜索下一株
            targetPlant = null;
            currentState = HerbivoreState.Searching;
        }
    }
    
    /// <summary>
    /// 搜索附近可食用的植物
    /// </summary>
    private Plant SearchNearbyFood()
    {
        var allPlants = EcosystemManager.Instance.GetAllPlants();
        
        Plant closestPlant = null;
        float closestDistance = animalData.detectionRange;
        
        foreach (var plant in allPlants)
        {
            if (plant == null || plant.IsDead) continue;
            
            // 检查植物类型是否匹配
            if ((plant.plantData.plantType & animalData.requiredPlantType) == 0) continue;
            
            // 跳过已吃过的
            if (eatenPlants.Contains(plant)) continue;
            
            // 🆕 可选：只吃幼年植物或只吃成熟植物
            // if (!plant.IsMature) continue;  // 只吃嫩叶
            // if (plant.IsMature) continue;   // 只吃幼苗
            
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
            currentState = HerbivoreState.MovingTo;
            if (showDebugInfo) Debug.Log($"[Herbivore] 发现食物 {targetPlant.plantData.plantName}");
        }
        
        return closestPlant;
    }
    
    /// <summary>
    /// 吃植物
    /// </summary>
    private void EatPlant(Plant plant)
    {
        if (plant == null) return;
        
        // 播放进食特效
        PlayInteractionEffect(plant.transform.position);
        
        if (consumePlant)
        {
            // 🍽️ 吃掉整株植物（调用Harvest，会返还肥力）
            plant.Harvest();
            
            if (showDebugInfo) Debug.Log($"[Herbivore] 吃掉了 {plant.plantData.plantName}");
        }
        else
        {
            // 🌿 只是啃食，不杀死植物（可以造成枯萎伤害）
            // TODO: 可以给Plant添加一个"受到伤害"的方法
            
            if (showDebugInfo) Debug.Log($"[Herbivore] 啃食了 {plant.plantData.plantName}");
        }
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // 绘制目标连线
        if (targetPlant != null && Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetPlant.transform.position);
            Gizmos.DrawWireSphere(targetPlant.transform.position, 0.5f);
        }
        
        #if UNITY_EDITOR
        if (Application.isPlaying && animalData != null)
        {
            string info = $"State: {currentState}\n" +
                         $"Target: {(targetPlant != null ? targetPlant.plantData.plantName : "None")}\n" +
                         $"Age: {CurrentAge:F1}/{animalData.lifespanMax:F1}s\n" +
                         $"NoFood: {NoResourceTimer:F1}s";
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, info);
        }
        #endif
    }
}
