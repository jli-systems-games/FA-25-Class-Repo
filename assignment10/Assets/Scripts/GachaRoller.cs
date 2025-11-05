using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class GachaRoller : MonoBehaviour
{
    public StickerSet set;
    public int seed = 0;
    System.Random rng;

    void Awake()
    {
        int s = seed != 0 ? seed : Environment.TickCount ^ GetInstanceID();
        rng = new System.Random(s);
    }

    public List<StickerEntry> RollFiveUnique()
    {
        var pool = set.entries.ToList();
        var picks = new List<StickerEntry>();
        for (int i = 0; i < 5 && pool.Count > 0; i++)
        {
            var chosen = WeightedPick(pool);
            picks.Add(chosen);
            pool.Remove(chosen);
        }
        return picks;
    }

    StickerEntry WeightedPick(List<StickerEntry> pool)
    {
        float Score(StickerEntry e)
        {
            float rW = e.rarity switch { Rarity.Common => set.wCommon, Rarity.Rare => set.wRare, _ => set.wEpic };
            float cW = e.category switch
            {
                StickerCategory.Deco => set.decoBias,
                StickerCategory.Washi => set.washiBias,
                StickerCategory.Frame => set.frameBias,
                StickerCategory.Emoji => set.emojiBias,
                StickerCategory.Note => set.noteBias,
                _ => set.sparkleBias
            };
            return Mathf.Max(0.0001f, e.weight * rW * cW);
        }
        float sum = pool.Sum(Score);
        float r = (float)rng.NextDouble() * sum;
        float acc = 0f;
        foreach (var e in pool)
        {
            acc += Score(e);
            if (r <= acc) return e;
        }
        return pool[pool.Count - 1];
    }
}
