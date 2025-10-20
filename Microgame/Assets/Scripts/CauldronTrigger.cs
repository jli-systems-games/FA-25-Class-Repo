using UnityEngine;

public class CauldronTrigger : MonoBehaviour
{
    [Header("Reference to the EyeballDrop script")]
    public EyeBallDrop gameScript;   

    void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Eyeball"))
        {
            Debug.Log("Eyeball entered cauldron!");
            gameScript.EndGame(true); 
        }
    }
}

