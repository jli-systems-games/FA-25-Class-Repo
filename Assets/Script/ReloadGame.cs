using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadGame : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
}