using UnityEngine;

public class DemoData
{
    public static Pet pet;
}

#region Pet Customization
public enum PetFurType
{
    Long,
    Curly,
    Balding
}

public enum PetMood
{
    Happy,
    Sad,
    Stressed,
    Dying
}
#endregion

public struct Pet //Struct is a class without functions
{
    public string petName;
    public PetMood petMood;
    public int petHealth;

    public int petHunger;
    public int petEnergy;
    public int petHygiene;
}

public struct Food
{
    public string foodName;
    public int foodHealth;

    public int itemHunger;
}
