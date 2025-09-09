using UnityEngine;
[CreateAssetMenu(fileName = "NPC", menuName = "Scriptable Objects/NPC")]

public class NPC : ScriptableObject
{ 
 
    public string npcName;
    public Sprite npcSprite;
    public int npcID;
    public int npcLevel;
    public int npcStr, npcDex, npcCon;

    public NPCStats npcStats;

    
    public int npcHP;

    
    public void CalculateStats()
    {
        npcHP = npcCon * npcLevel;
    }

    
    public void UpdateHP(int hpChange)
    {
        npcHP += hpChange;
    }
}


public struct NPCStats
{
    
    public string npcName;
    public Sprite npcSprite;
    public int npcID;
    public int npcLevel;
    public int npcStr, npcDex, npcCon;
}
