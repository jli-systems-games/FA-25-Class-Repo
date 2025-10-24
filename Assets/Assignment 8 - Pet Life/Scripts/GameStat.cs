using UnityEngine;

[CreateAssetMenu(fileName = "GameStat", menuName = "Scriptable Objects/GameStat")]
public class GameStat : ScriptableObject
{
    public float defaultDirt = 100f;
    public float defaultHunger = 50f;
    public float defaultPlay = 50f;

    public float dirtStat;
    public float hungerStat;
    public float playStat;

    public void ResetStats()
    {
        dirtStat = defaultDirt;
        hungerStat = defaultHunger;
        playStat = defaultPlay;
    }
}
