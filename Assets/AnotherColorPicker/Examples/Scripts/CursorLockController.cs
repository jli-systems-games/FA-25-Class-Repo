using UnityEngine;
using UnityEngine.EventSystems;

public class CursorLockController : MonoBehaviour
{

    public KeyCode toggleKey = KeyCode.P; 

    private bool isUnlock = false;

    void Start()
    {
        lockCursor(); 
    }

    void Update()
    {
  
        if (Input.GetKeyDown(toggleKey))
        {
            if (isUnlock)
                lockCursor();
            else
                unlockCursor();
        }

 
        if (Input.GetMouseButtonDown(0))
        {
 
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (isUnlock)
                lockCursor();
        }
    }

    void lockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isUnlock = false;
    }

    void unlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isUnlock = true;
    }
}