using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

public class Data
{
    //public static int data_playerLevel = 1;
    public static Character data_playerCharacter;
    public static Skin data_playerSkin;

    //visual presets
    //public static List<Sprite> data_skinFreckles;
    public static List<Sprite> data_skinHat;
    public static List<Sprite> data_skinDress;
    public static List<Sprite> data_skinShoes;
}

public struct Character
{
    public string characterName;
    public int characterId;

    public int characterLevel;

    public int characterStr;
    public int characterDex;
}

public struct Skin
{
    public int skinHatIndex, 
               skinDressIndex, 
               skinShoesIndex;

}

public enum Freckles
{
    Everywhere,
    Some,
    None
}