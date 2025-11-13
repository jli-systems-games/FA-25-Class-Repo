using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSCene : MonoBehaviour
{
    public void GoToSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
