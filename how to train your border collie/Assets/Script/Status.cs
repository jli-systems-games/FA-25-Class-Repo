using UnityEngine;

[CreateAssetMenu(fileName = "BorderCollieStats", menuName = "Border Collie Stats")]
public class BorderCollieStats : ScriptableObject
{
    [Range(0, 100)] public int Energy = 80;
    [Range(0, 100)] public int Happiness = 80;
    [Range(0, 100)] public int Fullness = 80;
    [Range(0, 100)] public int Skill = 0;

    [Header("Per-Second Deltas")]
    public ActionDelta sleepDelta = new ActionDelta { energy = +4, happiness = 0, fullness = 0, skill = 0 };
    public ActionDelta playDelta = new ActionDelta { energy = -2, happiness = +3, fullness = 0, skill = -1 };
    public ActionDelta eatDelta = new ActionDelta { energy = 0, happiness = 0, fullness = +5, skill = 0 };
    public ActionDelta trainDelta = new ActionDelta { energy = -3, happiness = -2, fullness = -2, skill = +4 };
}