using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    [Serializable]
    public class Item
    {
        public string id;
        public Vector3 pos;
        public Quaternion rot;
    }

    [Serializable]
    public class Data { public List<Item> items = new(); }

    public string fileName = "build.json";
    public List<GameObject> blockPrefabs; // 对应不同 id 的预制体
    Dictionary<string, GameObject> map;

    void Awake()
    {
        map = new();
        foreach (var p in blockPrefabs)
        {
            var tag = p.GetComponent<BlockTag>();
            if (tag) map[tag.id] = p;
        }
    }

    public void Save()
    {
        var data = new Data();
        foreach (var b in GameObject.FindObjectsOfType<BlockTag>())
        {
            if (b.gameObject.scene.IsValid())
            {
                data.items.Add(new Item { id = b.id, pos = b.transform.position, rot = b.transform.rotation });
            }
        }
        var json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileName, json);
#if UNITY_EDITOR
        Debug.Log("Saved: " + Application.persistentDataPath + "/" + fileName);
#endif
    }

    public void Load()
    {
        var path = Application.persistentDataPath + "/" + fileName;
        if (!System.IO.File.Exists(path)) return;

        // 清理旧方块
        foreach (var b in GameObject.FindObjectsOfType<BlockTag>())
        {
            Destroy(b.gameObject);
        }

        var json = System.IO.File.ReadAllText(path);
        var data = JsonUtility.FromJson<Data>(json);
        foreach (var it in data.items)
        {
            if (map.TryGetValue(it.id, out var prefab))
            {
                var go = Instantiate(prefab, it.pos, it.rot);
                go.layer = LayerMask.NameToLayer("Block");
            }
        }
#if UNITY_EDITOR
        Debug.Log("Loaded: " + path);
#endif
    }
}