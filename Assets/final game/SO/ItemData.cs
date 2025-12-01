using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;       
    public string displayName;
    public Sprite icon;
    public string description;
    public GameObject worldPrefab;
}
