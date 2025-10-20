using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreenUI : MonoBehaviour
{
    public void PlayAgain()
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

