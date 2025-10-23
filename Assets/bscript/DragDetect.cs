using UnityEngine;

public class PetInteraction : MonoBehaviour
{
    public PetManager petManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            petManager.FeedPet();
           
        }
        else if (collision.CompareTag("Brush"))
        {
            petManager.CleanPet();
        }
        Debug.Log("touched");
    }

    private void OnMouseDown()
    {
        
        petManager.PetThePet();
    }
}
