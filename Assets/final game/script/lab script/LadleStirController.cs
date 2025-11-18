using UnityEngine;
using UnityEngine.Events;

public class LadleStirController : MonoBehaviour
{
    [Header("국자가 움직일 수 있는 반경")]
    public float stirRadius = 0.5f;

    [Header("이벤트")]
    public UnityEvent onStirOnce;   // 드래그 후 MouseUp 한 번 = 저은 1회

    bool _isDragging = false;
    float _yHeight;
    Vector3 _centerPos;
    Quaternion _initialRot;

    void Start()
    {
        _yHeight = transform.position.y;
        _centerPos = transform.position;
        _initialRot = transform.rotation;
    }

    void OnMouseDown()
    {
        _isDragging = true;
    }

    void OnMouseUp()
    {
        if (_isDragging && onStirOnce != null)
        {
            onStirOnce.Invoke();  // 한 번 젓기 완료
        }
        _isDragging = false;
    }

    void Update()
    {
        if (!_isDragging) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0f, _yHeight, 0f));

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            Vector3 offset = hitPoint - _centerPos;
            offset.y = 0f;

            if (offset.magnitude > stirRadius)
                offset = offset.normalized * stirRadius;

            Vector3 targetPos = _centerPos + offset;
            targetPos.y = _yHeight;

            transform.position = targetPos;
            transform.rotation = _initialRot;
        }
    }
}
