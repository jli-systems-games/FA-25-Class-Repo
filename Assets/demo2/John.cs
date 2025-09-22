using UnityEngine;

public class NPC_John : NPCController //here, you replace MonoBehaviour with NPCController
{

    public override void Attack() 
    {
        SwingBat();
    }

    public void SwingBat() 
    {
        Debug.Log("John swings his cool bat.");
    }
}