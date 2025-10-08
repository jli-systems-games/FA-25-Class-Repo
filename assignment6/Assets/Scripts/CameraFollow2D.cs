using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("目标")]
    public Transform train;

    [Header("偏移与看前方")]
    public Vector2 baseOffset = new Vector2(0f, 2.5f);
    public Vector2 forwardAxis = Vector2.up; // 向上推进；横版改为 Vector2.right
    public float lookAheadDistPerSpeed = 0.25f;
    public float lookAheadMax = 4f;
    public float followSmooth = 0.15f;

    [Header("正交视野自适应")]
    public float orthoSizeMin = 6f;
    public float orthoSizeMax = 10f;
    public float sizePerSpeed = 0.08f;
    public float sizeSmooth = 0.2f;

    private Vector3 _vel;
    private Vector3 _lastPos;

    void Start()
    {
        if (!train) enabled = false;
        _lastPos = train.position;
    }

    void LateUpdate()
    {
        if (!train) return;

        Vector3 cur = train.position;
        float speed = (cur - _lastPos).magnitude / Mathf.Max(Time.deltaTime, 1e-4f);
        _lastPos = cur;

        Vector2 look = forwardAxis.normalized * Mathf.Min(lookAheadMax, lookAheadDistPerSpeed * speed);
        Vector3 targetPos = (Vector2)train.position + baseOffset + look;
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _vel, followSmooth);

        var cam = GetComponent<Camera>();
        if (cam && cam.orthographic)
        {
            float t = Mathf.Clamp(orthoSizeMin + sizePerSpeed * speed, orthoSizeMin, orthoSizeMax);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, t, 1f - Mathf.Exp(-sizeSmooth * Time.deltaTime));
        }
    }
}
