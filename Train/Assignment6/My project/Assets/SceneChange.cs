using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;   

    public void LoadTarget()
    {
        StartCoroutine(DelayLoad());
    }

    private System.Collections.IEnumerator DelayLoad()
    {
        yield return new WaitForSeconds(0.8f); 
        SceneManager.LoadScene(sceneName);
    }
}
