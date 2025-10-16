using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WheelAudioPair
{
    public string spriteName;
    public AudioClip selectClip;
    public AudioClip runClip;
}

public class WheelAudioMap : MonoBehaviour
{
    public List<WheelAudioPair> items = new List<WheelAudioPair>();
    public bool TryGetClips(string spriteName, out AudioClip select, out AudioClip run)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (string.Equals(items[i].spriteName, spriteName, StringComparison.OrdinalIgnoreCase))
            {
                select = items[i].selectClip;
                run = items[i].runClip ? items[i].runClip : items[i].selectClip;
                return true;
            }
        }
        select = null;
        run = null;
        return false;
    }
}
