// === Billboard.cs (새 파일) ===

using UnityEngine;

public class Billboard : MonoBehaviour
{
    // 메인 카메라의 Transform을 저장할 변수
    private Transform mainCameraTransform;

    void Start()
    {
        // 씬에서 'MainCamera' 태그가 붙은 카메라를 찾습니다.
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("씬에 Main Camera (Tag: MainCamera)가 없습니다!");
        }
    }

    // Update 대신 LateUpdate를 사용하면 카메라 움직임 후에 처리되어 더 부드럽습니다.
    void LateUpdate()
    {
        if (mainCameraTransform == null) return;

        // 1. NPC의 현재 위치에서 카메라를 향하는 방향을 계산합니다.
        // 2. Vector3.up (월드의 위 방향)을 기준으로 회전하여 캐릭터가 누워 보이지 않도록 합니다.
        // 이 LookAt 코드는 NPC가 항상 카메라를 바라보게 만들지만, 
        // 바닥을 뚫고 숙이거나 젖혀지는 현상 없이 Y축 회전을 유지합니다.

        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                         Vector3.up);
    }
}