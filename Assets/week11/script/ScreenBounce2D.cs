using UnityEngine;

/// 콜라이더 없이 카메라 경계에서 반사(bounce) 효과
[RequireComponent(typeof(Rigidbody2D))]
public class ScreenBounce2D : MonoBehaviour
{
    public Camera cam;
    public float padding = 0.1f;
    [Range(0f, 1.2f)] public float bounciness = 0.9f; // 1=완전탄성, 0.9 추천

    Rigidbody2D rb;
    Vector2 ext;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!cam) cam = Camera.main;

        var col = GetComponent<Collider2D>();
        if (col) ext = col.bounds.extents;
        else
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            ext = sr ? (Vector2)sr.bounds.extents : new Vector2(0.5f, 0.5f);
        }
    }

    void FixedUpdate()
    {
        if (!cam) return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 cpos = cam.transform.position;

        float minX = cpos.x - halfW + ext.x + padding;
        float maxX = cpos.x + halfW - ext.x - padding;
        float minY = cpos.y - halfH + ext.y + padding;
        float maxY = cpos.y + halfH - ext.y - padding;

        Vector2 p = rb.position;
        Vector2 v = rb.linearVelocity;

        // X 경계 체크
        if (p.x < minX) { p.x = minX; v.x = Mathf.Abs(v.x) * bounciness; }
        else if (p.x > maxX) { p.x = maxX; v.x = -Mathf.Abs(v.x) * bounciness; }

        // Y 경계 체크
        if (p.y < minY) { p.y = minY; v.y = Mathf.Abs(v.y) * bounciness; }
        else if (p.y > maxY) { p.y = maxY; v.y = -Mathf.Abs(v.y) * bounciness; }

        rb.MovePosition(p);
        rb.linearVelocity = v;
    }
}
