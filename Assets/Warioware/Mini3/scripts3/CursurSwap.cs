using UnityEngine;

public class CursurSwap : MonoBehaviour

{
    public Texture2D handCursor;     
    public Vector2 hotspot = new Vector2(8, 8); 

    void Start()
    {
        Cursor.visible = true; 
        Cursor.SetCursor(handCursor, hotspot, CursorMode.Auto);
    }
}