using System.Linq;
using UnityEngine;

public class GrandpaMouthTrigger : MonoBehaviour
{
    public GrandpaController grandpa;
    public Animator grandpaAnimator; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            if (grandpa.ActiveNeeds.Contains(GrandpaState.Hungry))
            {
               
                grandpaAnimator.SetTrigger("Eat");
                grandpa.ResolveNeedExternally(GrandpaState.Hungry);
                other.gameObject.SetActive(false);
            }
        }
    }
}
