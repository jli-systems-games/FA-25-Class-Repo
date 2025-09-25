using UnityEngine;

public class Audio : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
