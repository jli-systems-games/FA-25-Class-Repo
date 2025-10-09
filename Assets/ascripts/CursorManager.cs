using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D clickedCursor;
    private Vector2 hotspot;

    void Start()
    {
        hotspot = new Vector2(defaultCursor.width / 2, defaultCursor.height / 2);
        SetCursor(defaultCursor);
    }

    public void SetCursor(Texture2D newCursor)
    {
        Cursor.SetCursor(newCursor, hotspot, CursorMode.Auto);
    }


    public void OnButtonClickChangeCursor()
    {
        SetCursor(clickedCursor);
    }
    public void OnButtonClickCursor()
    {
        SetCursor(defaultCursor);
    }
}
