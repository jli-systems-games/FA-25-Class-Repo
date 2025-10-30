using UnityEngine;

public class CursorManager : MonoBehaviour
{
    void Start()
    {
        //Remove cursor visibility and keep it at the center, so that rotation can still occur even when the cursor reaches the end of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
