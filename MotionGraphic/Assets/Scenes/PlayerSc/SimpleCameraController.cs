using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("")]
public class SimpleCameraController : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxLookAngle = 80f;

    private float rotationX = 0f;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = transform.parent;
    }

    void Update()
    {
        if (playerTransform == null) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 delta = mouse.delta.ReadValue();
        float mouseX = delta.x * mouseSensitivity;
        float mouseY = delta.y * mouseSensitivity;

        playerTransform.Rotate(Vector3.up * mouseX);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}
