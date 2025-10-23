using UnityEngine;

public class Draggable3D : MonoBehaviour
{
    private bool isDragging = false;
    private float zDistance;
    private Vector3 offset;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position; 
    }

    private void OnMouseDown()
    {
        isDragging = true;
        zDistance = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance);
        offset = transform.position - Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance);
            Vector3 newPos = Camera.main.ScreenToWorldPoint(mousePos) + offset;
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        transform.position = startPos;

    }
}
