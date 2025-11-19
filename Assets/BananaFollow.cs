using UnityEngine;

public class BananaFollow : MonoBehaviour
{
    public Camera cam;

    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        transform.position = mousePos;
    }
}
