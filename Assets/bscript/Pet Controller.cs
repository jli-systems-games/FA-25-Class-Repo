using UnityEngine;


public class PetController : MonoBehaviour
{
    //public PetMood petMood = PetMood.Happy;
    //public int petHealth = 100;
   // public Pet pet;
   //public void GetSad()
   // {
   //     pet.petMood = PetMood.Sad;
   //     CheckState();
   // }
   //public void CheckState()
   // {
   //     switch (pet.petMood)
   //     {
   //         case PetMood.Happy:
   //             if (pet.petHealth > 50)
   //             {

   //             }
   //             else
   //             {
   //                 pet.petMood = PetMood.Stressed;
   //             }
   //             break;
   //         case PetMood.Sad:
   //             break;
   //         case PetMood.Stressed:
   //             break;
   //         case PetMood.Dying:
   //             break;
   //         default:
   //             break;
   //     }
   // }
}

public enum PetState
{
    Happy,
    Hungry,
    Dirty,
    Sad
}
public struct PetStats
{
    public float hunger;
    public float cleanliness;
    public float happiness;

    public PetStats(float h, float c, float hp)
    {
        hunger = h;
        cleanliness = c;
        happiness = hp;
    }
}
//public enum PetFurType
//{
//    Long,
//    Curly,
//    Balding
//}

//public enum PetMood
//{
//    Happy,
//    Sad,
//    Stressed,
//    Dying
//}
//public struct Pet
//{
//    public string petName;
//    public PetMood petMood;
//    public int petHealth;



//}