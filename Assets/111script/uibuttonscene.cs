using UnityEngine;
using UnityEngine.SceneManagement;

public class uibuttonscene : MonoBehaviour
{
    public string scene_name = "nextscene";

    public void go()
    {
        SceneManager.LoadScene(scene_name);
    }
}
