using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Line Renderer (optional)")]
    public LineRenderer lineRenderer;   // 인스펙터 연결
    public bool showLineRenderer = false;

    [Header("Interaction Settings")]
    public Camera cam;
    public float interactDistance = 3.5f;
    public KeyCode interactKey = KeyCode.E;

    void Update()
    {
        // 키 입력 또는 마우스 클릭으로 상호작용
        if (Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    void LateUpdate()
    {
        // 시각용 라인 (디버그)
        if (!showLineRenderer || cam == null || lineRenderer == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Vector3 start = ray.origin;
        Vector3 end = start + ray.direction * interactDistance;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            end = hit.point;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    void TryInteract()
    {
        if (cam == null) return;

        // 카메라 앞에서 구형 레이(약간 퍼지는 느낌)로 검사
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        // SphereCast = 약간 둥근 범위로 탐지 → 너무 정확히 맞추지 않아도 됨
        if (Physics.SphereCast(ray, 0.4f, out RaycastHit hit, interactDistance))
        {
            // 맞은 오브젝트에 InteractableItem 스크립트가 있으면 타깃 인식
            var item = hit.collider.GetComponentInParent<InteractableItem>();
            if (item != null)
            {
                // 아이템 찾았다고 TargetItemManager에 알림
                FindFirstObjectByType<TargetItemManager>()?.OnItemFound(item);

                // 시각 효과를 주고 싶다면 (예: 색 바꾸기)
                var renderer = item.GetComponentInChildren<Renderer>();
                if (renderer != null)
                    renderer.material.color = Color.red; // 찾은 아이템 빨갛게 표시
            }
        }
    }
}
