using UnityEngine;

public class FishControllerBehindCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f; // How fast the fish rotates

    [Header("Rotation Limits")]
    public float maxPitch = 30f; // Max up/down tilt
    public float maxYaw = 45f;   // Max left/right turn

    private float pitch = 0f; // rotation around Z axis
    private float yaw = 0f;   // rotation around Y axis

    void Update()
    {
        // --- Input ---
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
        float vertical = Input.GetAxisRaw("Vertical");     // W/S

        // --- Move ---
        Vector3 move = new Vector3(horizontal, vertical, 0f).normalized;
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        // --- Pitch (Z rotation) ---
        pitch = Mathf.Lerp(pitch, -vertical * maxPitch, rotationSpeed * Time.deltaTime);

        // --- Yaw (Y rotation) ---
        yaw = Mathf.Lerp(yaw, horizontal * maxYaw, rotationSpeed * Time.deltaTime);

        // --- Apply rotation ---
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
