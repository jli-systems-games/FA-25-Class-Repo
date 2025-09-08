using UnityEngine;

public class NPCController : MonoBehaviour
{
    public NPC npc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Debug.Log("Hello.");
        Attack();
    }

    public virtual void Attack()
    {
        Debug.Log("This character doesn't attack.");
    }
}
