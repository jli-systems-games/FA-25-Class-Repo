using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera cam;
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }

        // 마우스 클릭으로 하고 싶으면 여기도 같이
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            var item = hit.collider.GetComponentInParent<InteractableItem>();
            if (item != null)
            {
                // 찾았다고 TargetItemManager에 알려주기
                FindFirstObjectByType<TargetItemManager>().OnItemFound(item);

                // 아이템을 없애고 싶으면
                // Destroy(item.gameObject);
            }
        }
    }
}
