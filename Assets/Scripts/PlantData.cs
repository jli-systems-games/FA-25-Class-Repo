using UnityEngine;

/// <summary>
/// 植物数据配置 - ScriptableObject
/// 在Unity编辑器中创建不同植物的配置资产
/// </summary>
[CreateAssetMenu(fileName = "NewPlant", menuName = "Hesperia/Plant Data")]
public class PlantData : ScriptableObject
{
    [Header("基础信息")]
    public string plantName = "植物";
    [Tooltip("植物类型标签（可多选，用于动物检测）")]
    public PlantType plantType = PlantType.None;
    [Tooltip("植物预制体（必须包含Plant组件和碰撞箱）")]
    public GameObject plantPrefab;
    [Tooltip("卡牌UI贴图（包含图标、名字、消耗等所有信息）")]
    public Sprite cardSprite;
    [Tooltip("幼年期贴图")]
    public Sprite youngSprite;
    [Tooltip("成熟期贴图")]
    public Sprite matureSprite;
    [Tooltip("枯萎状态贴图")]
    public Sprite witherSprite;
    public int priority = 1; // 优先级：高=3, 中=2, 低=1

    [Header("库存系统")]
    [Tooltip("初始持有数量")]
    public int initialStock = 3;

    [Header("生命周期")]
    [Tooltip("成熟时间（秒）- 植物何时成熟并产生成熟效果")]
    public float lifespanMin = 5f;
    [Tooltip("最大寿命（秒）- 植物何时自然死亡")]
    public float lifespanMax = 20f;
    [Tooltip("枯萎耐受时间（秒）- 光照/肥力不足时能坚持多久")]
    public float witherTime = 5f;

    [Header("阳光需求")]
    [Tooltip("生存阳光需求 - 最小值 (0-100)")]
    public float lightRequirementMin = 50f;
    [Tooltip("生存阳光需求 - 最大值 (0-100)")]
    public float lightRequirementMax = 100f;

    [Header("阳光影响")]
    [Tooltip("对下方阳光的遮挡值 (0-100)")]
    public float shadowValue = 20f;

    [Header("肥力系统")]
    [Tooltip("幼年期每秒消耗肥力")]
    public float fertilityConsumptionPerSecond = 3f;
    [Tooltip("成熟后死亡返还肥力")]
    public float fertilityReturnOnMatureDeath = 100f;
    [Tooltip("未成熟死亡返还肥力倍率")]
    public float immatureDeathReturnMultiplier = 0.5f;

    [Header("繁殖系统")]
    [Tooltip("繁殖模式")]
    public SpreadMode spreadMode = SpreadMode.None;
    [Tooltip("繁殖生成的Prefab（如果为空则生成自己）")]
    public GameObject offspringPrefab;
    [Tooltip("繁殖数量")]
    public int spreadCount = 2;
    [Tooltip("繁殖范围（米）- 如果有SpawnArea碰撞箱则在碰撞箱内生成")]
    public float spreadRange = 2f;
    [Tooltip("繁殖间隔（秒，仅持续繁殖）")]
    public float spreadInterval = 15f;

    [Header("成熟效果")]
    [TextArea(3, 5)]
    public string matureEffectDescription = "";
    public PlantMatureEffect matureEffect;
}

/// <summary>
/// 植物成熟效果枚举
/// </summary>
public enum PlantMatureEffect
{
    None,                          // 无
    OakLeafFall,                   // 橡树落叶（提升土壤恢复速度）
    SunflowerFertilitySpread,      // 向日葵肥力扩散（死亡时返还2倍并扩散）
    FruitProduction,               // 结果植物（持续产出）
    VineAttachment                 // 爬藤植物（依附木本）
}

/// <summary>
/// 植物类型标签（可多选）
/// </summary>
[System.Flags]
public enum PlantType
{
    None = 0,
    Flowering = 1 << 0,      // 显花植物（吸引蝴蝶）
    Fruiting = 1 << 1,       // 结果植物（吸引松鼠、鸟类）
    Woody = 1 << 2,          // 木本植物（树）
    Herbaceous = 1 << 3,     // 草本植物（兔子食物）
    Fungus = 1 << 4,         // 真菌（蘑菇）
    Aquatic = 1 << 5         // 水生植物（未来扩展）
}

/// <summary>
/// 植物繁殖模式枚举
/// </summary>
public enum SpreadMode
{
    None,                    // 不繁殖
    OnMature,               // 成熟时触发一次（向日葵）
    Continuous,             // 成熟后持续繁殖（草本、蘑菇菌丝）
    OnDeath,                // 死亡时触发（暂未使用）
    RequireAnimal           // 需要动物传播（橡树+松鼠）
}