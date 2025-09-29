using UnityEngine;
using UnityEngine.SceneManagement;

public class Reload : MonoBehaviour
{
    public KeyCode reloadKey = KeyCode.P;

    void Update()
    {
        if (Input.GetKeyDown(reloadKey))
        {
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.name);
        }
    }
}