using UnityEngine;

[RequireComponent(typeof(Transform))]
public class CatControl : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float padding = 0.3f;
    public bool flipSpriteOnDir = true;

    Camera cam;
    SpriteRenderer sr;
    Transform tr;

    void Awake()
    {
        tr = transform;
        cam = Camera.main;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        float move = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);

        Vector3 pos = tr.position;
        pos.x += move * moveSpeed * Time.deltaTime;

        if (cam)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            float left  = cam.transform.position.x - halfW + padding;
            float right = cam.transform.position.x + halfW - padding;
            float halfPlayer = sr ? sr.bounds.extents.x : 0f;
            pos.x = Mathf.Clamp(pos.x, left + halfPlayer, right - halfPlayer);
        }

        tr.position = pos;
        if (flipSpriteOnDir && sr && move != 0) sr.flipX = move < 0;
    }
}