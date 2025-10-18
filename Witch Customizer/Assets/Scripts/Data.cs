using UnityEngine;
using System.Collections.Generic;

public class Data
{
    public static Data Instance = new Data();

    // Player info
    public string data_playerName;
    public string data_playerMageType;

    
    public int data_robeIndex;
    public int data_hatIndex;
    public int data_wandIndex;
    public int data_accessoryIndex;

    
    public static List<Sprite> data_robeSprites = new List<Sprite>();
    public static List<Sprite> data_hatSprites = new List<Sprite>();
    public static List<Sprite> data_wandSprites = new List<Sprite>();
    public static List<Sprite> data_accessorySprites = new List<Sprite>();

    
    public void ResetData()
    {
        data_playerName = "";
        data_playerMageType = "";
        data_robeIndex = 0;
        data_hatIndex = 0;
        data_wandIndex = 0;
        data_accessoryIndex = 0;
        
    }
}
