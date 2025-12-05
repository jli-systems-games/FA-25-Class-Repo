using UnityEngine;

public class LabItemClickPicker : MonoBehaviour
{
    [Tooltip("클릭 감지 거리")]
    public float maxDistance = 5f;

    void Update()
    {
        // 1. 마우스 왼쪽 버튼을 눌렀는지 확인
        if (Input.GetMouseButtonDown(0))
        {
            TryPickUpItem();
        }
    }

    void TryPickUpItem()
    {
        // 현재 활성화된 메인 카메라를 가져옵니다.
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("씬에 메인 카메라(Tag: MainCamera)가 없습니다.");
            return;
        }

        // 마우스 커서 위치에서 레이를 생성합니다.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 2. 레이캐스트 실행
        // LayerMask를 사용하여 줍기 가능한 아이템만 감지하도록 최적화할 수 있습니다.
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // 3. 충돌한 오브젝트에서 WorldItemPickup 컴포넌트 찾기
            // 결과 아이템 월드 프리팹에는 WorldItemPickup 스크립트가 붙어 있어야 합니다.
            WorldItemPickup pickup = hit.collider.GetComponent<WorldItemPickup>();

            if (pickup != null)
            {
                // 4. 아이템 줍기 함수 호출
                pickup.Pickup();
            }
        }
    }
}