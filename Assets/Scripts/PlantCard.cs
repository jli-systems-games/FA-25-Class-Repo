using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// UI植物卡牌
/// 放置在底部UI中，支持拖拽种植
/// </summary>
public class PlantCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("植物配置")]
    [SerializeField] private PlantData plantData;
    private int currentStock;  // 当前库存

    [Header("UI引用")]
    [SerializeField] private Image cardImage;       // 完整的卡牌图片
    [SerializeField] private TextMeshProUGUI stockText;  // 库存显示

    [Header("视觉状态")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emptyColor = Color.gray;

    [Header("系统引用")]
    private PlantingSystem plantingSystem;

    [Header("拖拽状态")]
    private bool isDragging = false;

    private void Start()
    {
        plantingSystem = FindObjectOfType<PlantingSystem>();

        if (plantingSystem == null)
        {
            Debug.LogError("[PlantCard] 找不到PlantingSystem！");
        }

        // 初始化库存
        if (plantData != null)
        {
            currentStock = plantData.initialStock;
        }

        // 初始化UI显示
        UpdateCardDisplay();
    }

    /// <summary>
    /// 更新卡牌显示
    /// </summary>
    private void UpdateCardDisplay()
    {
        if (plantData == null) return;

        // 根据库存状态设置颜色
        bool isEmpty = currentStock <= 0;

        if (cardImage != null)
        {
            cardImage.sprite = plantData.cardSprite;
            cardImage.color = isEmpty ? emptyColor : normalColor;
        }

        if (stockText != null)
        {
            stockText.text = $"x{currentStock}";
            stockText.color = isEmpty ? emptyColor : normalColor;
        }
    }

    /// <summary>
    /// 开始拖拽时触发
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 检查库存
        if (currentStock <= 0)
        {
            Debug.Log($"[PlantCard] {plantData.plantName} 库存不足！");
            return;
        }

        if (plantData == null || plantingSystem == null)
        {
            Debug.LogWarning("[PlantCard] PlantData或PlantingSystem为空！");
            return;
        }

        isDragging = true;

        // 通知PlantingSystem开始拖拽，传入自己的引用
        plantingSystem.StartDragging(plantData, this);

        Debug.Log($"[PlantCard] 开始拖拽卡牌: {plantData.plantName}");
    }

    /// <summary>
    /// 拖拽中（这里不需要处理，由PlantingSystem处理）
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // PlantingSystem会在Update中处理拖拽逻辑
    }

    /// <summary>
    /// 拖拽结束时触发
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // 注意：实际的放置逻辑在PlantingSystem中处理
        // 这里只是重置卡牌状态

        Debug.Log($"[PlantCard] 结束拖拽: {plantData.plantName}");
    }

    /// <summary>
    /// 设置植物数据（用于动态创建卡牌）
    /// </summary>
    public void SetPlantData(PlantData data)
    {
        plantData = data;
        if (plantData != null)
        {
            currentStock = plantData.initialStock;
        }
        UpdateCardDisplay();
    }

    /// <summary>
    /// 消耗一个库存（种植成功时调用）
    /// </summary>
    public void ConsumeStock()
    {
        if (currentStock > 0)
        {
            currentStock--;
            UpdateCardDisplay();
            Debug.Log($"[PlantCard] {plantData.plantName} 库存剩余: {currentStock}");
        }
    }

    /// <summary>
    /// 检查是否有库存
    /// </summary>
    public bool HasStock()
    {
        return currentStock > 0;
    }
}