using UnityEngine;
using System.Collections.Generic;

public class JournalController : MonoBehaviour
{
    public List<JournalPage> pages;
    int index = 0;
    public JournalPage CurrentPage => pages[index];

    void OnEnable() { ShowOnly(index); }

    public void NextPage() { index = Mathf.Min(index + 1, pages.Count - 1); ShowOnly(index); }
    public void PrevPage() { index = Mathf.Max(index - 1, 0); ShowOnly(index); }

    void ShowOnly(int i)
    {
        for (int k = 0; k < pages.Count; k++) pages[k].gameObject.SetActive(k == i);
    }

    public void SaveAll() { SaveSystem.Save(pages, index, 0); }
    public void LoadAll()
    {
        if (SaveSystem.Load(pages, IdToSprite, out var cur, out _))
        {
            index = cur; ShowOnly(index);
        }
    }

    public System.Func<string, Sprite> IdToSprite;
}
