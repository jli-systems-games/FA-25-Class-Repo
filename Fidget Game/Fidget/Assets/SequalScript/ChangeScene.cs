using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeSceneOnCollision : MonoBehaviour
{
    public string sceneName = "NextScene";
    public float delay = 0f;
    public string requiredTag = "Player";

    void OnCollisionEnter(Collision collision)
    {
        if (string.IsNullOrEmpty(requiredTag) || collision.collider.CompareTag(requiredTag))
            StartCoroutine(LoadAfterDelay());
    }

    IEnumerator LoadAfterDelay()
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
