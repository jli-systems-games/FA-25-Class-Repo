using UnityEngine;

public class FPCursorLock : MonoBehaviour
{
    [Header("Behavior")]
    public bool lockOnStart = true;     
    public bool allowEscapeToggle = true;   
    public KeyCode toggleKey = KeyCode.Escape;


    CursorLockMode _prevLockMode;
    bool _prevVisible;
    bool _userUnlocked; 

    void Awake()
    {

        _prevLockMode = Cursor.lockState;
        _prevVisible = Cursor.visible;
    }

    void OnEnable()
    {
        if (lockOnStart) LockCursor();
    }

    void OnDisable()
    {

        RestoreCursor();
    }

    void OnApplicationFocus(bool hasFocus)
    {

        if (!hasFocus)
        {
            UnlockCursor();
        }
        else if (lockOnStart && !_userUnlocked)
        {
            LockCursor();
        }
    }

    void Update()
    {
        if (!allowEscapeToggle) return;

        if (Input.GetKeyDown(toggleKey))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                _userUnlocked = true;
                UnlockCursor();
            }
            else
            {
                _userUnlocked = false;
                LockCursor();
            }
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void RestoreCursor()
    {
        Cursor.lockState = _prevLockMode;
        Cursor.visible = _prevVisible;
    }
}