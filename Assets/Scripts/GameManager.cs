using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int scenesSwitched;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        GameObject tempGameManager = 
            GameObject.FindGameObjectWithTag("GameManager");
        if (tempGameManager != gameObject)
        {
            Destroy(gameObject);
        }

        Data.globalScore = 0;

    }

    public void SceneSwitcher(string newScene)
    {
        SceneManager.LoadScene(newScene);

        Data.globalScore++;
        Debug.Log(Data.globalScore);
    }
}
