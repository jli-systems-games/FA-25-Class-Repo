using UnityEngine;

[CreateAssetMenu(fileName = "PetData", menuName = "Pet/PetData")]
public class PetData : ScriptableObject
{
    [Range(0, 100)] public float hunger = 100f;
    [Range(0, 100)] public float cleanliness = 100f;
    [Range(0, 100)] public float happiness = 100f;
}
