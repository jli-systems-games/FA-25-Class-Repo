using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    // Name of your main menu scene
    public string menuSceneName = "Menu";

    // Called when player clicks "Return to Menu"
    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}

