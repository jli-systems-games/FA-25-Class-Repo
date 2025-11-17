using UnityEngine;

public class NPC
{

        public int npcID = 0;
        public string npcName;
   
        public int npcStrength = 0;
        public int npcDexterity = 0;
        public int npcConstitution = 0;
        public int npcIntelligence = 0;
        public int npcWisdom = 0;
        public int npcCharisma = 0;

    public void UpdateStats()
    {

        Debug.Log(npcName);

    }

}
