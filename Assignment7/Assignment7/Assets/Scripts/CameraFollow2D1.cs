using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smooth = 6f;
    public float marginY = 0.8f;
    public float extraRight = 4f;
    public float desiredOrthoHalfHeight = 4f;
    public float trackHalfWidth = 3.5f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (!cam) cam = Camera.main;
        if (cam) { cam.orthographic = true; ApplyOrthoSize(); }
    }

    public void SetTrackHalfWidth(float half)
    {
        trackHalfWidth = Mathf.Max(0.5f, half);
        ApplyOrthoSize();
    }

    void ApplyOrthoSize()
    {
        if (!cam) return;
        float half = Mathf.Max(trackHalfWidth + marginY, desiredOrthoHalfHeight);
        cam.orthographicSize = half;
        float aspect = 16f / 9f;
        float h = 2f * cam.orthographicSize;
        float w = h * aspect;
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -trackHalfWidth + marginY, trackHalfWidth - marginY), -10f);
    }

    void LateUpdate()
    {
        if (!cam || !target) return;

        float yMin = -trackHalfWidth + marginY;
        float yMax = trackHalfWidth - marginY;
        float targetY = Mathf.Clamp(target.position.y, yMin, yMax);
        Vector3 want = new Vector3(target.position.x + extraRight, targetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, want, 1f - Mathf.Exp(-smooth * Time.deltaTime));
    }
}
