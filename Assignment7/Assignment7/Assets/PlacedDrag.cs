using System;
using UnityEngine;

public class PlacedDrag : MonoBehaviour
{
    public Camera cam;
    public BoxCollider2D bounds;
    public Action onDelete;
    bool dragging;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition); p.z = 0;
            if ((p - transform.position).sqrMagnitude < 1.2f) dragging = true;
        }
        if (dragging)
        {
            Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition); p.z = 0;
            if (bounds) p = ClampToBounds(p);
            transform.position = p;
            if (Input.GetMouseButtonUp(0)) dragging = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition); p.z = 0;
            if ((p - transform.position).sqrMagnitude < 1.2f)
            {
                if (onDelete != null) onDelete();
                else Destroy(gameObject);
            }
        }
    }

    Vector3 ClampToBounds(Vector3 worldPos)
    {
        var b = bounds.bounds;
        worldPos.x = Mathf.Clamp(worldPos.x, b.min.x, b.max.x);
        worldPos.y = Mathf.Clamp(worldPos.y, b.min.y, b.max.y);
        worldPos.z = 0f;
        return worldPos;
    }
}
