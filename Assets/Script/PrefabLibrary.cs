using System.Collections.Generic;
using UnityEngine;

public class PrefabLibrary : MonoBehaviour
{
    public List<GameObject> prefabs = new List<GameObject>();

    public GameObject Get(string name)
    {
        name = name.Replace("(Clone)", "").Trim();
        foreach (var p in prefabs)
        {
            if (p && p.name == name) return p;
        }
        return null;
    }
}