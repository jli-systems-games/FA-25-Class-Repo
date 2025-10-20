using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void Retry()
    {
        GameManager.Instance.StartGame();
    }

    public void MainMenu()
    {
        GameManager.Instance.BackToMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
