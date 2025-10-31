using UnityEngine;

public class Cursorunlock : MonoBehaviour
{
    void Start()
    {
  
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
