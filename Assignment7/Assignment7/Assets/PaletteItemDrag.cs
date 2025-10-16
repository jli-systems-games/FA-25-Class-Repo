using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaletteItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GarageRuntimeUI ui;
    public PaletteKind kind;
    public FrameType frameType;
    public int wheelItemIndex;

    Image img;
    GameObject ghost;
    Camera cam;

    void Awake()
    {
        img = GetComponent<Image>();
        cam = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ghost = new GameObject("Ghost", typeof(SpriteRenderer));
        var sr = ghost.GetComponent<SpriteRenderer>();
        sr.sprite = img.sprite;
        sr.sortingOrder = 100;
        sr.color = new Color(1, 1, 1, 0.7f);
        if (kind == PaletteKind.Wheel) ghost.transform.localScale = Vector3.one * ui.GetWheelWorldScale(wheelItemIndex);
        UpdateGhost(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghost) UpdateGhost(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!ghost) return;
        if (ui.IsInBuildAreaScreen(eventData.position))
        {
            Vector3 p = cam.ScreenToWorldPoint(eventData.position); p.z = 0;
            if (kind == PaletteKind.Frame) ui.PlaceBody(p, frameType);
            else ui.PlaceWheel(p, wheelItemIndex);
        }
        GameObject.Destroy(ghost);
        ghost = null;
    }

    void UpdateGhost(Vector2 screenPos)
    {
        Vector3 p = cam.ScreenToWorldPoint(screenPos);
        p.z = 0;
        ghost.transform.position = p;
    }
}

