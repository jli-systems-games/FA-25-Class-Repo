using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoad
{
    private static string filePath => Path.Combine(Application.persistentDataPath, "buildData.json");

    [System.Serializable]
    private class Wrapper
    {
        public List<BlockData> blocks;
    }

    public static void Save(List<BlockData> blocks)
    {
        Wrapper wrap = new Wrapper { blocks = blocks };
        string json = JsonUtility.ToJson(wrap, true);
        File.WriteAllText(filePath, json);
    }

    public static List<BlockData> Load()
    {
        if (!File.Exists(filePath))
        {
            return new List<BlockData>();
        }

        string json = File.ReadAllText(filePath);
        Wrapper wrap = JsonUtility.FromJson<Wrapper>(json);
        return wrap.blocks;
    }
}