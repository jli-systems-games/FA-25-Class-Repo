using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingMan : MonoBehaviour
{

    public void LoadMainScene()
    {
        SceneManager.LoadScene("main");
    }
}
