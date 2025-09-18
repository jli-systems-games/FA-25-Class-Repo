using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPan2D : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 10f;          // WASD / 箭头键
    public bool enableDragPan = true;      // 鼠标拖拽平移
    public float dragSpeed = 1f;           // 拖拽速度（越大越快）

    [Header("Zoom (Orthographic)")]
    public bool enableZoom = true;
    public float zoomSpeed = 5f;           // 滚轮缩放速度
    public float minOrthoSize = 2f;
    public float maxOrthoSize = 20f;

    [Header("Bounds (optional)")]
    public bool clampToBounds = false;
    public Rect worldBounds = new Rect(-50, -50, 100, 100); // x,y 为左下角

    Camera cam;
    Vector3 dragOrigin;     // 世界坐标
    bool dragging;

    void Awake() { cam = GetComponent<Camera>(); }

    void Update()
    {
        // --- 键盘平移 ---
        Vector3 input =
            new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
        transform.position += input.normalized * moveSpeed * Time.deltaTime;

        // --- 鼠标拖拽平移（中键或右键）---
        if (enableDragPan && (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1)))
        {
            dragging = true;
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            dragOrigin.z = 0;
        }
        if (enableDragPan && (Input.GetMouseButtonUp(2) || Input.GetMouseButtonUp(1)))
            dragging = false;

        if (enableDragPan && dragging)
        {
            Vector3 curPos = cam.ScreenToWorldPoint(Input.mousePosition);
            curPos.z = 0;
            Vector3 delta = (dragOrigin - curPos) * dragSpeed;
            transform.position += delta;
        }

        // --- 正交相机缩放 ---
        if (enableZoom && cam.orthographic)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.0001f)
            {
                cam.orthographicSize = Mathf.Clamp(
                    cam.orthographicSize - scroll * zoomSpeed,
                    minOrthoSize, maxOrthoSize);
            }
        }

        // --- 边界约束 ---
        if (clampToBounds)
        {
            Vector3 p = transform.position;
            p.x = Mathf.Clamp(p.x, worldBounds.xMin, worldBounds.xMax);
            p.y = Mathf.Clamp(p.y, worldBounds.yMin, worldBounds.yMax);
            transform.position = p;
        }
    }
}