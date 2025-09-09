using UnityEngine;

public class NPC_John : NPCController
{
    

    public override void Attack() 
    {
        SwingBat();
    }



    public void SwingBat() //John is special and has a bat. No other NPC has this behavior
    {
        Debug.Log("John swings his cool bat.");
    }
}