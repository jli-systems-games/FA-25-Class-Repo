using UnityEngine;

public class CursorSwap : MonoBehaviour
{
    public Texture2D handCursor;
    public Vector2 hotspot = new Vector2(8, 8);

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Cursor.SetCursor(handCursor, hotspot, CursorMode.Auto);
    }

    void OnDisable()
    {
        ResetCursor();
    }

    void OnDestroy()
    {
        ResetCursor();
    }

    void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
