using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<ZombieMovement>())
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);  
        }

    }
}
