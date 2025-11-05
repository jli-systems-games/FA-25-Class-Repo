using UnityEngine;

public enum StickerCategory { Deco, Washi, Frame, Emoji, Note, Sparkle }
public enum Rarity { Common, Rare, Epic }

[CreateAssetMenu(menuName = "Journal/StickerEntry")]
public class StickerEntry : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public StickerCategory category;
    public Rarity rarity;
    [Range(0f, 1f)] public float weight = 1f;
    public Color? tint;
}
