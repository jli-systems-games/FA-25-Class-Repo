using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToScene : MonoBehaviour
{
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
