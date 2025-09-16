using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController I;

    [Header("Scenes")]
    public string failSceneName = "Scene1";
    public string winSceneName = "Scene3";
    public float loadDelay = 0.3f;

    void Awake()
    {
        I = this;
    }

    public void Fail()
    {
        StartCoroutine(CoLoad(failSceneName));
    }

    public void Win()
    {
        StartCoroutine(CoLoad(winSceneName));
    }

    IEnumerator CoLoad(string sceneName)
    {
        yield return new WaitForSeconds(loadDelay);
        SceneManager.LoadScene(sceneName);
    }
}
