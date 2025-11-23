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
    [Tooltip("植物预制体（必须包含Plant组件和碰撞箱）")]
    public GameObject plantPrefab;
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
    [Tooltip("每秒消耗肥力")]
    public float fertilityConsumptionPerSecond = 3f;
    [Tooltip("成熟后死亡返还肥力")]
    public float fertilityReturnOnMatureDeath = 100f;
    [Tooltip("未成熟死亡返还肥力倍率")]
    public float immatureDeathReturnMultiplier = 0.5f;

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