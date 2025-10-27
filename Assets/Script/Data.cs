using UnityEngine;

public class Data
{
    public static Pet petData;

    public enum PetFurType
    {
        Short,
        Long,
        Curly,
        Spiky
    }

    public enum PetMood
    {
        Happy,
        Sad,
        Playful,
        Sleepy
    }

    public struct Pet
    {
        public string PetName;
        public PetMood petMood;
        public int petHealth;
        public int petHunger;
        public int petEnergy;
        public int PetSocial;
    }

    public struct Food
    {
        public string FoodName;
        public int foodhealth;  
    }
}
