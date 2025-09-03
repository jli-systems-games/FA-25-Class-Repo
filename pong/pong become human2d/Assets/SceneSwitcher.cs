using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [Header("场景走你")]
    public string sceneName;

 
    public void SwitchScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        
    }
}
//我笑死了这个脚本已是我反复利用无数次的脚本
