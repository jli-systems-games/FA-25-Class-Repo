using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject startMenuUI;   
    public string gameSceneName = "Garden";  

    void Update()
    {
        if (startMenuUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        Debug.Log("🎮 Loading scene: " + gameSceneName);

        
        SceneManager.LoadScene(gameSceneName);
    }
}
