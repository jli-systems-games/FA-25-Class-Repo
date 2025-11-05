using UnityEngine;

[CreateAssetMenu(menuName = "Journal/StickerSet")]
public class StickerSet : ScriptableObject
{
    public StickerEntry[] entries;
    [Header("Rarity Weights")]
    public float wCommon = 0.75f, wRare = 0.22f, wEpic = 0.03f;
    [Header("Theme Bias (可选)")]
    public float decoBias = 1f, washiBias = 1f, frameBias = 1f, emojiBias = 1f, noteBias = 1f, sparkleBias = 1f;
}
