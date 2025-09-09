using UnityEngine;

public class NPC_Raina : NPCController

{
    public override void GhostAppear()
    {
        Scream();
        if (npc.npcLevel > 5)  
        {
            Attack();
        }
        else
        {
            Debug.Log("Raina glares but does not attack yet...");
        }
    }

    public override void Attack()
    {
        PunchFace();
    }

    public void PunchFace()
    {
        Debug.Log("Raina punches the ghost's face");
    }

    public void Scream()
    {
        Debug.Log("Raina lets out a terrifying scream!");
    }
}
