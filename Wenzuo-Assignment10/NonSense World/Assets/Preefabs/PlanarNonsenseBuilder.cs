using System.Collections.Generic;
using UnityEngine;

public class PlanarNonsenseBuilder : MonoBehaviour
{
    [Header("Palette")]
    public PrefabPalette palette;

    [Header("Grid")]
    public int width = 12;
    public int height = 12;

    [Tooltip("用于估算地砖/碰撞盒的尺寸，不一定等于实际摆放间距")]
    public float tileSize = 10f;

    [Tooltip("实际摆放步长。设得比 tileSize 小→更紧凑；比如 tileSize=10, step=7.5")]
    public float step = 7.5f;

    [Range(0f, 0.6f), Tooltip("每个tile随机±抖动，避免过于整齐")]
    public float jitter = 0.4f;

    public GameObject fallbackFloorPrefab;

    [Header("Noise & Gaps")]
    [Range(0f, 0.3f)] public float gapChance = 0.06f;
    [Range(0f, 0.6f)] public float bumpChance = 0.18f;
    public float bumpHeight = 1.2f;

    [Header("Cross Roads / Inserted Things")]
    public int roadLines = 6; // 稍微多一点更密
    [Range(0f, 1f)] public float buildingFill = 0.28f;
    public float buildingYOffset = 0.0f;

    [Header("Colliders")]
    public bool addBoxColliderIfMissing = true;
    public float walkTopOffset = 0.05f;

    [Header("Rare Copies")]
    public int rareCopiesPerLevel = 5;
    public Vector2 rareOffsetXZ = new Vector2(0.6f, 0.6f);
    public float rareOffsetY = 0.4f;

    [Header("Debug")]
    public bool showGizmos = false;

    public Vector3 FirstPlatformCenter => firstCenter;
    public GameObject currentRarePrefab;
    public readonly List<GameObject> activeRares = new();

    System.Random rng = new System.Random();
    readonly List<Transform> tiles = new();
    Vector3 firstCenter;

    [ContextMenu("Rebuild")]
    public void Rebuild()
    {
        Clear();
        if (!palette) { Debug.LogError("Palette 未设置"); return; }

        var floorsParent = new GameObject("Floors").transform; floorsParent.SetParent(transform, false);
        var floorPf = ChooseFloorPrefab();

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (rng.NextDouble() < gapChance) continue;

                Vector3 basePos = new Vector3(x * step, 0f, y * step);
                basePos += new Vector3(Rand(-jitter, jitter), 0f, Rand(-jitter, jitter));

                float bump = (rng.NextDouble() < bumpChance) ? (rng.Next(0, 2) == 0 ? bumpHeight : -bumpHeight * 0.5f) : 0f;
                var t = SpawnPlatform(floorPf, basePos + Vector3.up * bump, Quaternion.identity, floorsParent, out _);
                tiles.Add(t);
                if (x == 0 && y == 0) firstCenter = GetTopCenter(t);
            }

        var roadsParent = new GameObject("CrossRoads").transform; roadsParent.SetParent(transform, false);
        var roadPf = ChooseRoadPrefab();

        for (int i = 0; i < roadLines; i++)
        {
            bool horizontal = rng.Next(0, 2) == 0;
            int line = horizontal ? rng.Next(0, height) : rng.Next(0, width);
            float yLift = (rng.NextDouble() < 0.5) ? 0f : bumpHeight;

            int count = horizontal ? width : height;
            for (int k = 0; k < count; k++)
            {
                int x = horizontal ? k : line;
                int y = horizontal ? line : k;
                Vector3 pos = new Vector3(x * step, yLift, y * step);
                pos += new Vector3(Rand(-jitter, jitter), 0f, Rand(-jitter, jitter));
                SpawnPlatform(roadPf, pos, Quaternion.identity, roadsParent, out _);
            }
        }

