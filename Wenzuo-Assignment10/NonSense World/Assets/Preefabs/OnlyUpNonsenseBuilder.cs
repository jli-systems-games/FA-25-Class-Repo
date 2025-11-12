using System.Collections.Generic;
using UnityEngine;

public class OnlyUpNonsenseBuilder : MonoBehaviour
{
    [Header("Palette")]
    public PrefabPalette palette;

    [Header("Main Path")]
    public int steps = 28;
    public float rise = 3.0f;
    public float gap = 4.0f;
    public float yawJitter = 25f;
    public Vector2 drift = new Vector2(1.5f, 1.5f);

    [Header("Branches")]
    public int sideChancePercent = 45;
    public int sideMin = 2, sideMax = 5;

    [Header("Colliders")]
    public bool addBoxColliderIfMissing = true;
    public float walkTopOffset = 0.05f;

    [Header("Rares Per Level")]
    public int rareCopiesPerLevel = 5;              // ← 每关复制几个目标
    public Vector2 rareOffsetXZ = new Vector2(0.6f, 0.6f); // 稍微随机偏移落点
    public float rareOffsetY = 0.4f;

    [Header("Debug")]
    public bool showMarkers = false;

    public Transform startMarker;
    readonly List<Transform> spawned = new();       // 所有平台 Transform
    readonly System.Random rng = new System.Random();

    // 当前关的“稀有件模板”和“实例集合”
    public GameObject currentRarePrefab;
    public readonly List<GameObject> activeRares = new();

    public Vector3 FirstPlatformCenter => startMarker ? startMarker.position : transform.position + Vector3.up * 1.2f;

    [ContextMenu("Rebuild")]
    public void Rebuild()
    {
        Clear();
        if (!palette || palette.platformPrefabs.Count == 0)
        {
            Debug.LogError("Palette 未配置或没有平台 Prefab！");
            return;
        }

        // —— A. 主路径 —— //
        Vector3 pos = transform.position; Quaternion rot = Quaternion.identity;
        Transform last = null;

        for (int i = 0; i < steps; i++)
        {
            var pf = Pick(palette.platformPrefabs);
            var t = SpawnPlatform(pf, pos, rot, out Vector3 topCenter);
            if (i == 0) startMarker = MakeMarker("Start", topCenter);

            if (last) MakeBridgeBetween(last, t);
            last = t;
            spawned.Add(t);

            rot = Quaternion.Euler(0, rot.eulerAngles.y + Rand(-yawJitter, yawJitter), 0);
            var forward = rot * Vector3.forward;
            pos = topCenter + forward * gap
                            + new Vector3(Rand(-drift.x, drift.x), 0, Rand(-drift.y, drift.y))
                            + Vector3.up * rise;

            // —— 支路 —— //
            if (rng.Next(100) < sideChancePercent)
            {
                int len = rng.Next(sideMin, sideMax + 1);
                Vector3 bpos = topCenter + (rot * Vector3.right) * Rand(-gap, gap);
                Quaternion brot = Quaternion.Euler(0, rot.eulerAngles.y + Rand(-60, 60), 0);
                Transform lastSide = t;
                for (int s = 0; s < len; s++)
                {
                    var spf = Pick(palette.platformPrefabs);
                    var st = SpawnPlatform(spf, bpos, brot, out Vector3 stop);
                    MakeBridgeBetween(lastSide, st);
                    spawned.Add(st); lastSide = st;
                    brot = Quaternion.Euler(0, brot.eulerAngles.y + Rand(-40, 40), 0);
                    bpos = stop + (brot * Vector3.forward) * gap + Vector3.up * rise * 0.6f;
                }
            }
        }

        // —— B. 多枚稀有件：在随机平台上放 N 个 —— //
        SpawnRareCopies();
    }

    // ========== 稀有件 ========== //
    public void SetRarePrefab(GameObject rarePrefab)
    {
        currentRarePrefab = rarePrefab;
    }

