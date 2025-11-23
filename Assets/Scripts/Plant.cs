using UnityEngine;

/// <summary>
/// 植物核心脚本
/// 处理生命周期、阳光需求、肥力消耗、成熟效果
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Plant : MonoBehaviour
{
    [Header("植物配置")]
    [SerializeField] private PlantData plantData;

    [Header("碰撞箱引用")]
    [SerializeField] private BoxCollider2D lightSensor;      // 光照检测箱
    [SerializeField] private BoxCollider2D shadowCollider;   // 阴影碰撞箱（整体）
    private SpriteRenderer lightSensorRenderer;              // 检测器的视觉反馈

    [Header("检测器颜色")]
    [SerializeField] private Color sensorGoodColor = Color.green;    // 光照达标颜色
    [SerializeField] private Color sensorBadColor = Color.red;       // 光照不达标颜色

    [Header("生命状态")]
    private float currentAge = 0f;
    private float maturityTime;  // 成熟时间
    private float deathTime;     // 死亡时间
    private bool isMature = false;
    private bool isDead = false;

    [Header("枯萎状态")]
    private bool isWithering = false;     // 是否正在枯萎
    private float witherTimer = 0f;       // 枯萎计时器

    [Header("阳光状态")]
    private float currentLight = 100f;
    private bool hasEnoughLight = true;

    [Header("肥力状态")]
    private bool hasEnoughFertility = true;

    [Header("视觉")]
    private SpriteRenderer spriteRenderer;
    private Color normalColor = Color.white;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (plantData != null)
        {
            spriteRenderer.sprite = plantData.youngSprite;  // 初始使用幼年贴图
            maturityTime = plantData.lifespanMin;  // 成熟时间
            deathTime = plantData.lifespanMax;      // 死亡时间
        }
    }

    private void Start()
    {
        // 注册到生态系统
        EcosystemManager.Instance.RegisterPlant(this);

        // 验证碰撞箱是否已挂载
        if (lightSensor == null || shadowCollider == null)
        {
            Debug.LogError($"[Plant] {plantData.plantName} 碰撞箱未正确挂载！请在Prefab上预先添加LightSensor和ShadowCollider");
        }

        // 获取LightSensor的SpriteRenderer用于显示光照状态
        if (lightSensor != null)
        {
            lightSensorRenderer = lightSensor.GetComponent<SpriteRenderer>();
            if (lightSensorRenderer == null)
            {
                Debug.LogWarning($"[Plant] {plantData.plantName} 的LightSensor缺少SpriteRenderer，无法显示光照状态");
            }
        }

        Debug.Log($"[Plant] {plantData.plantName} 种植成功，成熟时间: {maturityTime:F1}秒，死亡时间: {deathTime:F1}秒");
    }

    private void Update()
    {
        if (isDead) return;

        // 更新年龄
        currentAge += Time.deltaTime;

        // 检查是否成熟
        if (!isMature && currentAge >= maturityTime)
        {
            OnMatured();
        }

        // 检查阳光需求
        CheckLightRequirement();

        // 检查肥力需求
        CheckFertilityRequirement();

        // 检查枯萎状态
        CheckWitherState();

        // 检查是否寿终正寝
        if (currentAge >= deathTime)
        {
            Die(true); // 自然死亡
        }
    }

    /// <summary>
    /// 检查肥力需求
    /// </summary>
    private void CheckFertilityRequirement()
    {
        // 检查当前肥力是否足够支撑植物消耗
        float currentFertility = EcosystemManager.Instance.CurrentFertility;
        float consumption = plantData.fertilityConsumptionPerSecond;

        hasEnoughFertility = currentFertility >= consumption * Time.deltaTime;
    }

    /// <summary>
    /// 检查枯萎状态
    /// </summary>
    private void CheckWitherState()
    {
        // 判断是否应该枯萎（光照或肥力不足）
        bool shouldWither = !hasEnoughLight || !hasEnoughFertility;

        if (shouldWither)
        {
            if (!isWithering)
            {
                // 开始枯萎
                isWithering = true;
                witherTimer = 0f;
                spriteRenderer.sprite = plantData.witherSprite;
                Debug.Log($"[Plant] {plantData.plantName} 开始枯萎（光照:{hasEnoughLight}, 肥力:{hasEnoughFertility}）");
            }

            // 累积枯萎时间
            witherTimer += Time.deltaTime;

            // 超过枯萎耐受时间，死亡
            if (witherTimer >= plantData.witherTime)
            {
                Die(false); // 枯萎死亡
            }
        }
        else
        {
            if (isWithering)
            {
                // 恢复正常
                isWithering = false;
                witherTimer = 0f;
                // 恢复对应状态的贴图
                spriteRenderer.sprite = isMature ? plantData.matureSprite : plantData.youngSprite;
                Debug.Log($"[Plant] {plantData.plantName} 恢复正常");
            }
        }
    }

    /// <summary>
    /// 检查阳光需求
    /// </summary>
    private void CheckLightRequirement()
    {
        // 使用LightSensor碰撞箱进行区域检测
        currentLight = LightingSystem.Instance.GetLightAtCollider(lightSensor);

        // 判断阳光是否满足需求
        hasEnoughLight = currentLight >= plantData.lightRequirementMin &&
                         currentLight <= plantData.lightRequirementMax;

        // 更新LightSensor的颜色反馈
        if (lightSensorRenderer != null)
        {
            lightSensorRenderer.color = hasEnoughLight ? sensorGoodColor : sensorBadColor;
        }
    }

    /// <summary>
    /// 植物成熟时触发
    /// </summary>
    private void OnMatured()
    {
        isMature = true;

        // 切换到成熟贴图
        if (plantData.matureSprite != null)
        {
            spriteRenderer.sprite = plantData.matureSprite;
        }

        Debug.Log($"[Plant] {plantData.plantName} 已成熟！");

        // 触发特定成熟效果
        switch (plantData.matureEffect)
        {
            case PlantMatureEffect.OakLeafFall:
                // 橡树落叶效果：通知生态系统增加恢复速率
                EcosystemManager.Instance.OnOakMatured();
                break;

            case PlantMatureEffect.SunflowerFertilitySpread:
                // 向日葵在死亡时触发，这里不处理
                break;

            case PlantMatureEffect.FruitProduction:
                // 结果植物：可以添加持续产出逻辑
                break;
        }
    }

    /// <summary>
    /// 植物死亡
    /// </summary>
    private void Die(bool isNaturalDeath)
    {
        if (isDead) return;

        isDead = true;

        // 计算返还肥力
        float returnFertility = isMature ?
            plantData.fertilityReturnOnMatureDeath :
            plantData.fertilityReturnOnMatureDeath * plantData.immatureDeathReturnMultiplier;

        // 特殊死亡效果
        if (plantData.matureEffect == PlantMatureEffect.SunflowerFertilitySpread && isMature)
        {
            // 向日葵：返还2倍肥力并扩散（暂不实现扩散逻辑）
            returnFertility *= 2f;
            Debug.Log($"[Plant] 向日葵死亡，肥力扩散！返还 {returnFertility}");
        }

        // 返还肥力
        EcosystemManager.Instance.AddFertility(returnFertility);

        // 橡树死亡时减少恢复速率
        if (plantData.matureEffect == PlantMatureEffect.OakLeafFall && isMature)
        {
            EcosystemManager.Instance.OnOakDied();
        }

        // 从生态系统注销
        EcosystemManager.Instance.UnregisterPlant(this);

        Debug.Log($"[Plant] {plantData.plantName} 死亡，返还肥力: {returnFertility:F1}");

        // 销毁游戏对象
        Destroy(gameObject);
    }

    /// <summary>
    /// 获取当前肥力消耗速率
    /// </summary>
    public float GetFertilityConsumption()
    {
        return plantData.fertilityConsumptionPerSecond;
    }

    /// <summary>
    /// 获取阴影值
    /// </summary>
    public float GetShadowValue()
    {
        return plantData.shadowValue;
    }

    private void OnDestroy()
    {
        // 确保从系统中移除
        if (EcosystemManager.Instance != null)
        {
            EcosystemManager.Instance.UnregisterPlant(this);
        }
    }

    // 调试信息
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // 显示阳光值
        Gizmos.color = Color.yellow;
        Vector3 labelPos = transform.position + Vector3.up * 2f;

#if UNITY_EDITOR
        UnityEditor.Handles.Label(labelPos, $"光: {currentLight:F0}%\n龄: {currentAge:F1}s");
#endif
    }
}