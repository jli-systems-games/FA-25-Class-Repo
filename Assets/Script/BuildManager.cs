using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class BuildManager : MonoBehaviour
{
    public Camera cam;
    public LayerMask blockMask;
    public float gridSize = 1f;
    public float overlapEpsilon = 0.01f;
    public AudioClip placeClip;
    public float lineZ = 0f;
    public float baseY = 0f;
    public KeyCode deleteKey = KeyCode.X;
    public GameObject currentPrefab;
    public Material ghostOkMat;
    public Material ghostBadMat;
    public string corePrefabName = "Core";
    private GameObject coreInstance;
    public int pieceBudget = 10;
    public TMP_Text budgetTMP;
    private int usedNonCoreCount = 0;
    public string playSceneName = "Play";

    private GameObject ghost;
    private MeshRenderer[] ghostRenderers;
    private BlockTag ghostTag;
    private Quaternion ghostRot = Quaternion.identity;
    private int quarterRot = 0;
    private readonly List<Rigidbody> placedBlocks = new();
    private readonly List<GameObject> placedRoots  = new();

    void Start()
    {
        if (!cam) cam = Camera.main;
        TrySpawnGhost();
        RefreshHUD();
    }

    void Update()
    {
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
        {
            UpdateGhostVisible(false);
            return;
        }

        if (!cam || !currentPrefab)
        {
            UpdateGhostVisible(false);
            TryDeleteUnderCursor(false);
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane xyPlaneAtZ = new Plane(Vector3.forward, new Vector3(0f, 0f, lineZ));
        if (!xyPlaneAtZ.Raycast(ray, out float enter))
        {
            UpdateGhostVisible(false);
            TryDeleteUnderCursor(false);
            return;
        }

        Vector3 hitPoint = ray.GetPoint(enter); // 只取 X、Y
        Vector3Int size = ghostTag ? ghostTag.size : Vector3Int.one;

        int sizeX = ((quarterRot & 1) == 1) ? size.y : size.x;
        int sizeY = ((quarterRot & 1) == 1) ? size.x : size.y;

        float offX = (sizeX % 2 == 0) ? 0.5f * gridSize : 0f;
        float snappedX = Mathf.Round((hitPoint.x - offX) / gridSize) * gridSize + offX;

        float halfH    = 0.5f * sizeY * gridSize;
        float bottomY  = Mathf.Round((hitPoint.y - halfH) / gridSize) * gridSize;
        float minBottomY = Mathf.Ceil(baseY / gridSize) * gridSize;
        if (bottomY < minBottomY) bottomY = minBottomY;
        float snappedY = bottomY + halfH;

        Vector3 targetPos = new(snappedX, snappedY, lineZ);

        bool valid = !IsOverlap(targetPos, ghostRot, size);

        bool ghostIsCore = IsCorePrefab(currentPrefab);
        if (ghostIsCore && coreInstance != null) valid = false;
        if (!ghostIsCore && usedNonCoreCount >= pieceBudget) valid = false;

        UpdateGhostVisible(true);
        SetGhostMaterial(valid ? ghostOkMat : ghostBadMat);
        ghost.transform.SetPositionAndRotation(targetPos, ghostRot);

        if (Input.GetMouseButtonDown(0) && valid)
        {
            var go = Instantiate(currentPrefab, targetPos, ghostRot);
            go.layer = LayerMask.NameToLayer("Block");
            AudioSource.PlayClipAtPoint(placeClip, targetPos);

            var rb = go.GetComponent<Rigidbody>();
            if (!rb) rb = go.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity  = true;

            placedBlocks.Add(rb);
            placedRoots.Add(go);

            bool isCore = (go.GetComponent<CoreTag>() != null) || IsCorePrefab(go);
            if (isCore)
            {
                coreInstance = go;
                if (!go.GetComponent<CoreTag>()) go.AddComponent<CoreTag>();
            }
            else
            {
                usedNonCoreCount++;
            }

            RefreshHUD();
        }

        TryDeleteUnderCursor(Input.GetKeyDown(deleteKey));
    }

    bool IsCorePrefab(GameObject go)
    {
        if (!go) return false;
        if (go.GetComponent<CoreTag>() != null) return true;
        var name = go.name.Replace("(Clone)", "").Trim();
        return name == corePrefabName;
    }

    bool IsOverlap(Vector3 center, Quaternion rot, Vector3Int size)
    {
        Vector3 half = 0.5f * new Vector3(size.x * gridSize, size.y * gridSize, size.z * gridSize) - Vector3.one * overlapEpsilon;
        var cols = Physics.OverlapBox(center, half, rot, blockMask, QueryTriggerInteraction.Ignore);
        return cols.Length > 0;
    }

    void TrySpawnGhost()
    {
        if (!currentPrefab) { DestroyGhost(); return; }

        DestroyGhost();
        ghost = Instantiate(currentPrefab);
        ghost.name = "[GHOST] " + currentPrefab.name;
        ghost.layer = LayerMask.NameToLayer("Ignore Raycast");

        foreach (var c in ghost.GetComponentsInChildren<Collider>()) c.enabled = false;
        foreach (var rb in ghost.GetComponentsInChildren<Rigidbody>()) Destroy(rb);

        ghostRenderers = ghost.GetComponentsInChildren<MeshRenderer>();
        ghostTag = ghost.GetComponent<BlockTag>() ?? ghost.AddComponent<BlockTag>();
        SetGhostMaterial(ghostOkMat);

        ghostRot = Quaternion.identity;
        quarterRot = 0;
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
        if (!ghost) { if (currentPrefab && show) TrySpawnGhost(); return; }
        if (ghostRenderers == null) return;
        foreach (var r in ghostRenderers) r.enabled = show;
    }

    void SetGhostMaterial(Material m)
    {
        if (ghostRenderers == null || m == null) return;
        foreach (var r in ghostRenderers)
        {
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++) mats[i] = m;
            r.sharedMaterials = mats;
        }
    }

    void TryDeleteUnderCursor(bool doDelete)
    {
        Ray ray = cam ? cam.ScreenPointToRay(Input.mousePosition) : new Ray();
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, blockMask))
        {
            if (doDelete)
            {
                var rb = hit.rigidbody;
                var go = rb ? rb.gameObject : hit.collider.gameObject;

                placedBlocks.Remove(rb);
                placedRoots.Remove(go);

                bool isCore = go.GetComponent<CoreTag>() != null || IsCorePrefab(go);
                if (isCore)
                {
                    coreInstance = null;
                }
                else
                {
                    usedNonCoreCount = Mathf.Max(0, usedNonCoreCount - 1);
                }

                Destroy(go);
                RefreshHUD();
            }
        }
    }

    void RefreshHUD()
    {
        if (budgetTMP)
        {
            int remain = Mathf.Max(0, pieceBudget - usedNonCoreCount);
            budgetTMP.text = $"{remain}";
        }
    }

    public void SelectPrefab(GameObject prefab)
    {
        currentPrefab = prefab;
        TrySpawnGhost();
    }

   public void SaveBuild()
{
    var blocks = new List<BlockData>();
    foreach (var rb in FindObjectsByType<Rigidbody>(FindObjectsSortMode.None))
    {
        if (rb.gameObject.layer == LayerMask.NameToLayer("Block"))
        {
            BlockData b = new BlockData
            {
                prefabName = rb.name.Replace("(Clone)", "").Trim(),
                position = rb.transform.position,
                rotation = rb.transform.rotation
            };
            blocks.Add(b);
        }
    }
    SaveLoad.Save(blocks);
}

    public void SaveAndSwitchToPlay()
    {
        SaveBuild();
        SceneManager.LoadScene(playSceneName);
    }
}