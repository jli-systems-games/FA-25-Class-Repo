using System.Collections.Generic;
using UnityEngine;

public class CityTile : MonoBehaviour
{
    // —— 全局：生成表 —— //
    public static Dictionary<Vector2Int, CityTile> Tiles = new();
    public static HashSet<Vector2Int> RoadCells = new();
    public static System.Random Rng;
    [Header("Generation Params")]
    public static int Seed = 12345;
    [Range(0, 1f)] public float RoadThreshold = 0.55f;
    [Range(0, 1f)] public float ParkChance = 0.12f;
    [Range(0, 1f)] public float WaterChance = 0.08f;

    [Header("Prefabs")]
    public GameObject roadStraight, roadCorner, roadTJunction, roadCross, roadDeadEnd;
    public GameObject lotBuilding, lotPark, lotWater;

    enum CellKind { Road, Building, Park, Water }
    CellKind kind; Vector2Int grid;

    void Awake()
    {
        if (Rng == null) Rng = new System.Random(Seed);
        grid = new Vector2Int(Mathf.RoundToInt(transform.position.x),
                              Mathf.RoundToInt(transform.position.y));
        Tiles[grid] = this;

        // 阶段A：先分类，登记路
        float n = Mathf.PerlinNoise((grid.x + Seed) * 0.13f, (grid.y - Seed) * 0.13f);
        bool isRoad = n > RoadThreshold;
        if (isRoad) { kind = CellKind.Road; RoadCells.Add(grid); }
        else
        {
            float r = (float)Rng.NextDouble();
            if (r < WaterChance) kind = CellKind.Water;
            else if (r < WaterChance + ParkChance) kind = CellKind.Park;
            else kind = CellKind.Building;
        }
    }

    // 阶段B：所有格子生成完统一“成型”
    public static void BuildAll()
    {
        foreach (var kv in Tiles) kv.Value.BuildSelf();
    }

    void BuildSelf()
    {
        switch (kind)
        {
            case CellKind.Road: BuildRoad(); break;
            case CellKind.Building: BuildBuildingLot(); break;
            case CellKind.Park: Instantiate(lotPark, transform.position, Quaternion.identity, transform); break;
            case CellKind.Water: Instantiate(lotWater, transform.position, Quaternion.identity, transform); break;
        }
    }

    void BuildRoad()
    {
        bool n = RoadCells.Contains(grid + Vector2Int.up);
        bool s = RoadCells.Contains(grid + Vector2Int.down);
        bool e = RoadCells.Contains(grid + Vector2Int.right);
        bool w = RoadCells.Contains(grid + Vector2Int.left);

        int c = (n ? 1 : 0) + (s ? 1 : 0) + (e ? 1 : 0) + (w ? 1 : 0);
        GameObject prefab = roadDeadEnd; Quaternion rot = Quaternion.identity;

        if (c == 4) prefab = roadCross;
        else if (c == 3)
        {
            prefab = roadTJunction;
            if (!n) rot = Quaternion.Euler(0, 0, 180);
            else if (!s) rot = Quaternion.identity;
            else if (!e) rot = Quaternion.Euler(0, 0, 90);
            else rot = Quaternion.Euler(0, 0, -90);
        }
        else if (c == 2)
        {
            if ((n && s) || (e && w)) { prefab = roadStraight; rot = (e && w) ? Quaternion.Euler(0, 0, 90) : Quaternion.identity; }
            else
            {
                prefab = roadCorner;
                if (n && e) rot = Quaternion.identity;
                else if (e && s) rot = Quaternion.Euler(0, 0, 90);
                else if (s && w) rot = Quaternion.Euler(0, 0, 180);
                else rot = Quaternion.Euler(0, 0, 270);
            }
        }
        else if (c == 1)
        {
            prefab = roadDeadEnd;
            if (n) rot = Quaternion.identity;
            else if (e) rot = Quaternion.Euler(0, 0, 90);
            else if (s) rot = Quaternion.Euler(0, 0, 180);
            else rot = Quaternion.Euler(0, 0, 270);
        }
        Instantiate(prefab, transform.position, rot, transform);
    }

    void BuildBuildingLot()
    {
        var lot = Instantiate(lotBuilding, transform.position, Quaternion.identity, transform);
        // 楼高 + 颜色
        float h = Mathf.Lerp(0.8f, 3.5f, (float)Rng.NextDouble());
        var meshT = lot.GetComponentInChildren<Transform>();
        if (meshT) meshT.localScale = new Vector3(1f, h, 1f);
        var rend = lot.GetComponentInChildren<Renderer>();
        if (rend) rend.material.color = Color.HSVToRGB((float)Rng.NextDouble(), 0.4f, 0.9f);
        
      
    }
}
