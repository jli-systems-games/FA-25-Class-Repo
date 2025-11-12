using System;
using System.Collections.Generic;
using UnityEngine;


public class RareLandmarkSpawner : MonoBehaviour
{
    public List<GameObject> landmarkPrefabs = new();
    [Range(0f, 1f)] public float chance = 0.02f;
    public int attempts = 0; // 留0即可

    static RareLandmarkSpawner inst;
    void Awake() { inst = this; }

    public static void SpawnAll()
    {
        if (inst == null) return;

        // 尝试从场景中的 CityTileAutoBind 拿到 Palette 的地标列表
        if (inst.landmarkPrefabs.Count == 0)
        {
            var binder = UnityEngine.Object.FindFirstObjectByType<CityTileAutoBind>();

            if (binder && binder.palette && binder.palette.landmarks != null)
                inst.landmarkPrefabs = binder.palette.landmarks;
        }

        if (inst.landmarkPrefabs == null || inst.landmarkPrefabs.Count == 0) return;
        var rng = new System.Random(CityTile.Seed + 999);
        foreach (var kv in CityTile.Tiles)
            if ((inst.attempts > 0 && rng.Next(inst.attempts) > 0) || rng.NextDouble() < inst.chance)
                Drop(inst, kv.Value.transform.position, rng);
    }
    static void Drop(RareLandmarkSpawner s, Vector3 pos, System.Random rng)
    {
        var p = s.landmarkPrefabs[rng.Next(s.landmarkPrefabs.Count)];
        var go = GameObject.Instantiate(p, pos, Quaternion.Euler(0, rng.Next(0, 360), 0));
        if (!go.CompareTag("Weird")) go.tag = "Weird";
    }
}
