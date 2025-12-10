using UnityEngine;

/// <summary>
/// 动物类型枚举
/// </summary>
public enum AnimalType
{
    Butterfly,      // 蝴蝶（授粉者）
    Bee,            // 蜜蜂（授粉者）
    Squirrel,       // 松鼠（种子传播者）
    Rabbit,         // 兔子（草食动物）
    Bird,           // 鸟类（种子传播者）
    Fox,            // 狐狸（肉食动物）
    Earthworm,      // 蚯蚓（土壤改良者）
    Dragonfly       // 蜻蜓（捕食者）
}

/// <summary>
/// 动物行为模式
/// </summary>
public enum AnimalBehaviorMode
{
    Flying,         // 飞行（使用FlyingAI）
    Walking,        // 地面行走
    Burrowing,      // 钻地（蚯蚓）
    Perching        // 栖息（鸟类，树上跳跃）
}

/// <summary>
/// 动物数据配置 - ScriptableObject
/// 定义动物的生命周期、行为参数、生成条件等
/// </summary>
[CreateAssetMenu(fileName = "NewAnimal", menuName = "Hesperia/Animal Data")]
public class AnimalData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("动物名称")]
    public string animalName = "动物";
    
    [Tooltip("动物类型")]
    public AnimalType animalType = AnimalType.Butterfly;
    
    [Tooltip("动物预制体")]
    public GameObject animalPrefab;
    
    [Tooltip("动物贴图")]
    public Sprite animalSprite;
    
    [Header("生命周期")]
    [Tooltip("最小寿命（秒）")]
    public float lifespanMin = 30f;
    
    [Tooltip("最大寿命（秒）")]
    public float lifespanMax = 60f;
    
    [Tooltip("无资源存活时间（秒）- 找不到食物/花/宿主时能活多久")]
    public float maxTimeWithoutResource = 10f;
    
    [Header("行为参数")]
    [Tooltip("行为模式")]
    public AnimalBehaviorMode behaviorMode = AnimalBehaviorMode.Flying;
    
    [Tooltip("移动速度")]
    public float moveSpeed = 2f;
    
    [Tooltip("旋转速度")]
    public float rotationSpeed = 5f;
    
    [Tooltip("检测范围（寻找目标的距离）")]
    public float detectionRange = 5f;
    
    [Tooltip("到达距离（与目标的接触距离）")]
    public float arrivalDistance = 0.3f;
    
    [Header("生成条件")]
    [Tooltip("需要的植物类型")]
    public PlantType requiredPlantType = PlantType.Flowering;
    
    [Tooltip("需要的最少植物数量")]
    public int minimumPlantCount = 3;
    
    [Tooltip("最大同时存在数量")]
    public int maxPopulation = 5;
    
    [Tooltip("生成高度（Y坐标）")]
    public float spawnHeight = 2f;
    
    [Header("交互参数")]
    [Tooltip("交互时间（授粉、进食等需要的时间）")]
    public float interactionTime = 3f;
    
    [Tooltip("交互冷却时间（对同一目标重复交互的间隔）")]
    public float interactionCooldown = 5f;
    
    [Header("粒子特效")]
    [Tooltip("交互成功特效（授粉、进食等）")]
    public GameObject interactionParticle;
    
    [Tooltip("死亡特效")]
    public GameObject deathParticle;
    
    [Header("生态效果")]
    [Tooltip("授粉效果倍率（授粉时植物繁殖数量倍数）")]
    public int pollinationMultiplier = 1;
    
    [Tooltip("是否消耗植物（草食动物会吃掉植物）")]
    public bool consumesPlants = false;
    
    [Tooltip("是否改善土壤（蚯蚓等）")]
    public bool improvesSoil = false;
    
    [Tooltip("土壤改善速率（肥力恢复速度加成）")]
    public float soilImprovementRate = 0f;
}
