using UnityEngine;

// Interface for interactable objects
public interface IInteractable
{
    void Interact(GameObject player);
    void OnHoverEnter();
    void OnHoverExit();
}

// Example interactable object implementation
public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] private string interactionPrompt = "Press E to interact";
    [SerializeField] private Color hoverColor = Color.yellow;
    
    private Renderer objectRenderer;
    private Color originalColor;
    private bool isHovered;
    
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }
    
    public void Interact(GameObject player)
    {
        Debug.Log($"Interacted with {gameObject.name}!");
        
        // Add your interaction logic here
        // Examples:
        // - Pick up item
        // - Open door
        // - Start dialogue
        // - Activate mechanism
        
        // Example: Change color on interaction
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Random.ColorHSV();
        }
    }
    
    public void OnHoverEnter()
    {
        isHovered = true;
        
        // Visual feedback
        if (objectRenderer != null)
        {
            objectRenderer.material.color = hoverColor;
        }
        
        // You could show UI prompt here
        Debug.Log($"Looking at: {gameObject.name} - {interactionPrompt}");
    }
    
    public void OnHoverExit()
    {
        isHovered = false;
        
        // Reset visual feedback
        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw a small sphere to indicate this is interactable
        Gizmos.color = isHovered ? Color.green : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}

// Example: Collectable item
public class CollectableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName = "Item";
    [SerializeField] private float rotationSpeed = 50f;
    
    private bool isCollected = false;
    
    void Update()
    {
        if (!isCollected)
        {
            // Idle floating animation
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            transform.position += Vector3.up * Mathf.Sin(Time.time * 2f) * 0.001f;
        }
    }
    
    public void Interact(GameObject player)
    {
        if (isCollected) return;
        
        isCollected = true;
        Debug.Log($"Collected: {itemName}!");
        
        // Add to player inventory here
        // PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        // inventory.AddItem(itemName);
        
        // Play collection effect
        Destroy(gameObject, 0.1f);
    }
    
    public void OnHoverEnter()
    {
        Debug.Log($"Collectable: {itemName}");
    }
    
    public void OnHoverExit()
    {
        // Optional: Remove UI prompt
    }
}
