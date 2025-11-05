using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StickerDB : MonoBehaviour
{
    public StickerSet set;
    Dictionary<string, Sprite> map;
    void Awake()
    {
        map = set.entries.ToDictionary(e => e.id, e => e.sprite);
        FindObjectOfType<JournalController>().IdToSprite = (id) =>
            map.TryGetValue(id, out var sp) ? sp : null;
    }
}
