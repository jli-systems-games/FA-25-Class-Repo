using UnityEngine;

[CreateAssetMenu(menuName = "PetSim/PetStats")]
public class PetStatsSO : ScriptableObject
{
    public float startMoney = 100f;
    public float startWater = 100f;
    public float startEnergy = 100f;
    public float drainSeconds = 15f;
    public float danceEnergyPerSec = 15f;
    public float deathReloadDelay = 2f;
}
