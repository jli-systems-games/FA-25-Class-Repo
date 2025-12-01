using UnityEngine;

public class CenterClickPicker : MonoBehaviour
{
    [Header("카메라 (골드플레이어 카메라)")]
    public Camera playerCamera;

    [Header("집을 수 있는 최대 거리")]
    public float maxDistance = 3f;

    [Header("어느 버튼으로 집을지")]
    public int mouseButton = 0; // 0 = 왼쪽 클릭

    void Update()
    {
        // 마우스 클릭되면
        if (Input.GetMouseButtonDown(mouseButton))
        {
            TryPickupAtCenter();
        }
    }

    void TryPickupAtCenter()
    {
        if (playerCamera == null) return;

        // 화면 중앙 위치 계산 (크로스헤어 자리)
        Vector3 screenCenter = new Vector3(
            Screen.width / 2f,
            Screen.height / 2f,
            0f
        );

        // 중앙에서 앞으로 레이 쏘기
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            // 맞은 오브젝트에서 WorldItemPickup 찾기
            WorldItemPickup pickup = hit.collider.GetComponent<WorldItemPickup>();

            if (pickup != null)
            {
                pickup.Pickup();
                // Debug.Log("Picked up: " + hit.collider.name);
            }
        }
    }
}
