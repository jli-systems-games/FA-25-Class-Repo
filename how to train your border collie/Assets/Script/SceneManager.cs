using UnityEngine;
using UnityEngine.SceneManagement;

public class SimplerSceneManager : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}