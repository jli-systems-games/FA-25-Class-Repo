using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int currentGameIndex = 0;

    [SerializeField] private string[] gameScenes = { "EyeBallGame", "WandGame", "CardGame" };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        currentGameIndex = 0;
        LoadNextGame();
    }

    public void LoadNextGame()
    {
        if (currentGameIndex < gameScenes.Length)
        {
            string nextScene = gameScenes[currentGameIndex];
            Debug.Log("Loading scene: " + nextScene);
            SceneManager.LoadScene(nextScene);

            currentGameIndex++; 
        }
        else
        {
            Debug.Log("Loading scene: VictoryScreen");
            SceneManager.LoadScene("VictoryScreen");
        }
    }

    public void GameOver()
    {
        Debug.Log("Loading scene: GameOverScreen");
        SceneManager.LoadScene("GameOverScreen");
    }

    public void BackToMenu()
    {
        Debug.Log("Loading scene: MainMenu");
        SceneManager.LoadScene("MainMenu");
    }
}

