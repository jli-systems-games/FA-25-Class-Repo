using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ChangeScene : MonoBehaviour
{
    public void ClearScore(GameManager gameManager)
    {
        gameManager.score = 0;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
