using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStart : MonoBehaviour
{
    public string nextSceneName = "Intro";

    public void StartGame()
    {
        SceneManager.LoadScene(nextSceneName);
        Debug.Log("Loading scene: " + nextSceneName);
    }
}
