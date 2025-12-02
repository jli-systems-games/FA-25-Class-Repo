using UnityEngine;

public class InspectionSystem : MonoBehaviour
{
    [Header("=== 1. 相机设置 ===")]
    [Tooltip("玩家头上的主摄像机")]
    public Camera mainPlayerCamera; 
    
    [Tooltip("地下的检视摄像机 (必须拖入！)")]
    public Camera inspectionCamera; 

    [Header("=== 2. 场景引用 ===")]
    public GameObject inspectionStageParent; 
    public Transform inspectSpawnPoint;     

    [Header("=== 3. UI 管理 ===")]
    public GameObject mainHUDCanvas;  
    public GameObject dimmingCanvas;        

    [Header("=== 4. 玩家控制 ===")]
    public MonoBehaviour playerController;  
    
    [Header("=== 5. 检视参数 ===")]
    public float interactDistance = 3f;     
    public LayerMask interactLayer;         
    public float rotateSpeed = 5f;          
    public KeyCode exitKey = KeyCode.F;     

    [Header("=== 6. 缩放设置 (新增) ===")]
    public float zoomSpeed = 20f;   // 滚轮缩放速度
    public float minFOV = 20f;      // 放大最大倍数 (FOV越小放得越大)
    public float maxFOV = 60f;      // 缩小最小倍数 (默认视野)

    // --- 内部变量 ---
    private GameObject currentModel;        
    private bool isInspecting = false;      
    private float defaultFOV;       // 记录相机原本的 FOV，退出时复原

    void Start()
    {
        // 记录一下检视相机默认的视野大小 (比如 60)
        if (inspectionCamera != null) defaultFOV = inspectionCamera.fieldOfView;

        // 初始化关闭状态
        if (inspectionStageParent != null) inspectionStageParent.SetActive(false);
        if (dimmingCanvas != null) dimmingCanvas.SetActive(false);
        if (mainHUDCanvas != null) mainHUDCanvas.SetActive(true);
    }

    void Update()
    {
        if (isInspecting)
        {
            HandleInspectionLogic();
        }
        else
        {
            HandleRaycastLogic();
        }
    }

    // 逻辑 A: 寻找物体
    void HandleRaycastLogic()
    {
        if (mainPlayerCamera == null) return;

        Ray ray = new Ray(mainPlayerCamera.transform.position, mainPlayerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            InspectableItem item = hit.collider.GetComponent<InspectableItem>();
            if (item != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    EnterInspection(item.inspectionPrefab);
                }
            }
        }
    }

    // 逻辑 B: 检视操作 (旋转 + 缩放 + 退出)
    void HandleInspectionLogic()
    {
        if (currentModel == null) return;

        // 1. 鼠标拖拽旋转
        if (Input.GetMouseButton(0)) 
        {
            float rotX = Input.GetAxis("Mouse X") * rotateSpeed;
            float rotY = Input.GetAxis("Mouse Y") * rotateSpeed;

            // 相对于相机视角旋转
            // 只要 inspectionCamera 赋值了，操作就会非常顺手
            if (inspectionCamera != null)
            {
                currentModel.transform.Rotate(inspectionCamera.transform.up, -rotX, Space.World);
                currentModel.transform.Rotate(inspectionCamera.transform.right, rotY, Space.World);
            }
            else
            {
                // 保底逻辑 (万一没拖相机)
                currentModel.transform.Rotate(Vector3.up, -rotX, Space.World);
                currentModel.transform.Rotate(Vector3.right, rotY, Space.World);
            }
        }

        // 2. 鼠标滚轮缩放 (新增功能)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && inspectionCamera != null)
        {
            // 获取当前 FOV
            float currentFOV = inspectionCamera.fieldOfView;
            
            // 滚轮向上(正) -> FOV变小(放大)
            // 滚轮向下(负) -> FOV变大(缩小)
            currentFOV -= scroll * zoomSpeed; 

            // 限制范围 (防止无限放大或倒过来)
            currentFOV = Mathf.Clamp(currentFOV, minFOV, maxFOV);

            // 应用回去
            inspectionCamera.fieldOfView = currentFOV;
        }

        // 3. 退出
        if (Input.GetKeyDown(exitKey))
        {
            ExitInspection();
        }
    }

    void EnterInspection(GameObject prefab)
    {
        if (prefab == null) return;
        isInspecting = true;

        // 重置 FOV (每次进来都是默认大小，不会保留上次的放大状态)
        if (inspectionCamera != null) inspectionCamera.fieldOfView = defaultFOV;

        // UI & 舞台控制
        if (mainHUDCanvas != null) mainHUDCanvas.SetActive(false);
        if (dimmingCanvas != null) dimmingCanvas.SetActive(true);
        if (inspectionStageParent != null) inspectionStageParent.SetActive(true);

        // 玩家控制
        if (playerController != null) playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;                  

        // 生成
        currentModel = Instantiate(prefab, inspectSpawnPoint.position, Quaternion.identity);
        currentModel.transform.SetParent(inspectSpawnPoint);
        currentModel.transform.localPosition = Vector3.zero; 
        
        // 稍微重置一下旋转，让物品正对着相机
        currentModel.transform.localRotation = Quaternion.identity;

        Collider col = currentModel.GetComponent<Collider>();
        if (col != null) Destroy(col);
    }

    void ExitInspection()
    {
        isInspecting = false;

        if (currentModel != null) Destroy(currentModel);
        if (inspectionStageParent != null) inspectionStageParent.SetActive(false);
        if (dimmingCanvas != null) dimmingCanvas.SetActive(false);
        if (mainHUDCanvas != null) mainHUDCanvas.SetActive(true);

        if (playerController != null) playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}