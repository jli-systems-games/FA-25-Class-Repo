
using UnityEngine;

public class NPCController : MonoBehaviour
{
   
    public NPC npc;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        Attack();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            stop();
    }
    void Update()
    {
    
    }



    

    public virtual void Attack()  
    {
        Debug.Log("This character attack you with a random stuff");
        
    }
    public virtual void StopAttack()
    {
        Debug.Log("This character stoped attacking");

    }
    public virtual void stop()
    {
        Debug.Log("This main character is escaping");

    }
}
