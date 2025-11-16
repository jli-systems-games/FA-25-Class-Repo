using UnityEngine;

public class PaddleManual : MonoBehaviour
{
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public float speed = 9f;
    public float padding = 0.5f;

    float halfHeight;
    float camHalfHeight;

    void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        halfHeight = sr.bounds.size.y * 0.5f;
        camHalfHeight = Camera.main.orthographicSize;
    }

    void Update()
    {
        float dir = 0f;
        if (Input.GetKey(upKey)) dir += 1f;
        if (Input.GetKey(downKey)) dir -= 1f;

        Vector3 pos = transform.position;
        pos += Vector3.up * dir * speed * Time.deltaTime;

        float yMin = -camHalfHeight + halfHeight + padding;
        float yMax = camHalfHeight - halfHeight - padding;
        pos.y = Mathf.Clamp(pos.y, yMin, yMax);

        transform.position = pos; 
    }
}
