using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 动物生成条件配置
/// </summary>
[System.Serializable]
public class AnimalSpawnCondition
{
    [Header("基础信息")]
    public string animalName = "Butterfly";
    public GameObject animalPrefab;
    
    [Header("生成条件")]
    [Tooltip("需要的植物类型")]
    public PlantType requiredPlantType = PlantType.Flowering;
    [Tooltip("需要的最少植物数量")]
    public int minimumPlantCount = 3;
    
    [Header("生成限制")]
    [Tooltip("最大同时存在数量")]
    public int maxAnimalCount = 5;
    
    [Header("生成位置")]
    [Tooltip("生成高度（Y坐标）")]
    public float spawnHeight = 2f;
    
    // 运行时数据
    [System.NonSerialized]
    public List<GameObject> spawnedAnimals = new List<GameObject>();
}

/// <summary>
/// 动物生成管理器 - 根据生态条件生成动物
/// </summary>
public class AnimalSpawner : MonoBehaviour
{
    public static AnimalSpawner Instance { get; private set; }
    
    [Header("生成配置")]
    [SerializeField] private List<AnimalSpawnCondition> spawnConditions = new List<AnimalSpawnCondition>();
    
    [Header("检测间隔")]
    [SerializeField] private float checkInterval = 3f;  // 每3秒检测一次条件
    private float checkTimer = 0f;
    
    [Header("生成范围")]
    [SerializeField] private Vector2 spawnAreaCenter = Vector2.zero;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(15f, 10f);
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Update()
    {
        checkTimer += Time.deltaTime;
        
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            CheckSpawnConditions();
        }
    }
    
    /// <summary>
    /// 检查所有生成条件
    /// </summary>
    private void CheckSpawnConditions()
    {
        foreach (var condition in spawnConditions)
        {
            // 清理已销毁的动物引用
            condition.spawnedAnimals.RemoveAll(animal => animal == null);
            
            // 检查是否满足生成条件
            if (ShouldSpawnAnimal(condition))
            {
                SpawnAnimal(condition);
            }
        }
    }
    
    /// <summary>
    /// 检查是否应该生成动物
    /// </summary>
    private bool ShouldSpawnAnimal(AnimalSpawnCondition condition)
    {
        // 已达到最大数量
        if (condition.spawnedAnimals.Count >= condition.maxAnimalCount)
        {
            return false;
        }
        
        // 检查场上符合条件的植物数量
        int matchingPlantCount = CountPlantsOfType(condition.requiredPlantType);
        
        if (matchingPlantCount >= condition.minimumPlantCount)
        {
            Debug.Log($"[AnimalSpawner] {condition.animalName} 生成条件满足：{matchingPlantCount} 株 {condition.requiredPlantType} 植物");
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 统计指定类型的植物数量
    /// </summary>
    private int CountPlantsOfType(PlantType targetType)
    {
        var allPlants = EcosystemManager.Instance.GetAllPlants();
        
        int count = 0;
        foreach (var plant in allPlants)
        {
            if (plant != null && plant.plantData != null)
            {
                // 检查植物是否包含目标类型标签（支持多标签）
                if ((plant.plantData.plantType & targetType) != 0)
                {
                    count++;
                }
            }
        }
        
        return count;
    }
    
    /// <summary>
    /// 生成动物
    /// </summary>
    private void SpawnAnimal(AnimalSpawnCondition condition)
    {
        if (condition.animalPrefab == null)
        {
            Debug.LogWarning($"[AnimalSpawner] {condition.animalName} 的 Prefab 未设置！");
            return;
        }
        
        // 在生成区域内随机位置
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
        float randomY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2);
        Vector3 spawnPosition = new Vector3(randomX, condition.spawnHeight, randomY);
        
        // 生成动物
        GameObject animal = Instantiate(condition.animalPrefab, spawnPosition, Quaternion.identity, transform);
        condition.spawnedAnimals.Add(animal);
        
        // 如果有FlyingAI组件，设置飞行区域
        FlyingAI flyingAI = animal.GetComponent<FlyingAI>();
        if (flyingAI != null)
        {
            flyingAI.SetFlyArea(spawnAreaCenter, spawnAreaSize);
        }
        
        Debug.Log($"[AnimalSpawner] 生成 {condition.animalName} at {spawnPosition}，当前数量: {condition.spawnedAnimals.Count}/{condition.maxAnimalCount}");
    }
    
    // 调试可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制生成区域
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(spawnAreaCenter.x, 0, spawnAreaCenter.y), new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y));
    }
}
