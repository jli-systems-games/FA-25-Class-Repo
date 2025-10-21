using UnityEngine;

public class GrandpaMouthTrigger : MonoBehaviour
{
    public GrandpaController grandpa;
    public Animator grandpaAnimator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Food")) return;
        if (grandpa == null) return;

        // ActiveNeeds 대신 컨트롤러의 HasNeed/ResolveNeedExternally 사용
        if (grandpa.HasNeed(GrandpaState.Hungry))
        {
            if (grandpaAnimator != null)
                grandpaAnimator.SetTrigger("Eat");

            grandpa.ResolveNeedExternally(GrandpaState.Hungry);
            other.gameObject.SetActive(false);
        }
    }
}
