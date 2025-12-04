using UnityEngine;

public class DisableZoneMovement : MonoBehaviour
{
    public float speed;

    private float rotationY = 0f;

    void Update()
    {
        rotationY += 1f * speed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }
}
