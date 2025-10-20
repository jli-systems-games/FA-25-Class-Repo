using UnityEngine;

public class Data
{
    public static Pet pet;

}

#region Pet Customizations
public enum PetMood
{
    Happy,
    Sad,
    Stressed,
    Dying
}
#endregion

#region Structs
public struct Pet
{
    public string petName;
    public PetMood petMood;
    public int petHealth;

    public int petHunger;
    public int petEnergy;
    public int petSocial;
}

public struct Item
{
    public string foodName;
    public int foodHealth;

    public int itemHunger;
}

#endregion

