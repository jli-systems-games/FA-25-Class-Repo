using UnityEngine;

public enum Rarity { Common, Rare, Legendary }


[System.Serializable]

public class Achievement
{
    public string title;
    public Rarity rarity;
    public int scoreReward;
}