    public void SpawnRareCopies()
    {
        // 清掉旧的
        foreach (var go in activeRares) if (go) DestroyImmediate(go);
        activeRares.Clear();

        // 没有稀有件就从平台里挑一个看起来特别的顶替
        if (!currentRarePrefab)
        {
            if (palette.rarePrefabs != null && palette.rarePrefabs.Count > 0)
                currentRarePrefab = Pick(palette.rarePrefabs);
            else
                currentRarePrefab = Pick(palette.platformPrefabs);
        }

        if (spawned.Count == 0) return;

        // 随机挑若干不同平台作为落点（尽量不选第一块）
        var used = new HashSet<int>();
        for (int i = 0; i < rareCopiesPerLevel; i++)
        {
            int tries = 0;
            int idx;
            do { idx = rng.Next(spawned.Count); tries++; } while ((idx == 0 || used.Contains(idx)) && tries < 20);
            used.Add(idx);

            var t = spawned[idx];
            var top = GetTopCenter(t);
            var pos = top + new Vector3(Rand(-rareOffsetXZ.x, rareOffsetXZ.x), rareOffsetY, Rand(-rareOffsetXZ.y, rareOffsetXZ.y));

            var rare = Instantiate(currentRarePrefab, pos, Quaternion.Euler(0, rng.Next(0, 360), 0), t);
            rare.name = $"Rare_{currentRarePrefab.name}";
            if (!rare.GetComponent<RareObjective>()) rare.AddComponent<RareObjective>();
            activeRares.Add(rare);
        }
    }

    // ========== 工具 ========== //
    Transform SpawnPlatform(GameObject pf, Vector3 approxPos, Quaternion rot, out Vector3 topCenter)
    {
        var go = Instantiate(pf, approxPos, rot, transform);
        go.name = $"Platform_{pf.name}";

        var r = go.GetComponentInChildren<Renderer>();
        var b = r ? r.bounds : new Bounds(approxPos, Vector3.one * 2f);
        float topY = b.max.y + walkTopOffset;
        topCenter = new Vector3(b.center.x, topY, b.center.z);

        if (addBoxColliderIfMissing && go.GetComponentInChildren<Collider>() == null)
        {
            var col = go.AddComponent<BoxCollider>();
            col.center = go.transform.InverseTransformPoint(b.center);
            col.size = b.size + Vector3.up * 0.2f;
        }

        if (showMarkers) MakeMarker($"Top_{pf.name}", topCenter);
        return go.transform;
    }

    void MakeBridgeBetween(Transform a, Transform b)
    {
        Vector3 pa = GetTopCenter(a), pb = GetTopCenter(b);
        Vector3 mid = (pa + pb) * 0.5f;
        Vector3 dir = (pb - pa); float len = dir.magnitude;
        if (len < 0.2f) return;

        GameObject bridge;
        if (palette.bridgePrefab)
        {
            bridge = Instantiate(palette.bridgePrefab, mid, Quaternion.LookRotation(dir.normalized, Vector3.up), transform);
        }
        else
        {
            bridge = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bridge.transform.SetParent(transform, false);
            bridge.transform.SetPositionAndRotation(mid, Quaternion.LookRotation(dir.normalized, Vector3.up));
            bridge.transform.localScale = new Vector3(0.8f, 0.12f, len);
            var mr = bridge.GetComponent<MeshRenderer>(); if (mr) mr.enabled = false;
        }
        bridge.name = "Bridge";
        var bc = bridge.GetComponent<Collider>(); if (bc) bc.isTrigger = false;
    }

    Vector3 GetTopCenter(Transform t)
    {
        var r = t.GetComponentInChildren<Renderer>();
        var b = r ? r.bounds : new Bounds(t.position, Vector3.one);
        return new Vector3(b.center.x, b.max.y + walkTopOffset, b.center.z);
    }

    Transform MakeMarker(string n, Vector3 p)
    {
        var g = new GameObject(n).transform;
        g.SetParent(transform, false);
        g.position = p;
        if (showMarkers)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.SetParent(g, false);
            s.transform.localScale = Vector3.one * 0.2f;
            var mr = s.GetComponent<MeshRenderer>(); if (mr) mr.material.color = (n == "Start") ? Color.green : Color.yellow;
        }
        return g;
    }

    float Rand(float a, float b) => (float)(rng.NextDouble() * (b - a) + a);
    GameObject Pick(List<GameObject> list) => list[rng.Next(list.Count)];

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);
        spawned.Clear(); startMarker = null; activeRares.Clear(); currentRarePrefab = null;
    }
}
