using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundFitter2D : MonoBehaviour
{
    public Camera cam;
    public float z = 10f;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!cam) cam = Camera.main;
    }

    void LateUpdate()
    {
        if (!cam || !sr || !sr.sprite) return;
        float h = 2f * cam.orthographicSize;
        float w = h * cam.aspect;
        Vector2 spSize = sr.sprite.bounds.size;
        if (spSize.x <= 0.0001f || spSize.y <= 0.0001f) return;

        float sx = w / spSize.x;
        float sy = h / spSize.y;
        float s = Mathf.Max(sx, sy);
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + z);
        transform.localScale = new Vector3(s, s, 1f);
        sr.sortingOrder = -100;
    }
}
