using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadSceneSimple : MonoBehaviour
{
    public string sceneName;   
    public float delay = 0f;   

    public void LoadMyScene()
    {
        StartCoroutine(LoadAfterDelay());
    }

    private System.Collections.IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
