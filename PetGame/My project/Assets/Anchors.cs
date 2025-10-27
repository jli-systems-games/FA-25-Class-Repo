using UnityEngine;

public class Anchors : MonoBehaviour
{
    public Transform respawnPoint;
    public Transform dancePoint;
    public Transform attackPoint;
    public BoxCollider moveBounds;
    public LayerMask floorMask;

    public Vector3 ClampToBounds(Vector3 p)
    {
        if (!moveBounds) return p;
        var b = moveBounds.bounds;
        p.x = Mathf.Clamp(p.x, b.min.x, b.max.x);
        p.y = Mathf.Clamp(p.y, b.min.y, b.max.y);
        p.z = Mathf.Clamp(p.z, b.min.z, b.max.z);
        return p;
    }
}
