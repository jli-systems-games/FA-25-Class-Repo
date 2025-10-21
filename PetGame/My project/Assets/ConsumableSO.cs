using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Pet/ConsumableSO")]
public class ConsumableSO : ScriptableObject
{
    public string displayName = "Beer";
    public int hpDelta = 0;
    public int moodDelta = 25;
    public int energyDelta = 0;
    public bool isAlcohol = true;
    public float drunkSeconds = 60f;
}
