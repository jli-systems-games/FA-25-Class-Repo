using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    private string sceneName = "ColdScene";
    
    public void LoadGame()
    {
        SceneManager.LoadScene(sceneName);

    }
}