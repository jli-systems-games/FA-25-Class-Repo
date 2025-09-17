using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Transform cameraMainTransform;

    [Header("Movement Settings")]
    public float playerSpeed = 5f;
    public float jumpForce = 5f;
    public float groundCheckDis = 0.4f;
    public LayerMask groundMask;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("Input Actions")]
    public InputActionReference movementControl;
    public InputActionReference jumpControl;
    public InputActionReference lookControl;

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        lookControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        lookControl.action.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cameraMainTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();

        if (jumpControl.action.triggered && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;

        rb.MovePosition(rb.position + move * playerSpeed * Time.fixedDeltaTime);
    }

    private void HandleLook()
    {
        Vector2 look = lookControl.action.ReadValue<Vector2>();

        float mouseX = look.x * mouseSensitivity * Time.deltaTime;
        float mouseY = look.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraMainTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDis, groundMask);
    }
}
