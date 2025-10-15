using UnityEngine;

public class FishControllerBehindCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    [Header("Rotation Limits")]
    public float maxVerticalRotation = 30f; 
    public float maxHorizontalRotation = 45f; 

    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;   

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); 
        float vertical = Input.GetAxisRaw("Vertical"); 

        Vector3 move = new Vector3(horizontal, vertical, 0f).normalized;
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        verticalRotation = Mathf.Lerp(verticalRotation, -vertical * maxVerticalRotation, rotationSpeed * Time.deltaTime);

        horizontalRotation = Mathf.Lerp(horizontalRotation, horizontal * maxHorizontalRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
    }
}
