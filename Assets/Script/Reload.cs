using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReset : MonoBehaviour
{

    public void ResetCurrentScene()
    {

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }


    public void ResetToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}