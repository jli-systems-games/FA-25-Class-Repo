using UnityEngine;

public class DemoPetCon : MonoBehaviour
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
        switch(pet.petMood)
        {
            case PetMood.Happy:
                if(pet.petHealth>50)
                {

                }
                else
                {
                    pet.petMood = PetMood.Stressed;
                }
                break;
            case PetMood.Stressed:
                //
                break;
            case PetMood.Sad:
                //
                break;
            case PetMood.Dying:
                //
                break;

        }
    }
}

public enum PetFurType
{
    Long,
    curly,
    Balding
}

public enum PetMood
{
    Happy,
    Sad,
    Stressed,
    Dying
}

public struct Pet
{
    public string petName;
    public PetMood petMood;
    public int petHealth;

    public int petHunger;
    public int petEnergy;
    public int PetSocial;
}
public struct Food
{
    public string foodName;
    public int foodeHealth;

    public int itemHunger;
}