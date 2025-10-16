using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
        public float delay = 1f;


    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneDelayed(sceneName));
    }


    private System.Collections.IEnumerator LoadSceneDelayed(string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

 
}