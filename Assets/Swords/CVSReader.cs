using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class CSVReader : MonoBehaviour
{
    public TextAsset npcData;
    public TextAsset swordData;
    private int numberColumns = 5; //change this to match your spreadsheet

    public void Awake()
    {
        // ImportNPCs();
        //ImportWeapons();

        ImportSwords();
    }


    public void ImportSwords()
    {

        string[] data = swordData.text.Split(new string[] { ",", "\n" },
         StringSplitOptions.None);


        int tableSize = (data.Length / numberColumns) - 1;


        for (int i = 0; i < tableSize; i++)
        {

            Sword tempWepon = new Sword();


            tempWepon.weponID = int.Parse(data[numberColumns * (i + 1)]);
            tempWepon.swordName = data[numberColumns * (i + 1) + 1];
            tempWepon.quickness = int.Parse(data[numberColumns * (i + 1) + 2]);
            tempWepon.weight = int.Parse(data[numberColumns * (i + 1) + 3]);
            tempWepon.stabAbilty = int.Parse(data[numberColumns * (i + 1) + 4]);

            Data.swordDatabase.Add(tempWepon.weponID, tempWepon);



        }

        

    }



    public void ImportNPCs()
    {
        string[] data = npcData.text.Split(new string[] { ",", "\n" },
            StringSplitOptions.None);

        int tableSize = (data.Length / numberColumns) - 1;

        for (int i = 0; i < tableSize; i++)
        {

            NPC tempNPC = new NPC(); //be sure to define a NPC struct either here or in Data!

            tempNPC.npcID = int.Parse(data[numberColumns * (i + 1)]);

            tempNPC.npcName = data[numberColumns * (i + 1) + 1];
          
            tempNPC.npcStrength = int.Parse
               (data[numberColumns * (i + 1) + 2]);
            tempNPC.npcDexterity = int.Parse
                (data[numberColumns * (i + 1) + 3]);
            tempNPC.npcConstitution = int.Parse
                (data[numberColumns * (i + 1) + 4]);
            tempNPC.npcIntelligence = int.Parse
                (data[numberColumns * (i + 1) + 5]);
            tempNPC.npcWisdom = int.Parse
                (data[numberColumns * (i + 1) + 6]);
            tempNPC.npcCharisma = int.Parse
                (data[numberColumns * (i + 1) + 7]);
          
            tempNPC.UpdateStats();
          
            Data.npcDatabase.Add(tempNPC.npcID, tempNPC); //there should be a static dictionary in a Data script

            Debug.Log(Data.npcDatabase[0]);

        }
    }

    //The code from the last time I ran this exercise:
    /*
    
    public void ImportWeapons()  
    {
        string[] data = npcData.text.Split(new string[] { ",", "\n" },
            StringSplitOptions.None);

        int tableSize = (data.Length / numberColumns) - 1;

        for (int i = 0; i < tableSize; i++)
        {
            Weapon tempWeapon = new Weapon();

            tempWeapon.weaponName = data[numberColumns * (i + 1)];

            string weaponTypeString = data[numberColumns * (i + 1) + 1];
            if(weaponTypeString == "Physical")
            {
                tempWeapon.weaponType = WeaponType.Physical;
            }
            else
            {
                tempWeapon.weaponType = WeaponType.Magic;
            }

            tempWeapon.weaponPower = int.Parse
                (data[numberColumns * (i + 1) + 2]);

            tempWeapon.reqStr = int.Parse
                (data[numberColumns * (i + 1) + 3]);
            tempWeapon.reqDex = int.Parse
                (data[numberColumns * (i + 1) + 4]);
            tempWeapon.reqCon = int.Parse
                (data[numberColumns * (i + 1) + 5]);
            tempWeapon.reqInt = int.Parse
                (data[numberColumns * (i + 1) + 6]);
            tempWeapon.reqWis = int.Parse
                (data[numberColumns * (i + 1) + 7]);
            tempWeapon.reqCha = int.Parse
                (data[numberColumns * (i + 1) + 8]);

            Data.weaponDatabase.Add(i, tempWeapon);
        }
    }*/
}
