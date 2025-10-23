using UnityEngine;
using UnityEngine.SceneManagement;

public class StatusSceneManager : MonoBehaviour
{
    public BorderCollieStats stats;
    public string successScene = "CollegeScene"; 
    public string failScene = "RebellionScene"; 

    void Update()
    {
        CheckConditions();
    }

    void CheckConditions()
    {
     
        if (stats.Skill >= 100)
        {
            LoadScene(successScene);
            return;
        }

    
        if (stats.Energy <= 0 || stats.Happiness <= 0 || stats.Fullness <= 0)
        {
            LoadScene(failScene);
            return;
        }
    }

    void LoadScene(string sceneName)
    {

        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}