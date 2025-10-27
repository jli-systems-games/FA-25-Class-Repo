using UnityEngine;

public class PetController : MonoBehaviour
{
    public PetMood PetMood = PetMood.Happy;
    public int petHealth = 10;

    public Pet pet;

    public void CheckState()
    {
        switch (pet.petMood)
        {
            case PetMood.Happy:
                if (pet.petEnergy > 5)
                {
                    
                }else
                {
                    pet.petMood = PetMood.Sad;
                }
                break;
            case PetMood.Sad:
                break;
            case PetMood.Hugnry:
                break;
            case PetMood.Full:
                break;
        }
    }
}


public enum PetMood
{
    Happy,
    Sad,
    Hugnry,
    Full
}

public struct Pet
{
    public string PetName;
    public PetMood petMood;
    public int petEnergy;
    public int petHunger;
}

public struct Food
{
    public int foodHealth;  
}