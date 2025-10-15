using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;

            int nextIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;

            SceneManager.LoadScene(1);
        }
    }
}