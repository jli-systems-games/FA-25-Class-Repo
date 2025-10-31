using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : MonoBehaviour
{

    public string targetSceneName;


    public void Teleport()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}
