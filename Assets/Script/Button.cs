using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public int sceneIndex = 0;

    public void LoadSceneByIndex()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}