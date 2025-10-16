using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetAndLoadScene : MonoBehaviour
{
    [Header("sceneÕâ¸öÌø×ª")]
    public string targetSceneName;

   
    public void OnResetAndLoad()
    {
 
        Data.clickedButtons.Clear();
        Data.colorRecords.Clear();


        SceneManager.LoadScene(targetSceneName);
    }
}
