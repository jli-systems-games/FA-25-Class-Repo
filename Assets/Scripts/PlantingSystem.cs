using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 种植系统管理器 - 为Hesperia教程定制
/// 处理植物的拖拽放置逻辑
/// </summary>
public class PlantingSystem : MonoBehaviour
{
    [Header("拖拽设置")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform plantContainer;

    [Header("种植区域限制")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float groundY = 0f;

    [Header("🟢 教程系统")]
    [SerializeField] private TutorialEventManager tutorialManager;

    [Header("当前拖拽状态")]
    private PlantData currentDraggedPlantData;
    private PlantCard currentDraggedCard;
    private GameObject dragPreview;
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

            if (Input.GetMouseButtonUp(0))
            {
                TryPlacePlant();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                TrySelectPlant();
            }
        }
    }

    /// <summary>
    /// 尝试选中植物
    /// </summary>
    private void TrySelectPlant()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);

        Plant closestPlant = null;
        float closestDistance = float.MaxValue;

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
    /// 开始拖拽植物（从PlantCard调用）
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

        CreateDragPreview();

        Debug.Log($"[PlantingSystem] 开始拖拽: {plantData.plantName}");
    }

    /// <summary>
    /// 创建拖拽预览
    /// </summary>
    private void CreateDragPreview()
    {
        dragPreview = new GameObject("DragPreview");
        dragPreviewRenderer = dragPreview.AddComponent<SpriteRenderer>();
        dragPreviewRenderer.sprite = currentDraggedPlantData.youngSprite;
        dragPreviewRenderer.color = validPlacementColor;
        dragPreviewRenderer.sortingOrder = 100;

        dragPreview.transform.position = GetMouseWorldPosition();
    }

    /// <summary>
    /// 更新拖拽预览位置
    /// </summary>
    private void UpdateDragPreview()
    {
        if (dragPreview == null) return;

        Vector3 mousePos = GetMouseWorldPosition();

        Vector3 targetPos = new Vector3(
            Mathf.Clamp(mousePos.x, minX, maxX),
            groundY,
            0f
        );

        dragPreview.transform.position = targetPos;

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

        if (CanPlaceAtPosition(placePosition))
        {
            if (currentDraggedPlantData.plantPrefab != null)
            {
                GameObject plantObj = Instantiate(
                    currentDraggedPlantData.plantPrefab,
                    placePosition,
                    Quaternion.identity,
                    plantContainer
                );

                Plant plant = plantObj.GetComponent<Plant>();
                if (plant != null)
                {
                    // 设置PlantData（使用反射访问私有字段）
                    var field = typeof(Plant).GetField("plantData",
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.Instance);
                    field?.SetValue(plant, currentDraggedPlantData);

                    // 消耗库存
                    if (currentDraggedCard != null)
                    {
                        currentDraggedCard.ConsumeStock();
                    }

                    Debug.Log($"[PlantingSystem] 成功种植: {currentDraggedPlantData.plantName} at {placePosition}");

                    // 🟢 教程检测：种植成功
                    CheckTutorialPlantSuccess(currentDraggedPlantData, plant);
                }
                else
                {
                    Debug.LogError($"[PlantingSystem] 预制体缺少Plant组件！");
                }
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

        // 检查肥力是否足够
        if (EcosystemManager.Instance.CurrentFertility <= 0)
            return false;

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
    /// 检测是否点击在UI上
    /// </summary>
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    // ============================================================
    // 🟢 教程检测系统
    // ============================================================

    /// <summary>
    /// 检测教程：种植成功事件
    /// </summary>
    private void CheckTutorialPlantSuccess(PlantData plantData, Plant plant)
    {
        if (tutorialManager == null || !tutorialManager.IsWaitingForCompletion())
            return;

        string waitingEvent = tutorialManager.GetCurrentWaitingEvent();

        // 教程第一步：检测拖拽任意植物
        // Ink: "Just drag it up. # wait_event drag"
        if (waitingEvent == "drag")
        {
            tutorialManager.CompleteEvent("drag");
            Debug.Log("✓ 教程完成: 玩家种植了第一株植物");
        }

        // 教程第二步：检测种植蘑菇
        // Ink: "Try placing this mycelium under the tree. # wait_event plant_mushroom"
        else if (waitingEvent == "plant_mushroom")
        {
            // 检查是否是蘑菇类植物
            if (IsMushroom(plantData))
            {
                tutorialManager.CompleteEvent("plant_mushroom");
                Debug.Log("✓ 教程完成: 玩家种植了蘑菇");

                // 🟢 额外：监听这株蘑菇的死亡事件
                if (plant != null)
                {
                    StartCoroutine(MonitorPlantDeath(plant, "mushroom_death"));
                }
            }
        }
    }

    /// <summary>
    /// 判断是否是蘑菇类植物
    /// </summary>
    private bool IsMushroom(PlantData plantData)
    {
        // 方法1: 通过名字判断
        if (plantData.plantName.ToLower().Contains("mushroom") ||
            plantData.plantName.ToLower().Contains("mycelium") ||
            plantData.plantName.Contains("蘑菇") ||
            plantData.plantName.Contains("菌"))
        {
            return true;
        }

        // 方法2: 通过植物类型判断（如果你的PlantData有类型标签）
        if ((plantData.plantType & PlantType.Fungus) != 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 监听植物死亡事件（用于教程）
    /// </summary>
    private System.Collections.IEnumerator MonitorPlantDeath(Plant plant, string eventToComplete)
    {
        if (plant == null) yield break;

        // 持续监听植物状态
        while (plant != null && !plant.IsDead)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // 植物死亡了
        if (plant == null || plant.IsDead)
        {
            Debug.Log($"[PlantingSystem] 检测到植物死亡");

            // 检查是否是教程需要的事件
            if (tutorialManager != null && tutorialManager.IsWaitingForCompletion())
            {
                string waitingEvent = tutorialManager.GetCurrentWaitingEvent();
                if (waitingEvent == eventToComplete)
                {
                    tutorialManager.CompleteEvent(eventToComplete);
                    Debug.Log($"✓ 教程完成: {eventToComplete}");
                }
            }
        }
    }

    // ============================================================
    // 🟢 可选：提供公开方法让其他脚本手动触发教程完成
    // ============================================================

    /// <summary>
    /// 手动完成当前教程事件（调试用）
    /// </summary>
    public void CompleteCurrentTutorialEvent()
    {
        if (tutorialManager != null && tutorialManager.IsWaitingForCompletion())
        {
            string eventName = tutorialManager.GetCurrentWaitingEvent();
            tutorialManager.CompleteEvent(eventName);
            Debug.Log($"[PlantingSystem] 手动完成教程: {eventName}");
        }
    }
}