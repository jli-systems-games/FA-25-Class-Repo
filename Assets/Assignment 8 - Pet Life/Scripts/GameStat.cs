using UnityEngine;

[CreateAssetMenu(fileName = "GameStat", menuName = "Scriptable Objects/GameStat")]
public class GameStat : ScriptableObject
{
    public float defaultDirt = 70f;
    public float defaultHunger = 50f;
    public float defaultGrowth = 0f;

    public float dirtStat;
    public float hungerStat;
    public float growthStat;

    public void ResetStats()
    {
        dirtStat = defaultDirt;
        hungerStat = defaultHunger;
        growthStat = defaultGrowth;
    }

    public void EndStats()
    {
        dirtStat = 100;
        hungerStat = 100;
        growthStat = 100;
    }
}
