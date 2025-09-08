using UnityEngine;

public class Enemy : NPCController
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            Attack();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            StopAttack();
    }
    public override void Attack()
    {
        Axe();
    }
    public override void StopAttack()
    {
        Laugh();
    }
    public void Axe()
    {
        Debug.Log("Swing the axe at the main character");
    }
    public void Laugh()
    {
        Debug.Log("The main character is escaping, haha");
    }
}
