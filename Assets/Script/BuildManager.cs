using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    [Header("基础设置")]
    public Camera cam;
    public LayerMask groundMask;  // 只包含 Ground
    public LayerMask blockMask;   // 只包含 Block
    public float gridSize = 3f;   // 网格大小
    public float planeY = 0f;     // 地面平面 y
    public float overlapEpsilon = 0.01f;

    [Header("放置")]
    public GameObject currentPrefab;
    public Material ghostOkMat;
    public Material ghostBadMat;
    public KeyCode rotateKey = KeyCode.R;
    public KeyCode upKey = KeyCode.E;
    public KeyCode downKey = KeyCode.Q;
    public KeyCode deleteKey = KeyCode.X;
    public KeyCode toggleSnapKey = KeyCode.G;

    [Header("模式")]
    public bool faceSnap = true; // true=贴面吸附 ; false=网格吸附（地面）
    public int heightOffset = 0; // 垂直层偏移（以格为单位）

    GameObject ghost;
    MeshRenderer[] ghostRenderers;
    BlockTag ghostTag;
    Quaternion ghostRot = Quaternion.identity;

    void Start()
    {
        if (!cam) cam = Camera.main;
        SpawnGhost();
    }

    void Update()
    {
        // UI 上方不响应放置/删除
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
        {
            UpdateGhostVisible(false);
            return;
        }

        // 模式切换 / 旋转 / 高度层
        if (Input.GetKeyDown(toggleSnapKey)) faceSnap = !faceSnap;
        if (Input.GetKeyDown(rotateKey)) ghostRot *= Quaternion.Euler(0, 90, 0);
        if (Input.GetKeyDown(upKey)) heightOffset++;
        if (Input.GetKeyDown(downKey)) heightOffset = Mathf.Max(0, heightOffset - 1);

        // 光标射线
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hitInfo, 500f, faceSnap ? (groundMask | blockMask) : (groundMask));

        if (!hitSomething)
        {
            UpdateGhostVisible(false);
            TryDeleteUnderCursor(false);
            return;
        }

        // 计算目标位置
        Vector3 targetPos;
        Quaternion targetRot = ghostRot; // 基于当前旋转
        Vector3Int blockSize = ghostTag ? ghostTag.size : Vector3Int.one;
        bool valid;

        if (faceSnap && ((1 << hitInfo.collider.gameObject.layer) & blockMask) != 0)
        {
            // 贴已有方块表面（按射线法线）
            var targetBlock = hitInfo.collider.GetComponentInParent<BlockTag>();
            var normal = hitInfo.normal; // 指向外的法线（世界系）

            // 先取目标块的世界对齐
            Vector3 baseCenter = hitInfo.collider.bounds.center;

            // 以法线方向把方块贴到面上：半块厚 + 对方半厚
            Vector3 half = 0.5f * new Vector3(blockSize.x * gridSize, blockSize.y * gridSize, blockSize.z * gridSize);
            float push = Vector3.Scale(half, new Vector3(Mathf.Abs(normal.x), Mathf.Abs(normal.y), Mathf.Abs(normal.z))).magnitude;

            // 对方块的半厚
            Vector3 otherHalf = 0.5f * hitInfo.collider.bounds.size;
            float pushOther = Vector3.Scale(otherHalf, new Vector3(Mathf.Abs(normal.x), Mathf.Abs(normal.y), Mathf.Abs(normal.z))).magnitude;

            targetPos = hitInfo.point + normal * (push + overlapEpsilon);
            // 吸回到与对方面中心同一“格”的对齐（在法线平面的两个轴分别取整）
            // 先把点投影到面切平面：找到两个正交切向
            Vector3 tangent, bitangent;
            ComputeTangentSpace(normal, out tangent, out bitangent);

            // 在切向坐标中做网格对齐
            float t = Vector3.Dot(targetPos, tangent) / gridSize;
            float b = Vector3.Dot(targetPos, bitangent) / gridSize;
            t = Mathf.Round(t);
            b = Mathf.Round(b);
            float n = Vector3.Dot(hitInfo.collider.bounds.center + normal * pushOther, normal) / gridSize; // 让法向基准对齐到对方面
            targetPos = tangent * (t * gridSize) + bitangent * (b * gridSize) + normal * (n * gridSize);

            // 叠层（只对 Y）
            targetPos += Vector3.up * (heightOffset * gridSize);

            valid = !IsOverlap(targetPos, targetRot, blockSize);
        }
        else
        {
            // 地面网格吸附
            Plane p = new Plane(Vector3.up, new Vector3(0, planeY, 0));
            float enter;
            if (!p.Raycast(ray, out enter))
            {
                UpdateGhostVisible(false);
                TryDeleteUnderCursor(false);
                return;
            }
            Vector3 hitPoint = ray.GetPoint(enter);

            // 网格对齐（中心对齐），加上层高
            targetPos = new Vector3(
                Mathf.Round(hitPoint.x / gridSize) * gridSize,
                planeY + heightOffset * gridSize,
                Mathf.Round(hitPoint.z / gridSize) * gridSize
            );

            valid = !IsOverlap(targetPos, targetRot, blockSize);
        }

        // 更新幽灵
        UpdateGhostVisible(true);
        ghost.transform.SetPositionAndRotation(targetPos, targetRot);
        SetGhostMaterial(valid ? ghostOkMat : ghostBadMat);

        // 放置
        if (Input.GetMouseButtonDown(0) && valid && currentPrefab)
        {
            var go = Instantiate(currentPrefab, targetPos, targetRot);
            go.layer = LayerMask.NameToLayer("Block");
        }

        // 删除（指向任何 Block）
        TryDeleteUnderCursor(Input.GetKeyDown(deleteKey));
    }

    void ComputeTangentSpace(Vector3 normal, out Vector3 tangent, out Vector3 bitangent)
    {
        // 寻个不平行向量
        Vector3 up = Mathf.Abs(Vector3.Dot(normal, Vector3.up)) > 0.9f ? Vector3.right : Vector3.up;
        tangent = Vector3.Normalize(Vector3.Cross(up, normal));
        bitangent = Vector3.Normalize(Vector3.Cross(normal, tangent));
    }

    bool IsOverlap(Vector3 center, Quaternion rot, Vector3Int size)
    {
        // 近似为对齐盒投影：使用旋转后的半长宽高
        Vector3 half = 0.5f * new Vector3(size.x * gridSize, size.y * gridSize, size.z * gridSize) - Vector3.one * overlapEpsilon;
        // 用 OverlapBox 检查与 Block 层重叠（忽略幽灵自身）
        var cols = Physics.OverlapBox(center, half, rot, blockMask, QueryTriggerInteraction.Ignore);
        return cols.Length > 0;
    }

    void SpawnGhost()
    {
        DestroyGhost();

        if (!currentPrefab) return;
        ghost = Instantiate(currentPrefab);
        ghost.name = "[GHOST] " + currentPrefab.name;
        ghost.layer = LayerMask.NameToLayer("Ignore Raycast"); // 避免自身被射线命中

        // 清理可能影响判断的组件（可选）
        foreach (var c in ghost.GetComponentsInChildren<Collider>()) c.enabled = false;
        foreach (var rb in ghost.GetComponentsInChildren<Rigidbody>()) Destroy(rb);

        ghostRenderers = ghost.GetComponentsInChildren<MeshRenderer>();
        ghostTag = ghost.GetComponent<BlockTag>();
        if (!ghostTag) ghostTag = ghost.AddComponent<BlockTag>(); // 兜底 1x1
        SetGhostMaterial(ghostOkMat);
    }

    void DestroyGhost()
    {
        if (ghost) Destroy(ghost);
        ghost = null;
        ghostRenderers = null;
        ghostTag = null;
    }

    void UpdateGhostVisible(bool show)
    {
        if (!ghost) { if (currentPrefab) SpawnGhost(); return; }
        if (ghostRenderers == null) return;
        foreach (var r in ghostRenderers) r.enabled = show;
    }

    void SetGhostMaterial(Material m)
    {
        if (ghostRenderers == null) return;
        foreach (var r in ghostRenderers)
        {
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++) mats[i] = m;
            r.sharedMaterials = mats;
        }
    }

    void TryDeleteUnderCursor(bool doDelete)
    {
        // 从鼠标射线检测 Block
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, blockMask))
        {
            // 高亮（可选：给被指向的方块换材质/描边）
            if (doDelete)
            {
                Destroy(hit.collider.attachedRigidbody ? hit.collider.attachedRigidbody.gameObject : hit.collider.gameObject);
            }
        }
    }

    // UI 调用：选择方块
    public void SelectPrefab(GameObject prefab)
    {
        currentPrefab = prefab;
        ghostRot = Quaternion.identity;
        SpawnGhost();
    }
}