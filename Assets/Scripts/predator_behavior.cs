using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 捕食者行为 - 鸟类、蜻蜓等
/// 寻找猎物并捕食
/// </summary>
public class PredatorBehavior : Animal
{
    [Header("搜索参数")]
    [SerializeField] private float searchInterval = 1.5f;
    private float searchTimer = 0f;
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;
    
    // 状态机
    private enum PredatorState
    {
        Hunting,      // 搜索猎物
        Chasing,      // 追逐猎物
        Attacking     // 攻击中
    }
    
    private PredatorState currentState = PredatorState.Hunting;
    private Animal targetPrey = null;  // 目标猎物
    private float attackTimer = 0f;
    
    // 已捕食的动物（避免重复攻击尸体）
    private HashSet<Animal> eatenPrey = new HashSet<Animal>();
    
    private FlyingAI flyingAI;
    
    protected override void Awake()
    {
        base.Awake();
        
        flyingAI = GetComponent<FlyingAI>();
        
        if (animalData.dietType != DietType.Carnivore && 
            animalData.dietType != DietType.Omnivore)
        {
            Debug.LogWarning($"[Predator] {animalData.animalName} 不是肉食/杂食动物，但挂载了PredatorBehavior！");
        }
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (IsDead) return;
        
        switch (currentState)
        {
            case PredatorState.Hunting:
                UpdateHunting();
                break;
                
            case PredatorState.Chasing:
                UpdateChasing();
                break;
                
            case PredatorState.Attacking:
                UpdateAttacking();
                break;
        }
    }
    
    /// <summary>
    /// 搜索猎物
    /// </summary>
    private void UpdateHunting()
    {
        searchTimer += Time.deltaTime;
        
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            Animal foundPrey = SearchNearbyPrey();
            
            SetResourceAvailable(foundPrey != null);
        }
        
        // 启用随机飞行
        if (flyingAI != null)
        {
            flyingAI.enabled = true;
        }
    }
    
    /// <summary>
    /// 追逐猎物
    /// </summary>
    private void UpdateChasing()
    {
        // 检查猎物是否有效
        if (targetPrey == null || targetPrey.IsDead)
        {
            if (showDebugInfo) Debug.Log("[Predator] 猎物消失，重新搜索");
            targetPrey = null;
            currentState = PredatorState.Hunting;
            SetResourceAvailable(false);
            return;
        }
        
        SetResourceAvailable(true);
        
        // 禁用随机飞行
        if (flyingAI != null)
        {
            flyingAI.enabled = false;
        }
        
        // 追逐猎物
        Vector3 targetPosition = targetPrey.transform.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        // 捕食者移动速度通常更快（可以在AnimalData里设置更高的moveSpeed）
        transform.position += direction * animalData.moveSpeed * Time.deltaTime;
        
        // 旋转朝向猎物
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, animalData.rotationSpeed * Time.deltaTime);
        }
        
        // 检查是否抓到了
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < animalData.arrivalDistance)
        {
            currentState = PredatorState.Attacking;
            attackTimer = 0f;
            if (showDebugInfo) Debug.Log($"[Predator] 抓到 {targetPrey.AnimalData.animalName}！");
        }
    }
    
    /// <summary>
    /// 攻击猎物
    /// </summary>
    private void UpdateAttacking()
    {
        if (targetPrey == null || targetPrey.IsDead)
        {
            if (showDebugInfo) Debug.Log("[Predator] 攻击中断");
            targetPrey = null;
            currentState = PredatorState.Hunting;
            SetResourceAvailable(false);
            return;
        }
        
        attackTimer += Time.deltaTime;
        
        if (attackTimer >= animalData.interactionTime)
        {
            // 捕食成功
            EatPrey(targetPrey);
            
            // 标记已捕食
            eatenPrey.Add(targetPrey);
            
            // 继续搜索下一个猎物
            targetPrey = null;
            currentState = PredatorState.Hunting;
        }
    }
    
    /// <summary>
    /// 搜索附近的猎物
    /// </summary>
    private Animal SearchNearbyPrey()
    {
        // 获取场景中所有动物
        Animal[] allAnimals = FindObjectsOfType<Animal>();
        
        Animal closestPrey = null;
        float closestDistance = animalData.detectionRange;
        
        foreach (var animal in allAnimals)
        {
            // 跳过自己
            if (animal == this) continue;
            
            // 跳过死亡的
            if (animal.IsDead) continue;
            
            // 跳过已吃过的
            if (eatenPrey.Contains(animal)) continue;
            
            // 检查是否是目标猎物类型
            if (!IsTargetPrey(animal)) continue;
            
            float distance = Vector3.Distance(transform.position, animal.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPrey = animal;
            }
        }
        
        if (closestPrey != null)
        {
            targetPrey = closestPrey;
            currentState = PredatorState.Chasing;
            if (showDebugInfo) Debug.Log($"[Predator] 发现猎物 {targetPrey.AnimalData.animalName}，距离 {closestDistance:F1}m");
        }
        
        return closestPrey;
    }
    
    /// <summary>
    /// 检查是否是目标猎物
    /// </summary>
    private bool IsTargetPrey(Animal animal)
    {
        if (animalData.preyAnimalTypes == null || animalData.preyAnimalTypes.Length == 0)
        {
            // 如果没有指定猎物类型，则攻击所有小型动物
            return true;
        }
        
        // 检查是否在猎物列表中
        return animalData.preyAnimalTypes.Contains(animal.AnimalData.animalType);
    }
    
    /// <summary>
    /// 捕食猎物
    /// </summary>
    private void EatPrey(Animal prey)
    {
        if (prey == null) return;
        
        // 播放捕食特效
        PlayInteractionEffect(prey.transform.position);
        
        // 杀死猎物
        prey.Die(false);  // false = 非自然死亡
        
        if (showDebugInfo) Debug.Log($"[Predator] 成功捕食 {prey.AnimalData.animalName}");
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // 绘制追逐路径
        if (targetPrey != null && Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPrey.transform.position);
            Gizmos.DrawWireSphere(targetPrey.transform.position, 0.5f);
        }
        
        #if UNITY_EDITOR
        if (Application.isPlaying && animalData != null)
        {
            string info = $"State: {currentState}\n" +
                         $"Target: {(targetPrey != null ? targetPrey.AnimalData.animalName : "None")}\n" +
                         $"Age: {CurrentAge:F1}/{animalData.lifespanMax:F1}s\n" +
                         $"NoPrey: {NoResourceTimer:F1}s";
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, info);
        }
        #endif
    }
}
