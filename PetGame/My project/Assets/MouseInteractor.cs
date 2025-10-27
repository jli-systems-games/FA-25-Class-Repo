using UnityEngine;

public interface ISpot { void OnClick(); }

public class MouseInteractor : MonoBehaviour
{
    public Camera cam;
    public Texture2D cursorNormal;
    public Texture2D cursorHover;
    public LayerMask interactableMask;
    ISpot hover;
    Vector2 hs;

    void Start()
    {
        if (!cam) cam = Camera.main;
        if (cursorNormal) { hs = new Vector2(cursorNormal.width / 2, cursorNormal.height / 2); Cursor.SetCursor(cursorNormal, hs, CursorMode.Auto); }
    }

    void Update()
    {
        Ray r = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out RaycastHit h, 200f, interactableMask))
        {
            var s = h.collider.GetComponentInParent<ISpot>();
            if (s != hover) { hover = s; if (cursorHover) Cursor.SetCursor(cursorHover, hs, CursorMode.Auto); }
            if (Input.GetMouseButtonDown(0)) s?.OnClick();
        }
        else
        {
            if (hover != null) { hover = null; if (cursorNormal) Cursor.SetCursor(cursorNormal, hs, CursorMode.Auto); }
        }
    }
}
