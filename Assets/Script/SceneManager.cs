using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneFunction(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}