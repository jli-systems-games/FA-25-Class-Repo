using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public int sceneIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetAll();
        }
        SceneManager.LoadScene(sceneIndex);
    }
}