        var stuffParent = new GameObject("InsertedStuff").transform; stuffParent.SetParent(transform, false);
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (rng.NextDouble() > buildingFill) continue;
                var pf = Pick(palette.platformPrefabs);
                Vector3 pos = new Vector3(x * step, buildingYOffset, y * step)
                            + new Vector3(Rand(-jitter, jitter), 0f, Rand(-jitter, jitter));
                Quaternion rot = Quaternion.Euler(0, rng.Next(0, 360), 0);
                var go = Instantiate(pf, pos, rot, stuffParent);
                go.name = $"Thing_{pf.name}";
                if (addBoxColliderIfMissing && go.GetComponentInChildren<Collider>() == null)
                {
                    var r = go.GetComponentInChildren<Renderer>();
                    if (r)
                    {
                        var col = go.AddComponent<BoxCollider>();
                        col.center = go.transform.InverseTransformPoint(r.bounds.center);
                        col.size = r.bounds.size + Vector3.up * 0.2f;
                    }
                }
            }

        SpawnRareSingleForPlayer();
    }

    public void SetRarePrefab(GameObject rarePrefab) => currentRarePrefab = rarePrefab;

    // —— 稀有件：只生成 1 个，并等高玩家 —— //
    public void SpawnRareSingleForPlayer(float playerHeightMeters = -1f)
    {
        // 清掉旧的
        foreach (var go in activeRares) if (go) DestroyImmediate(go);
        activeRares.Clear();

        // 兜底：如果没指定 rarePrefab，就从 palette 里选一个
        if (!currentRarePrefab)
        {
            if (palette?.rarePrefabs != null && palette.rarePrefabs.Count > 0)
                currentRarePrefab = Pick(palette.rarePrefabs);
            else if (palette?.platformPrefabs != null && palette.platformPrefabs.Count > 0)
                currentRarePrefab = Pick(palette.platformPrefabs);
        }
        if (!currentRarePrefab || tiles.Count == 0) return;

        // 随机挑一块 tile 的顶部中心
        int idx = rng.Next(tiles.Count);
        var t = tiles[idx];
        var top = GetTopCenter(t);

        // 稍微偏移一下，避免正中间太死板
        Vector3 pos = top + new Vector3(Rand(-rareOffsetXZ.x, rareOffsetXZ.x), rareOffsetY, Rand(-rareOffsetXZ.y, rareOffsetXZ.y));
        var rare = Instantiate(currentRarePrefab, pos, Quaternion.Euler(0, rng.Next(0, 360), 0), t);
        rare.name = $"Rare_{currentRarePrefab.name}";
        if (!rare.GetComponent<RareObjective>()) rare.AddComponent<RareObjective>();

        // 按玩家身高缩放
        float targetH = playerHeightMeters > 0 ? playerHeightMeters : GuessPlayerHeight();
        FitToTargetHeight(rare, targetH, 0.25f, 6f);

        activeRares.Add(rare);
    }

    // 估算玩家身高（找 Player 标签下的 CapsuleCollider），找不到默认 1.8m
    float GuessPlayerHeight()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            var cap = player.GetComponentInChildren<CapsuleCollider>();
            if (cap) return cap.height * cap.transform.lossyScale.y;
        }
        return 1.8f;
    }

    // 把任意物体缩放到指定高度（保持等比）
    static void FitToTargetHeight(GameObject go, float targetHeight, float minScale, float maxScale)
    {
        var rend = go.GetComponentInChildren<Renderer>();
        if (!rend) return;

        var b = rend.bounds;
        float h = Mathf.Max(0.0001f, b.size.y);
        float mul = targetHeight / h;

        // 把缩放从 world 转回 local：直接在 root 上等比缩放通常足够
        Vector3 s = go.transform.localScale * mul;
        float clamp = Mathf.Clamp(s.y, minScale, maxScale);
        float ratio = clamp / s.y;
        go.transform.localScale = s * ratio;
    }


    // —— 工具 —— //
    Transform SpawnPlatform(GameObject pf, Vector3 pos, Quaternion rot, Transform parent, out Vector3 topCenter)
    {
        GameObject go;
        if (pf)
        {
            go = Instantiate(pf, pos, rot, parent);
            go.name = $"Tile_{pf.name}";
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.SetParent(parent, false);
            go.transform.SetPositionAndRotation(pos, rot);
            go.transform.localScale = new Vector3(tileSize, 0.2f, tileSize);
            var mr = go.GetComponent<MeshRenderer>(); if (mr) mr.enabled = false;
            go.name = "Tile_Cube";
        }

        if (addBoxColliderIfMissing && go.GetComponentInChildren<Collider>() == null)
        {
            var r = go.GetComponentInChildren<Renderer>();
            if (r)
            {
                var col = go.AddComponent<BoxCollider>();
                col.center = go.transform.InverseTransformPoint(r.bounds.center);
                col.size = r.bounds.size + Vector3.up * 0.2f;
            }
        }

        var rend = go.GetComponentInChildren<Renderer>();
        var b = rend ? rend.bounds : new Bounds(pos, new Vector3(tileSize, 0.2f, tileSize));
        topCenter = new Vector3(b.center.x, b.max.y + walkTopOffset, b.center.z);
        return go.transform;
    }

    Vector3 GetTopCenter(Transform t)
    {
        var r = t.GetComponentInChildren<Renderer>();
        var b = r ? r.bounds : new Bounds(t.position, Vector3.one);
        return new Vector3(b.center.x, b.max.y + walkTopOffset, b.center.z);
    }

    GameObject ChooseFloorPrefab()
    {
        if (palette?.platformPrefabs != null)
        {
            foreach (var p in palette.platformPrefabs)
            {
                var n = p.name.ToLower();
                if (n.Contains("floor") || n.Contains("tile") || n.Contains("road") || n.Contains("plane"))
                    return p;
            }
        }
        return fallbackFloorPrefab;
    }

    GameObject ChooseRoadPrefab()
    {
        if (palette?.platformPrefabs != null)
        {
            foreach (var p in palette.platformPrefabs)
            {
                var n = p.name.ToLower();
                if (n.Contains("road") || n.Contains("bridge") || n.Contains("platform"))
                    return p;
            }
        }
        return null;
    }

    float Rand(float a, float b) => (float)(rng.NextDouble() * (b - a) + a);
    GameObject Pick(List<GameObject> list) => list[rng.Next(list.Count)];

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);
        tiles.Clear();
        activeRares.Clear();
        currentRarePrefab = null;
        firstCenter = transform.position + Vector3.up * 0.2f;
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3((width - 1) * step * 0.5f, 0, (height - 1) * step * 0.5f),
                            new Vector3(width * step, 0.1f, height * step));
    }
}
