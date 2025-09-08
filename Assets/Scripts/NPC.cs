using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "Scriptable Objects/NPC")]
public class NPC : ScriptableObject
{
    public string npcName;
    public Sprite npcSprite;
    public int npcLevel;
    public Color npcFavoriteColor;
    public Alignment npcAlignment = Alignment.Good;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public struct CharacterStats
{
    public string npcName;
    public Sprite npcSprite;

}

public enum Alignment
{
    Good,
    Evil,
    Chill
}
