using UnityEngine;

public class PottyMover : MonoBehaviour
{
    public float jumpHeight = 0.5f;
    public float moveSpeed = 1f;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (PowerupManager.Instance == null ||
            PowerupManager.Instance.currentMain != MainPowerup.FroggyHat)
        {
            return;
        }

        if (cam == null) cam = Camera.main;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;

        Vector3 current = transform.position;
        Vector3 target = Vector3.MoveTowards(current, mouseWorld, moveSpeed * Time.deltaTime);

        float yOffset = Mathf.Sin(Time.time * 3f) * jumpHeight;
        target.y += yOffset;

        transform.position = target;
    }
}
