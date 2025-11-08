using UnityEngine;
using UnityEngine.SceneManagement;

public class unlockcursoronscene : MonoBehaviour
{
    void Start()
    {
        unlock();
    }

    void OnEnable()
    {
        unlock();
    }

    void unlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}