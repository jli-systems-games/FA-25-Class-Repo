using UnityEngine;

public class BillboardMarker : MonoBehaviour
{
    // ⭐️ 플레이어 카메라를 Inspector에서 연결
    public Transform targetCamera;

    // ⭐️ 원하는 마커의 고정 크기 (예: 0.1)
    public float fixedScale = 0.1f;

    void Start()
    {
        if (targetCamera == null)
        {
            // Player Camera가 연결되지 않았다면, 메인 카메라를 찾습니다.
            if (Camera.main != null)
            {
                targetCamera = Camera.main.transform;
            }
            else
            {
                Debug.LogError("BillboardMarker: 카메라가 연결되지 않았습니다.");
                enabled = false;
            }
        }
    }

    void LateUpdate()
    {
        if (targetCamera == null) return;

        // 1. 카메라를 바라보도록 회전 (Y축 고정 없이 3D 회전)
        transform.LookAt(transform.position + targetCamera.rotation * Vector3.forward, targetCamera.rotation * Vector3.up);

        // 2. 거리와 상관없이 크기 고정 (World Space Canvas에서 중요)
        float distance = Vector3.Distance(transform.position, targetCamera.position);
        transform.localScale = Vector3.one * (distance * fixedScale);
    }
}