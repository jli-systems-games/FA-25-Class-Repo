using UnityEngine;

public class NPCController : MonoBehaviour
{

    public NPC npc;

    void Start()
    {

        Attack();
        GhostAppear();
    }


    public virtual void Attack()  
    {
        Debug.Log("This character doesn't attack.");  
    }

    public virtual void GhostAppear()
    {
        Debug.Log("The ghost suddenly appears in front of you!");
    }
}
