using UnityEngine;
using UnityEngine.SceneManagement;

public class GoScene : MonoBehaviour
{
    public string sceneName;
    
    public void GoToScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
