using UnityEngine;

public class PetController : MonoBehaviour
{
    //public PetMood petMood = PetMood.Happy;
    //public int petHealth = 100;


    public Pet pet;
    public Dog dog;

    private void Start()
    {
        pet.petMood = PetMood.Happy;
    }

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
                //do somewthing
                if(pet.petHealth > 50)
                {

                }
                else
                {
                    pet.petMood = PetMood.Stressed;
                }
                    break;
            case PetMood.Sad:
                //do somewthing
                break;
            case PetMood.Stressed:
                //do somewthing
                break;
            case PetMood.Dying:
                //do somewthing
                break;
            default:
                break;
        }
    }
}

public enum PetFurType
{
    Long,
    Curly,
    Balding
}

