using UnityEngine;

public class WalnutCollisionSound : MonoBehaviour
{
    public AudioSource audioSource;  
  

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walnut"))
        {
            audioSource.Play();
        }
    }
}
