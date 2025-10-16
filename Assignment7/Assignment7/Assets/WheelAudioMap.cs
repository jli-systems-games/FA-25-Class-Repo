using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WheelAudioPairBySprite
{
    public Sprite sprite;
    public AudioClip selectClip;
    public AudioClip runClip;
}

public class WheelAudioMap : MonoBehaviour
{
    public List<WheelAudioPairBySprite> items = new List<WheelAudioPairBySprite>();

    public bool TryGetClips(Sprite sprite, out AudioClip select, out AudioClip run)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].sprite == sprite)
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
