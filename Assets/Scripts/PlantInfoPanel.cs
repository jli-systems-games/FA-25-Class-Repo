using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 植物信息面板 - 显示选中植物的详细信息并提供收割功能
/// </summary>
public class PlantInfoPanel : MonoBehaviour
{
    public static PlantInfoPanel Instance { get; private set; }

    [Header("UI引用")]
    [SerializeField] private GameObject panelRoot;  // 整个面板的根物体
    [SerializeField] private TextMeshProUGUI plantNameText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI lightText;
    [SerializeField] private TextMeshProUGUI fertilityConsumptionText;
    [SerializeField] private TextMeshProUGUI harvestReturnText;
    [SerializeField] private TextMeshProUGUI matureEffectText;
    [SerializeField] private Button harvestButton;
    [SerializeField] private Button closeButton;

    private Plant selectedPlant;

    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("[PlantInfoPanel] 场景中存在多个PlantInfoPanel，销毁多余的");
            Destroy(gameObject);
            return;
        }

        // 绑定按钮事件
        if (harvestButton != null)
        {
            harvestButton.onClick.AddListener(OnHarvestClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        // 初始隐藏面板
        HidePanel();
    }

    private void Update()
    {
        // 如果有选中的植物，实时更新数据
        if (selectedPlant != null && panelRoot.activeSelf)
        {
            UpdatePanelData();
        }
    }

    /// <summary>
    /// 显示植物信息面板
    /// </summary>
    public void ShowPanel(Plant plant)
    {
        if (plant == null) return;

        selectedPlant = plant;
        panelRoot.SetActive(true);
        UpdatePanelData();

        Debug.Log($"[PlantInfoPanel] 显示面板: {plant.plantData.plantName}");
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    public void HidePanel()
    {
        selectedPlant = null;
        panelRoot.SetActive(false);
    }

    /// <summary>
    /// 实时更新面板数据
    /// </summary>
    private void UpdatePanelData()
    {
        if (selectedPlant == null || selectedPlant.plantData == null) return;

        // 植物名称
        if (plantNameText != null)
        {
            plantNameText.text = selectedPlant.plantData.plantName;
        }

        // 状态（幼年/成熟/枯萎）
        if (statusText != null)
        {
            string status = selectedPlant.IsMature ? "Mature" : "Young";
            if (selectedPlant.IsWithering)
            {
                status = "Withering";
            }
            statusText.text = $"Status: {status}";
        }

        // 年龄
        if (ageText != null)
        {
            ageText.text = $"Age: {selectedPlant.CurrentAge:F1}s";
        }

        // 光照
        if (lightText != null)
        {
            lightText.text = $"Light: {selectedPlant.CurrentLight:F0}%";
        }

        // 肥力消耗
        if (fertilityConsumptionText != null)
        {
            float consumption = selectedPlant.IsMature ? 0f : selectedPlant.plantData.fertilityConsumptionPerSecond;
            fertilityConsumptionText.text = $"Consumption: {consumption:F1}/s";
        }

        // 收割返还
        if (harvestReturnText != null)
        {
            float returnAmount = selectedPlant.IsMature ?
                selectedPlant.plantData.fertilityReturnOnMatureDeath :
                selectedPlant.plantData.fertilityReturnOnMatureDeath * selectedPlant.plantData.immatureDeathReturnMultiplier;
            harvestReturnText.text = $"Harvest Return: +{returnAmount:F0}";
        }

        // 成熟效果
        if (matureEffectText != null)
        {
            if (!string.IsNullOrEmpty(selectedPlant.plantData.matureEffectDescription))
            {
                matureEffectText.text = $"Mature Effect:\n{selectedPlant.plantData.matureEffectDescription}";
            }
            else
            {
                matureEffectText.text = "Mature Effect: None";
            }
        }
    }

    /// <summary>
    /// 收割按钮点击
    /// </summary>
    private void OnHarvestClicked()
    {
        if (selectedPlant == null) return;

        Debug.Log($"[PlantInfoPanel] 收割植物: {selectedPlant.plantData.plantName}");

        // 调用植物的收割方法
        selectedPlant.Harvest();

        // 关闭面板
        HidePanel();
    }

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    private void OnCloseClicked()
    {
        HidePanel();
    }
}