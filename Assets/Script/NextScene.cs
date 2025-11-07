using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public string nextSceneName;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}