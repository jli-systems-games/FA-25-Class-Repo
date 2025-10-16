using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager.LoadScene("Game Scene");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene("Start Scene");
    }

    public void StartGameScene()
    {
        SceneManager.LoadScene("Customizer Scene");
    }
}
