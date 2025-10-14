using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Data
{
    //public static int data_playerLevel=1;
    public Character data_playerCharacter;
    public static List<Sprite> data_skinFreckles;

    
    public static List<Sprite> data_skinHat = new List<Sprite>();   
    public static List<Sprite> data_skinDress = new List<Sprite>();   


    public struct Character
    {
        public string charaterName;
        public int characterLevel;
        public int characterStr;
        public int characterDex;
        public int characterId;
    }
    public struct Skin
    {
        public Color skinColor;
        public int skinFreckles;
    }
    public enum Freckles
    {
        Everywhere,Some,None
    }
}