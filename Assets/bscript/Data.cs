using UnityEngine;
using System.Collections.Generic;
public class Data 
{
    //public static int data_playerlevel = 1;
    //public static Character data_playerCharacters;
    //public static Character data_playerSkin;
    //public static Character date_playerCharacter;
    //public static List<Sprite> data_skinFreckles;
    //public static List<Sprite> data_customizerHats;
    //public static List<Sprite> data_customizerDress;
    //public static List<Sprite> data_customizerShoes;
}

public class GameData
{
    public int bodyColorIndex;
    public int[] stringSoundIndices = new int[4];
}

public static class CentralData
{
    public static GameData current = new GameData();
}

//public struct Character
//{
//    public string characterName;
//    public int charaerId;

//    public int characterlevel;

//    public int characterStr;
//    public int characterDex;
//}

//public struct Skin
//{
//    public int skinHatIndex,
//                skinDressIndex,
//                skinShoesIndex;

//}