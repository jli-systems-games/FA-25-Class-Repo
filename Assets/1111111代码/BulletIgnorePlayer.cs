using UnityEngine;

public class BulletIgnorePlayer : MonoBehaviour
{
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player1");
        
        Collider bulletCollider = GetComponent<Collider>();
        
        if (bulletCollider == null)
        {
            Debug.LogWarning("子弹没有Collider组件");
            return;
        }
        
        foreach (GameObject player in players)
        {
            Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
            
            foreach (Collider playerCollider in playerColliders)
            {
                Physics.IgnoreCollision(bulletCollider, playerCollider);
            }
        }
    }
}
