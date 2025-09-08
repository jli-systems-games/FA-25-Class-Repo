using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectManager : MonoBehaviour
{
    public int targetCount = 5;       
    public string winScene = "Win";   
    int current;

    public void ReportCollected()
    {
        current++;
        if (current >= targetCount)
        {
            SceneManager.LoadScene(winScene);
        }
    }
}
