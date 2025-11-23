using UnityEngine;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// 黄昏庭院生态系统管理器
/// 管理全局肥力、时间流逝、植物列表
/// </summary>
public class EcosystemManager : MonoBehaviour
{
    public static EcosystemManager Instance { get; private set; }

    [Header("肥力系统")]
    [SerializeField] private float currentFertility = 100f;
    [SerializeField] private float maxFertility = 150f;
    [SerializeField] private float baseFertilityRecoveryRate = 3f; // 基础恢复速率
    private float currentFertilityRecoveryRate; // 当前恢复速率（受橡树影响）

    [Header("时间系统")]
    [Tooltip("1秒 = 1游戏日")]
    private float gameTime = 0f;

    [Header("植物管理")]
    private List<Plant> allPlants = new List<Plant>();
    private int matureOakCount = 0; // 成熟橡树数量（影响恢复速率）

    [Header("UI引用")]
    public TextMeshProUGUI fertilityText;
    public TextMeshProUGUI recoveryRateText;
    public UnityEngine.UI.Slider fertilitySlider;  // 肥力滑动条

    // 属性访问器
    public float CurrentFertility => currentFertility;
    public float MaxFertility => maxFertility;
    public float CurrentRecoveryRate => currentFertilityRecoveryRate;

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

        currentFertilityRecoveryRate = baseFertilityRecoveryRate;
    }

    private void Update()
    {
        gameTime += Time.deltaTime;

        // 肥力恢复（每秒）
        if (currentFertility < maxFertility)
        {
            currentFertility += currentFertilityRecoveryRate * Time.deltaTime;
            currentFertility = Mathf.Min(currentFertility, maxFertility);
        }

        // 肥力消耗（所有植物）
        float totalConsumption = 0f;
        foreach (Plant plant in allPlants)
        {
            totalConsumption += plant.GetFertilityConsumption();
        }
        currentFertility -= totalConsumption * Time.deltaTime;
        currentFertility = Mathf.Max(currentFertility, 0f);

        UpdateUI();
    }

    /// <summary>
    /// 尝试消耗肥力（种植时）
    /// </summary>
    public bool TryConsumeFertility(float amount)
    {
        if (currentFertility >= amount)
        {
            currentFertility -= amount;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 添加肥力（植物死亡返还）
    /// </summary>
    public void AddFertility(float amount)
    {
        currentFertility += amount;
        currentFertility = Mathf.Min(currentFertility, maxFertility);
    }

    /// <summary>
    /// 注册植物到系统
    /// </summary>
    public void RegisterPlant(Plant plant)
    {
        if (!allPlants.Contains(plant))
        {
            allPlants.Add(plant);
        }
    }

    /// <summary>
    /// 从系统移除植物
    /// </summary>
    public void UnregisterPlant(Plant plant)
    {
        allPlants.Remove(plant);
    }

    /// <summary>
    /// 橡树成熟时调用（增加恢复速率）
    /// </summary>
    public void OnOakMatured()
    {
        matureOakCount++;
        RecalculateFertilityRecoveryRate();
    }

    /// <summary>
    /// 橡树死亡时调用（减少恢复速率）
    /// </summary>
    public void OnOakDied()
    {
        matureOakCount = Mathf.Max(0, matureOakCount - 1);
        RecalculateFertilityRecoveryRate();
    }

    /// <summary>
    /// 重新计算肥力恢复速率
    /// 每棵成熟橡树 +2/秒
    /// </summary>
    private void RecalculateFertilityRecoveryRate()
    {
        currentFertilityRecoveryRate = baseFertilityRecoveryRate + (matureOakCount * 2f);
    }

    /// <summary>
    /// 获取所有植物列表
    /// </summary>
    public List<Plant> GetAllPlants()
    {
        return new List<Plant>(allPlants);
    }

    private void UpdateUI()
    {
        if (fertilityText != null)
        {
            fertilityText.text = $"Fertility: {currentFertility:F1} / {maxFertility}";
        }
        if (recoveryRateText != null)
        {
            recoveryRateText.text = $"Recovery: {currentFertilityRecoveryRate:F1}/s";
        }
        if (fertilitySlider != null)
        {
            fertilitySlider.maxValue = maxFertility;
            fertilitySlider.value = currentFertility;
        }
    }
}