using UnityEngine;
using UnityEngine.SceneManagement;

public class Reload : MonoBehaviour
{
    public KeyCode reloadKey = KeyCode.R;
    public string startSceneName = "Welcome";

    void Update()
    {
        if (Input.GetKeyDown(reloadKey))
        {
            ResetToStartScene();
        }
    }

    void ResetToStartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneName);
    }
}