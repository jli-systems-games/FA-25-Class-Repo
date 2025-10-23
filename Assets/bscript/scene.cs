using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
   
    public void SwitchToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
