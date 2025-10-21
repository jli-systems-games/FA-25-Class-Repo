using UnityEngine;
using UnityEngine.EventSystems;

public class FoodDragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform draggedFood; // 드래그용 이미지
    private bool dragging = false;

    void Start()
    {
        if (draggedFood != null)
            draggedFood.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        if (draggedFood != null)
            draggedFood.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        if (draggedFood != null)
            draggedFood.gameObject.SetActive(false);
    }

    void Update()
    {
        if (dragging && draggedFood != null)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                draggedFood.parent as RectTransform,
                Input.mousePosition,
                null,
                out pos
            );
            draggedFood.anchoredPosition = pos;
        }
    }
}
