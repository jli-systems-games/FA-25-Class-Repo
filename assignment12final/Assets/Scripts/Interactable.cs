using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("interactionId")]
    public string interactionId;

    [Header("oneTime")]
    public bool oneTime = true;

    [Header("interactKey")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;
    private bool hasUsed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (InteractHintUI.Instance != null && !string.IsNullOrEmpty(interactionId))
            {
                InteractHintUI.Instance.ShowHint(interactionId, this.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (InteractHintUI.Instance != null && !string.IsNullOrEmpty(interactionId))
            {
                InteractHintUI.Instance.HideHint(interactionId);
            }
        }
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (oneTime && hasUsed)
            {
                return;
            }

            if (InteractionManager.Instance != null)
            {
                InteractionManager.Instance.HandleInteraction(interactionId);

                if (oneTime)
                {
                    hasUsed = true;
                }
            }
        }
    }
}
