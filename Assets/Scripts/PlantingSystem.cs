using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 种植系统管理器
/// 处理植物的拖拽放置逻辑
/// </summary>
public class PlantingSystem : MonoBehaviour
{
    [Header("拖拽设置")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform plantContainer; // 植物的父容器

    [Header("种植区域限制")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float groundY = 0f; // 地面Y坐标（植物扎根位置）

    [Header("当前拖拽状态")]
    private PlantData currentDraggedPlantData;
    private PlantCard currentDraggedCard;      // 当前拖拽的卡牌
    private GameObject dragPreview; // 拖拽预览对象
    private SpriteRenderer dragPreviewRenderer;
    private bool isDragging = false;

    [Header("预览设置")]
    [SerializeField] private Color validPlacementColor = new Color(1f, 1f, 1f, 0.6f);
    [SerializeField] private Color invalidPlacementColor = new Color(1f, 0f, 0f, 0.6f);

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (plantContainer == null)
        {
            plantContainer = transform;
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            UpdateDragPreview();

            // 检测鼠标释放
            if (Input.GetMouseButtonUp(0))
            {
                TryPlacePlant();
            }
        }
        else
        {
            // 不在拖拽时，检测点击植物
            if (Input.GetMouseButtonDown(0))
            {
                TrySelectPlant();
            }
        }
    }

    /// <summary>
    /// 尝试选中植物（射线检测所有碰撞，选最近的）
    /// </summary>
    private void TrySelectPlant()
    {
        // 从鼠标位置发射射线
        Vector3 mousePos = GetMouseWorldPosition();

        // 检测所有碰撞的物体
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);

        Plant closestPlant = null;
        float closestDistance = float.MaxValue;

        // 遍历所有碰撞物，找最近的植物
        foreach (Collider2D hit in hits)
        {
            Plant plant = hit.GetComponentInParent<Plant>();
            if (plant != null)
            {
                float distance = Vector2.Distance(mousePos, plant.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlant = plant;
                }
            }
        }

        // 如果找到植物，显示信息面板
        if (closestPlant != null)
        {
            if (PlantInfoPanel.Instance != null)
            {
                PlantInfoPanel.Instance.ShowPanel(closestPlant);
            }

            Debug.Log($"[PlantingSystem] 选中植物: {closestPlant.plantData.plantName}");
        }
    }

    /// <summary>
    /// 开始拖拽植物（从UI卡牌调用）
    /// </summary>
    public void StartDragging(PlantData plantData, PlantCard plantCard)
    {
        if (plantData == null)
        {
            Debug.LogWarning("[PlantingSystem] PlantData为空！");
            return;
        }

        currentDraggedPlantData = plantData;
        currentDraggedCard = plantCard;
        isDragging = true;

        // 创建拖拽预览
        CreateDragPreview();

        Debug.Log($"[PlantingSystem] 开始拖拽: {plantData.plantName}");
    }

    /// <summary>
    /// 创建拖拽预览对象
    /// </summary>
    private void CreateDragPreview()
    {
        dragPreview = new GameObject("DragPreview");
        dragPreviewRenderer = dragPreview.AddComponent<SpriteRenderer>();
        dragPreviewRenderer.sprite = currentDraggedPlantData.youngSprite;  // 拖拽预览使用幼年贴图
        dragPreviewRenderer.color = validPlacementColor;
        dragPreviewRenderer.sortingOrder = 100; // 确保在最上层

        // 设置sprite的pivot在底部（已在sprite设置中完成，这里只是确认）
        dragPreview.transform.position = GetMouseWorldPosition();
    }

    /// <summary>
    /// 更新拖拽预览位置
    /// </summary>
    private void UpdateDragPreview()
    {
        if (dragPreview == null) return;

        Vector3 mousePos = GetMouseWorldPosition();

        // 只移动X轴，Y轴锁定在地面
        Vector3 targetPos = new Vector3(
            Mathf.Clamp(mousePos.x, minX, maxX),
            groundY,
            0f
        );

        dragPreview.transform.position = targetPos;

        // 检查是否可以放置（检查肥力和位置）
        bool canPlace = CanPlaceAtPosition(targetPos);
        dragPreviewRenderer.color = canPlace ? validPlacementColor : invalidPlacementColor;
    }

    /// <summary>
    /// 尝试放置植物
    /// </summary>
    private void TryPlacePlant()
    {
        if (dragPreview == null)
        {
            CancelDragging();
            return;
        }

        Vector3 placePosition = dragPreview.transform.position;

        // 检查是否可以放置
        if (CanPlaceAtPosition(placePosition))
        {
            // 实例化预制体
            if (currentDraggedPlantData.plantPrefab != null)
            {
                GameObject plantObj = Instantiate(
                    currentDraggedPlantData.plantPrefab,
                    placePosition,
                    Quaternion.identity,
                    plantContainer
                );

                // 获取Plant组件并设置PlantData
                Plant plant = plantObj.GetComponent<Plant>();
                if (plant != null)
                {
                    // 通过反射设置PlantData（因为是私有字段）
                    var field = typeof(Plant).GetField("plantData",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);
                    field?.SetValue(plant, currentDraggedPlantData);

                    // 种植成功，消耗库存
                    if (currentDraggedCard != null)
                    {
                        currentDraggedCard.ConsumeStock();
                    }
                }
                else
                {
                    Debug.LogError($"[PlantingSystem] 预制体缺少Plant组件！");
                }

                Debug.Log($"[PlantingSystem] 成功种植: {currentDraggedPlantData.plantName} at {placePosition}");
            }
            else
            {
                Debug.LogError($"[PlantingSystem] PlantData中未设置plantPrefab！");
            }
        }
        else
        {
            Debug.Log($"[PlantingSystem] 无法种植：肥力不足或位置无效");
        }

        CancelDragging();
    }

    /// <summary>
    /// 取消拖拽
    /// </summary>
    private void CancelDragging()
    {
        isDragging = false;
        currentDraggedPlantData = null;
        currentDraggedCard = null;

        if (dragPreview != null)
        {
            Destroy(dragPreview);
            dragPreview = null;
        }
    }

    /// <summary>
    /// 检查是否可以在指定位置放置植物
    /// </summary>
    private bool CanPlaceAtPosition(Vector3 position)
    {
        // 检查X轴范围
        if (position.x < minX || position.x > maxX)
            return false;

        // 检查肥力是否足够（种植时不消耗肥力，但需要检查是否能承受消耗）
        // 这里暂时简化，只要肥力>0就可以种植
        if (EcosystemManager.Instance.CurrentFertility <= 0)
            return false;

        // TODO: 可以添加更多检查，如与其他植物的距离

        return true;
    }

    /// <summary>
    /// 获取鼠标在世界空间的位置
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    /// <summary>
    /// 检测是否点击在UI上（避免误触）
    /// </summary>
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}