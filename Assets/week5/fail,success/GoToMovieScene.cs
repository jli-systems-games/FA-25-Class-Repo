using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMovieScene : MonoBehaviour

{
    [SerializeField] private string sceneName = "movie";

    public void LoadMovie()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMovieAsync()
    {
        StartCoroutine(LoadAsync());
    }

    private System.Collections.IEnumerator LoadAsync()
    {
        var op = SceneManager.LoadSceneAsync(sceneName);

        yield return op;
    }
}