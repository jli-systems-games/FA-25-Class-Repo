using UnityEngine;

public class BrushFollowMouse : MonoBehaviour
{
    public float distanceFromCamera = 4.54f;

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.z = distanceFromCamera;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        transform.position = worldPosition;
    }
}
