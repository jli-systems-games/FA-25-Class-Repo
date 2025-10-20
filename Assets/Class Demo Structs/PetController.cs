using UnityEngine;

public class PetController : MonoBehaviour
{
    //public PetMood petMood = PetMood.Happy;
    //public int petHealth = 100;

    public Pet pet;
    public Dog dog;

    public void GetSad()
    {
        pet.petMood = PetMood.Sad;
        CheckState();
    }

    public void CheckState()
    {
        switch (pet.petMood)
        {
            case PetMood.Happy:
                if (pet.petHealth > 50)
                {

                }
                else
                {
                    pet.petMood = PetMood.Stressed;
                }
                    break;
            case PetMood.Sad:
                break;
            case PetMood.Stressed:
                break;
            case PetMood.Dying:
                break;
            default:
                break;
        }
    }
}
