using UnityEngine;

public class LadleStirController : MonoBehaviour
{
    [Header("국자가 움직일 수 있는 반경")]
    public float stirRadius = 0.5f;   // 처음 위치를 중심으로 이 반경 안에서만 움직임

    bool _isDragging = false;
    float _yHeight;           // 국자의 고정 높이
    Vector3 _centerPos;       // 국자가 처음 있던 위치 (원 중심)
    Quaternion _initialRot;   // 처음 회전값 저장

    void Start()
    {
        // 국자의 높이 + 시작 위치 + 시작 회전 저장
        _yHeight = transform.position.y;
        _centerPos = transform.position;   // ★ 처음 둔 자리 기준으로 원
        _initialRot = transform.rotation;  // ★ 시작 회전 그대로 유지
    }

    void OnMouseDown()
    {
        _isDragging = true;
    }

    void OnMouseUp()
    {
        _isDragging = false;
    }

    void Update()
    {
        if (!_isDragging) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 국자가 움직일 평면 (y 높이 고정)
        Plane plane = new Plane(Vector3.up, new Vector3(0f, _yHeight, 0f));

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            // 시작 위치(_centerPos) 기준으로 오프셋 계산
            Vector3 offset = hitPoint - _centerPos;
            offset.y = 0f;

            // 반경 밖으로 못 나가게 길이 제한
            if (offset.magnitude > stirRadius)
            {
                offset = offset.normalized * stirRadius;
            }

            // 최종 위치 = 시작 위치 + 제한된 오프셋
            Vector3 targetPos = _centerPos + offset;
            targetPos.y = _yHeight;

            transform.position = targetPos;

            // ★ 회전은 처음 값으로 고정 (LookAt 안 함)
            transform.rotation = _initialRot;
        }
    }
}
