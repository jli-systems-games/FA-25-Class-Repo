using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "Scriptable Objects/NPC")]
//Unity6 sets this up for us when we make a SCriptableObject script, but be aware you'll need this
//if you convert a script later on if you want to make them in your asset folder
public class NPC : ScriptableObject //rather than "inheriting" from MonoBehaviour,
                                    //you will inherit from ScriptableObject
                                    //you won't be able to attach this to a GameObject or use some built-in
                                    //functions like "Start", but you can create these as data sets in your assets
{
    //these are variables all objects of type "NPC" have:
    public string npcName;
    public Sprite npcSprite;
    public int npcID;
    public int npcLevel;
    public int npcStr, npcDex, npcCon;

    //we CAN also just have all these stats in our struct and make a variable of it!
    //BUT, because these have a bunch of different variables, you will not be able
    //to edit this in Unity editor
    //this is more useful if you are copying stat blocks between object instances
    //so that you only need to send a single variable between scripts instead of every little stat
    public NPCStats npcStats;

    //here is a stat I will calculate later
    public int npcHP;

    //one of the powers of "ScriptableObjects" is they are more than a stat block
    //you can add functions to a ScriptableObjects
    public void CalculateStats()
    {
        npcHP = npcCon * npcLevel;
    }

    //you can also bake behaviour into a scriptable object, like a- 
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