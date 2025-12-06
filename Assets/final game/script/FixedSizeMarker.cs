using UnityEngine;

public class FixedSizeMarker : MonoBehaviour
{
    [Tooltip("마커의 기본 스케일을 결정하는 상수. 이 값이 클수록 마커가 더 크게 보입니다.")]
    public float targetScaleFactor = 0.5f;

    // 초기 로컬 스케일을 저장합니다. (선택 사항, 필요시 주석 해제)
    // private Vector3 _initialScale;

    // void Start()
    // {
    //     _initialScale = transform.localScale;
    // }

    void LateUpdate()
    {
        // 1. 현재 활성화된 메인 카메라를 가져옵니다.
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            // Debug.LogWarning("씬에 Main Camera (태그: MainCamera)가 없습니다.");
            return;
        }

        // 2. NPC 마커와 카메라 사이의 거리 계산
        // Z축 거리가 아닌 3D 공간의 실제 거리를 사용합니다.
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);

        // 3. 스케일 보정 계산
        // 거리에 'targetScaleFactor'를 곱하여, 멀어질수록 스케일이 커지게 합니다.
        // 예를 들어, 거리가 5m이고 Factor가 0.5면 최종 스케일은 2.5가 됩니다.
        float scale = distance * targetScaleFactor;

        // 4. 로컬 스케일 적용 (모든 축에 동일하게 적용)
        transform.localScale = Vector3.one * scale;
    }
}