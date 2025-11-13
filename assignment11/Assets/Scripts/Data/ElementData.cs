using UnityEngine;

[CreateAssetMenu(fileName = "NewElementData", menuName = "Battle/Element Data")]
public class ElementData : ScriptableObject
{
    public string displayName;
    public ElementType elementType;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float attack = 20f;
    public float defense = 5f;
    [Tooltip("Attacks per second baseline")]
    public float speed = 1f;

    [Header("Matchups")]
    public ElementType strengthAgainst;
    public ElementType weakAgainst;

    [Header("Presentation")]
    public Color color = Color.white;
    public Sprite icon;
    public Sprite bodySprite;
    public GameObject attackVFXPrefab;
    public AudioClip hitSfx;
}
