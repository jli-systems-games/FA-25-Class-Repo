using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class JournalSave
{
    public List<List<StickerState>> pages = new();
    public int currentPage;
    public int seedUsed;
}

public static class SaveSystem
{
    const string KEY = "JournalSave_v1";

    public static void Save(List<JournalPage> pageRefs, int currentPage, int seedUsed)
    {
        var save = new JournalSave { currentPage = currentPage, seedUsed = seedUsed };
        foreach (var p in pageRefs)
        {
            p.SaveSnapshot();
            save.pages.Add(new List<StickerState>(p.states));
        }
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(save));
        PlayerPrefs.Save();
    }

    public static bool Load(List<JournalPage> pageRefs, System.Func<string, Sprite> idToSprite, out int currentPage, out int seedUsed)
    {
        currentPage = 0; seedUsed = 0;
        if (!PlayerPrefs.HasKey(KEY)) return false;
        var save = JsonUtility.FromJson<JournalSave>(PlayerPrefs.GetString(KEY));
        for (int i = 0; i < Mathf.Min(pageRefs.Count, save.pages.Count); i++)
        {
            pageRefs[i].states = save.pages[i];
            pageRefs[i].LoadFromSnapshot(idToSprite);
        }
        currentPage = save.currentPage; seedUsed = save.seedUsed;
        return true;
    }
}
