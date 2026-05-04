using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableMask;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    // Components
    private CharacterController controller;

    // Movement
    private Vector3 velocity;
    private Vector3 currentMovement;
    private bool isSprinting;
    private bool isGrounded;

    // Interaction
    private IInteractable currentInteractable;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleInteraction();
    }

    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)  horizontal -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontal += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)  vertical   -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)    vertical   += 1f;

        isSprinting = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 forward = cameraTransform.forward;
        Vector3 right   = cameraTransform.right;
        forward.y = 0f;
        right.y   = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMovement = (forward * vertical + right * horizontal).normalized * currentSpeed;

        float accelerationRate = desiredMovement.magnitude > 0 ? acceleration : deceleration;
        currentMovement = Vector3.Lerp(currentMovement, desiredMovement, accelerationRate * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMovement = currentMovement + new Vector3(0, velocity.y, 0);
        controller.Move(finalMovement * Time.deltaTime);
    }

    void HandleJump()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    void HandleInteraction()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        IInteractable newInteractable = null;

        if (Physics.Raycast(ray, out hit, interactionRange, interactableMask))
        {
            newInteractable = hit.collider.GetComponent<IInteractable>();
        }

        if (newInteractable != currentInteractable)
        {
            if (currentInteractable != null) currentInteractable.OnHoverExit();
            if (newInteractable != null)     newInteractable.OnHoverEnter();
            currentInteractable = newInteractable;
        }

        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame && currentInteractable != null)
        {
            currentInteractable.Interact(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (cameraTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * interactionRange);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * (groundCheckDistance + 0.1f));
    }
}
