using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the next scene in Build Settings (no need to name it)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        // Works in a built game, not in the Unity editor
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}
