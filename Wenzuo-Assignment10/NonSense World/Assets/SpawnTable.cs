using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City/Spawn Table")]
public class SpawnTable : ScriptableObject
{
    [Serializable] public struct Entry { public GameObject prefab; public float weight; }
    public List<Entry> entries = new();

    public GameObject Pick(System.Random rng)
    {
        if (entries.Count == 0) return null;
        float sum = 0f; foreach (var e in entries) sum += Mathf.Max(0.0001f, e.weight);
        float t = (float)rng.NextDouble() * sum;
        foreach (var e in entries)
        {
            t -= Mathf.Max(0.0001f, e.weight);
            if (t <= 0f) return e.prefab;
        }
        return entries[^1].prefab;
    }
}
