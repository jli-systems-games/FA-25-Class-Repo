using UnityEngine;

/// <summary>
/// 植物核心脚本
/// 处理生命周期、阳光需求、肥力消耗、成熟效果
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Plant : MonoBehaviour
{
    [Header("植物配置")]
    [SerializeField] public PlantData plantData;  // 改为public，允许外部访问

    [Header("预设状态（场景编辑器用）")]
    [Tooltip("勾选后，游戏开始时使用预设状态而不是从幼苗开始")]
    [SerializeField] private bool usePresetState = false;

    [Tooltip("预设年龄（秒）- 0=幼苗, maturityTime=成熟, deathTime=即将死亡")]
    [SerializeField] private float presetAge = 0f;

    [Tooltip("预设为成熟状态")]
    [SerializeField] private bool startMature = false;

    [Tooltip("预设枯萎伤害（秒）- 0=健康, witherTime=濒死")]
    [SerializeField] private float presetWitherDamage = 0f;

    [Header("贴图引用")]
    [SerializeField] private SpriteRenderer spriteRenderer;  // 植物贴图渲染器（可以在子物体）

    [Header("授粉点")]
    [SerializeField] private Transform pollinationPoint;  // 授粉点位置（蝴蝶会飞向这里）

    [Header("粒子特效")]
    [SerializeField] private ParticleSystem matureParticle;  // 成熟时播放的粒子
    [SerializeField] private ParticleSystem deathParticle;   // 死亡时播放的粒子

    [Header("碰撞箱引用")]
    [SerializeField] private BoxCollider2D lightSensor;      // 光照检测箱
    [SerializeField] private BoxCollider2D shadowCollider;   // 阴影碰撞箱（幼年）
    [SerializeField] private BoxCollider2D matureShadowCollider;  // 成熟阴影碰撞箱（可选）
    private BoxCollider2D currentShadowCollider;             // 当前使用的阴影碰撞箱
    private SpriteRenderer lightSensorRenderer;              // 检测器的视觉反馈

    [Header("成长进度指示器")]
    [SerializeField] private UnityEngine.UI.Image growthIndicator;  // UI Image组件（Filled类型）

    [Header("检测器颜色")]
    [SerializeField] private Color sensorGoodColor = Color.green;    // 光照达标颜色
    [SerializeField] private Color sensorBadColor = Color.red;       // 光照不达标颜色

    [Header("生命状态")]
    private float currentAge = 0f;
    private float maturityTime;  // 成熟时间
    private float deathTime;     // 死亡时间
    private bool isMature = false;
    private bool isDead = false;

    // 公开属性供UI访问
    public float CurrentAge => currentAge;
    public bool IsMature => isMature;
    public bool IsDead => isDead;
    public bool IsWithering => isWithering;
    public float CurrentLight => currentLight;

    /// <summary>
    /// 获取授粉点位置（蝴蝶等动物飞向的目标点）
    /// </summary>
    public Vector3 GetPollinationPoint()
    {
        // 如果有指定授粉点，使用授粉点位置
        if (pollinationPoint != null)
        {
            return pollinationPoint.position;
        }

        // 否则使用植物根部位置（默认）
        return transform.position;
    }

    [Header("枯萎状态")]
    private bool isWithering = false;     // 是否正在枯萎
    private float witherTimer = 0f;       // 枯萎计时器

    [Header("繁殖状态")]
    private float spreadTimer = 0f;       // 繁殖计时器（持续繁殖用）
    private bool hasSpreadOnMature = false; // 是否已在成熟时繁殖过

    [Header("阳光状态")]
    private float currentLight = 100f;
    private bool hasEnoughLight = true;

    [Header("肥力状态")]
    private bool hasEnoughFertility = true;

    private void Awake()
    {
        if (plantData != null)
        {
            // 设置生命周期时间
            maturityTime = plantData.lifespanMin;
            deathTime = plantData.lifespanMax;

            // 应用预设状态（如果启用）
            if (usePresetState)
            {
                ApplyPresetState();
            }
            else
            {
                // 默认：从幼苗开始
                if (spriteRenderer != null && plantData.youngSprite != null)
                {
                    spriteRenderer.sprite = plantData.youngSprite;
                }
            }
        }
    }

    /// <summary>
    /// 应用预设状态（场景编辑器用）
    /// </summary>
    private void ApplyPresetState()
    {
        // 设置年龄
        currentAge = Mathf.Clamp(presetAge, 0f, deathTime);

        // 设置成熟状态
        if (startMature || currentAge >= maturityTime)
        {
            isMature = true;

            // 切换到成熟贴图
            if (spriteRenderer != null && plantData.matureSprite != null)
            {
                spriteRenderer.sprite = plantData.matureSprite;
            }

            // 切换到成熟阴影碰撞箱
            if (matureShadowCollider != null)
            {
                shadowCollider.enabled = false;
                shadowCollider.gameObject.SetActive(false);

                matureShadowCollider.enabled = true;
                matureShadowCollider.gameObject.SetActive(true);

                currentShadowCollider = matureShadowCollider;
                Debug.Log($"[Plant] {plantData.plantName} 预设状态：切换到成熟阴影碰撞箱");
            }

            // 注意：成熟效果在Start中触发，因为需要EcosystemManager已初始化
        }
        else
        {
            // 幼年状态
            if (spriteRenderer != null && plantData.youngSprite != null)
            {
                spriteRenderer.sprite = plantData.youngSprite;
            }
        }

        // 设置枯萎伤害
        witherTimer = Mathf.Clamp(presetWitherDamage, 0f, plantData.witherTime);
        if (witherTimer > 0f)
        {
            isWithering = true;
            if (spriteRenderer != null && plantData.witherSprite != null)
            {
                spriteRenderer.sprite = plantData.witherSprite;
            }
        }

        Debug.Log($"[Plant] {plantData.plantName} 使用预设状态：年龄={currentAge:F1}秒, 成熟={isMature}, 枯萎伤害={witherTimer:F1}秒");
    }

    /// <summary>
    /// 触发成熟效果（从OnMatured提取出来，供预设状态调用）
    /// </summary>
    private void TriggerMatureEffects()
    {
        // 触发特定成熟效果
        switch (plantData.matureEffect)
        {
            case PlantMatureEffect.OakLeafFall:
                // 橡树落叶效果：通知生态系统增加恢复速率
                EcosystemManager.Instance.OnOakMatured();
                Debug.Log($"[Plant] {plantData.plantName} 预设成熟：橡树效果已激活");
                break;

            case PlantMatureEffect.SunflowerFertilitySpread:
                // 向日葵在死亡时触发，这里不处理
                break;

            case PlantMatureEffect.FruitProduction:
                // 结果植物：可以添加持续产出逻辑
                break;
        }
    }

    private void Start()
    {
        // 注册到生态系统
        EcosystemManager.Instance.RegisterPlant(this);

        // 如果使用预设状态且已成熟，触发成熟效果
        if (usePresetState && isMature)
        {
            TriggerMatureEffects();
        }

        // 验证必要组件
        if (spriteRenderer == null)
        {
            Debug.LogError($"[Plant] {plantData.plantName} 缺少SpriteRenderer引用！请在Inspector中拖入贴图物体的SpriteRenderer");
        }

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

        // 初始化阴影碰撞箱
        if (matureShadowCollider != null)
        {
            // 如果已经是成熟状态（预设或其他原因），不要重置阴影箱
            if (isMature && currentShadowCollider == matureShadowCollider)
            {
                // 保持成熟阴影箱启用状态
                shadowCollider.enabled = false;
                shadowCollider.gameObject.SetActive(false);
                Debug.Log($"[Plant] {plantData.plantName} 保持成熟阴影碰撞箱状态（预设成熟）");
            }
            else
            {
                // 幼年状态，禁用成熟阴影箱
                currentShadowCollider = shadowCollider;
                matureShadowCollider.enabled = false;
                matureShadowCollider.gameObject.SetActive(false);
                Debug.Log($"[Plant] {plantData.plantName} 禁用成熟阴影碰撞箱（从幼苗开始）");
            }
        }
        else
        {
            // 没有成熟阴影箱，直接使用幼年阴影箱
            currentShadowCollider = shadowCollider;
        }

        // 确保幼年阴影碰撞箱启用
        if (shadowCollider != null)
        {
            shadowCollider.enabled = true;
            shadowCollider.gameObject.SetActive(true);
        }

        Debug.Log($"[Plant] {plantData.plantName} 种植成功，成熟时间: {maturityTime:F1}秒，死亡时间: {deathTime:F1}秒");
    }

    private void Update()
    {
        if (isDead) return;

        // 检查阳光需求
        CheckLightRequirement();

        // 幼年期持续消耗肥力
        if (!isMature)
        {
            CheckFertilityRequirement();
        }
        else
        {
            // 成熟后不消耗肥力，但要重置hasEnoughFertility为true
            hasEnoughFertility = true;
        }

        // 检查枯萎状态
        CheckWitherState();

        // 只有非枯萎状态才增长年龄（枯萎=停止生长）
        if (!isWithering)
        {
            currentAge += Time.deltaTime;

            // 检查是否成熟
            if (!isMature && currentAge >= maturityTime)
            {
                OnMatured();
            }
        }

        // 更新成长进度指示器
        UpdateGrowthIndicator();

        // 成熟后处理繁殖
        if (isMature && !isWithering)
        {
            HandleSpread();
        }

        // 检查是否寿终正寝
        if (currentAge >= deathTime)
        {
            Die(true); // 自然死亡
        }
    }

    /// <summary>
    /// 更新成长进度指示器
    /// </summary>
    private void UpdateGrowthIndicator()
    {
        if (growthIndicator == null) return;

        if (!isMature)
        {
            // 幼年期：显示成长进度
            float progress = Mathf.Clamp01(currentAge / maturityTime);
            growthIndicator.fillAmount = progress;

            // 根据光照和肥力状态改变颜色
            if (isWithering)
            {
                growthIndicator.color = Color.red;  // 枯萎：红色
            }
            else if (hasEnoughLight && hasEnoughFertility)
            {
                growthIndicator.color = Color.green;  // 正常生长：绿色
            }
            else
            {
                growthIndicator.color = Color.yellow;  // 条件不足：黄色
            }
        }
        else
        {
            // 成熟后：显示满圈，半透明
            growthIndicator.fillAmount = 1f;
            growthIndicator.color = new Color(1f, 1f, 1f, 0.3f);  // 半透明白色
        }
    }

    /// <summary>
    /// 检查肥力需求（幼年期持续消耗）
    /// </summary>
    private void CheckFertilityRequirement()
    {
        float consumption = plantData.fertilityConsumptionPerSecond * Time.deltaTime;
        hasEnoughFertility = EcosystemManager.Instance.TryConsumeFertility(consumption);
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
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = plantData.witherSprite;
                }
                Debug.Log($"[Plant] {plantData.plantName} 开始枯萎（光照:{hasEnoughLight}, 肥力:{hasEnoughFertility}）");
            }

            // 累积枯萎时间（不可逆！）
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
                // 恢复生长，但枯萎伤害不会消退
                isWithering = false;
                // 不重置witherTimer！枯萎伤害是永久的

                // 恢复对应状态的贴图
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = isMature ? plantData.matureSprite : plantData.youngSprite;
                }
                Debug.Log($"[Plant] {plantData.plantName} 恢复生长，但枯萎伤害累积: {witherTimer:F1}/{plantData.witherTime:F1}秒");
            }
        }
    }

    /// <summary>
    /// 处理繁殖逻辑
    /// </summary>
    private void HandleSpread()
    {
        switch (plantData.spreadMode)
        {
            case SpreadMode.OnMature:
                // 成熟时触发一次（向日葵）
                if (!hasSpreadOnMature)
                {
                    TriggerSpread();
                    hasSpreadOnMature = true;
                }
                break;

            case SpreadMode.Continuous:
                // 成熟后持续繁殖（草本、蘑菇菌丝）
                spreadTimer += Time.deltaTime;
                if (spreadTimer >= plantData.spreadInterval)
                {
                    TriggerSpread();
                    spreadTimer = 0f;
                }
                break;

            case SpreadMode.OnDeath:
                // 死亡时触发，在Die方法中处理
                break;

            case SpreadMode.RequireAnimal:
                // 需要动物，暂不实现
                break;

            case SpreadMode.None:
            default:
                // 不繁殖
                break;
        }
    }

    /// <summary>
    /// 触发繁殖（生成幼苗）
    /// </summary>
    private void TriggerSpread()
    {
        // 确定要生成的Prefab（优先使用offspringPrefab，为空则生成自己）
        GameObject prefabToSpawn = plantData.offspringPrefab != null ?
            plantData.offspringPrefab : plantData.plantPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[Plant] {plantData.plantName} 没有设置繁殖Prefab，无法繁殖");
            return;
        }

        // 检查植物总数上限
        int currentPlantCount = EcosystemManager.Instance.CurrentPlantCount;
        int maxPlantCount = EcosystemManager.Instance.MaxPlantCount;

        if (currentPlantCount >= maxPlantCount)
        {
            Debug.LogWarning($"[Plant] 植物数量已达上限 ({currentPlantCount}/{maxPlantCount})，停止繁殖");
            return;
        }

        // 检查是否有指定的生成区域（蘑菇菌丝用）
        BoxCollider2D spawnArea = GetComponentInChildren<BoxCollider2D>();
        bool hasSpawnArea = spawnArea != null && spawnArea.gameObject.name.Contains("SpawnArea");

        int spawnedCount = 0;

        for (int i = 0; i < plantData.spreadCount; i++)
        {
            // 再次检查是否超过上限（每次循环都检查）
            if (EcosystemManager.Instance.CurrentPlantCount >= maxPlantCount)
            {
                Debug.Log($"[Plant] 繁殖中止：已生成 {spawnedCount}/{plantData.spreadCount}，达到植物上限");
                break;
            }

            Vector3 spawnPosition;

            if (hasSpawnArea)
            {
                // 在指定碰撞箱内随机生成（蘑菇菌丝）
                Bounds bounds = spawnArea.bounds;
                float randomX = Random.Range(bounds.min.x, bounds.max.x);
                float randomZ = Random.Range(bounds.min.z, bounds.max.z);
                spawnPosition = new Vector3(randomX, transform.position.y, randomZ);
            }
            else
            {
                // 在范围内随机位置（普通植物）
                Vector2 randomOffset = Random.insideUnitCircle * plantData.spreadRange;
                spawnPosition = transform.position + new Vector3(randomOffset.x, 0f, randomOffset.y);
                spawnPosition.y = transform.position.y;
            }

            // 直接生成，不检测重叠（让生态自然淘汰）
            GameObject offspring = Instantiate(
                prefabToSpawn,
                spawnPosition,
                Quaternion.identity,
                transform.parent
            );

            spawnedCount++;
            Debug.Log($"[Plant] {plantData.plantName} 繁殖生成幼苗 at {spawnPosition}");
        }

        if (spawnedCount > 0)
        {
            Debug.Log($"[Plant] {plantData.plantName} 繁殖完成，生成 {spawnedCount} 株幼苗，当前总数: {EcosystemManager.Instance.CurrentPlantCount}");
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
        if (plantData.matureSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = plantData.matureSprite;
        }

        // 播放成熟粒子特效
        if (matureParticle != null)
        {
            matureParticle.Play();
            Debug.Log($"[Plant] {plantData.plantName} 播放成熟粒子特效");
        }

        // 切换到成熟阴影碰撞箱（如果有）
        if (matureShadowCollider != null)
        {
            // 禁用幼年阴影
            shadowCollider.enabled = false;
            shadowCollider.gameObject.SetActive(false);

            // 启用成熟阴影
            matureShadowCollider.enabled = true;
            matureShadowCollider.gameObject.SetActive(true);

            // 更新当前使用的阴影碰撞箱
            currentShadowCollider = matureShadowCollider;

            Debug.Log($"[Plant] {plantData.plantName} 切换到成熟阴影碰撞箱");
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

        // 播放死亡粒子特效
        if (deathParticle != null)
        {
            deathParticle.Play();
            Debug.Log($"[Plant] {plantData.plantName} 播放死亡粒子特效");
        }

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

        // 延迟销毁，让粒子播放完
        if (deathParticle != null)
        {
            // 获取粒子持续时间
            float particleDuration = deathParticle.main.duration + deathParticle.main.startLifetime.constantMax;
            Destroy(gameObject, particleDuration);
        }
        else
        {
            // 立即销毁
            Destroy(gameObject);
        }
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

    // 不再使用OnMouseDown，改用PlantingSystem的射线检测

    /// <summary>
    /// 收割植物（手动收割）
    /// </summary>
    public void Harvest()
    {
        if (isDead) return;

        Debug.Log($"[Plant] 手动收割: {plantData.plantName}");

        // 调用死亡方法（会播放粒子特效和返还肥力）
        Die(false); // false表示非自然死亡（收割）
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