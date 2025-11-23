// === Billboard.cs (수정된 파일) ===

using UnityEngine;

public class Billboard : MonoBehaviour
{
    // 바라볼 대상 (카메라의 Transform)
    private Transform targetCameraTransform;

    void Start()
    {
        // 1. SceneToggle 찾기 (골드플레이어 카메라 참조용)
        SceneToggle sceneToggle = FindFirstObjectByType<SceneToggle>();

        // 2. 씬에 현재 활성화된 Main Camera가 있는지 확인 (주로 랩 카메라일 가능성)
        if (Camera.main != null)
        {
            // 랩 카메라든 월드 카메라든 현재 'MainCamera' 태그를 가진 카메라를 사용
            targetCameraTransform = Camera.main.transform;
        }
        // 3. Main Camera가 없으면 (태그가 없거나 비활성화되었거나), SceneToggle의 worldCamera 사용
        else if (sceneToggle != null && sceneToggle.worldCamera != null)
        {
            // 골드플레이어 카메라 (worldCamera)를 사용
            targetCameraTransform = sceneToggle.worldCamera.transform;
        }
        else
        {
            Debug.LogError("Billboard: 바라볼 카메라를 찾을 수 없습니다! MainCamera 태그나 SceneToggle의 worldCamera 설정을 확인해주세요.", this);
        }
    }

    void LateUpdate()
    {
        // 타겟 카메라 Transform이 없으면 아무것도 하지 않습니다.
        if (targetCameraTransform == null) return;

        // NPC의 Y축 회전만 유지하면서, 카메라를 정면으로 바라보게 합니다.
        transform.LookAt(transform.position + targetCameraTransform.rotation * Vector3.forward,
                         Vector3.up);

        // 💡 다른 방식 (카메라 위치 자체를 바라보기):
        // transform.LookAt(targetCameraTransform.position, Vector3.up);
        // *두 방식 중 더 자연스러운 것을 선택하여 사용하시면 됩니다. 위에 코드는 카메라의 회전(시선) 방향을 따라가므로 NPC가 더 플랫하게 느껴질 수 있습니다.
    }
}