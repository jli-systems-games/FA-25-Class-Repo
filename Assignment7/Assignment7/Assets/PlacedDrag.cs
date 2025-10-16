using UnityEngine;
using System;

public class PlacedDrag : MonoBehaviour
{
    public Camera cam;
    public BoxCollider2D bounds;
    public Action onDelete;

    bool dragging;
    Vector3 grabOffset;

    void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            dragging = true;
            var wp = WorldPoint(Input.mousePosition);
            grabOffset = transform.position - wp;
        }
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) onDelete?.Invoke();
        if (!dragging) return;
        var pos = WorldPoint(Input.mousePosition) + grabOffset;
        if (bounds) pos = ClampToBounds(pos);
        pos.z = 0f;
        transform.position = pos;
    }

    Vector3 WorldPoint(Vector3 screen)
    {
        var c = cam ? cam : Camera.main;
        float z = Mathf.Abs(c.transform.position.z - 0f);
        return c.ScreenToWorldPoint(new Vector3(screen.x, screen.y, z));
    }

    Vector3 ClampToBounds(Vector3 worldPos)
    {
        var b = bounds.bounds;
        worldPos.x = Mathf.Clamp(worldPos.x, b.min.x, b.max.x);
        worldPos.y = Mathf.Clamp(worldPos.y, b.min.y, b.max.y);
        return worldPos;
    }
}
