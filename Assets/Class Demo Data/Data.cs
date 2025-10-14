using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

public class Data
{
    //public static int data_playerLevel = 1;

    public static Character data_playerCharacter;

    //Visual Presets
    public static List<Sprite> data_skinFreckles;
    public static List<Sprite> data_skinHat;
    public static List<Sprite> data_skinDress;
    public static List<Sprite> data_skinShoe;
}

public struct Character
{
    public string characterName;
    public int characterId;

    public int characterLevel;

    public int characterStrength;
}

public struct Skin
{
    public Color skinColor;
    //public int skinFreckleIndex;

    public int skinHatIndex, skinDressIndex, skinShoeIndex;

    public Sprite skinDress;
    public Freckles skinFreckles;
}

public enum Freckles
{
    Everywhere,
    Some,
    None
}
