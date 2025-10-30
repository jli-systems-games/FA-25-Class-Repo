using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnR : MonoBehaviour
{
    public KeyCode restartKey = KeyCode.R;
    public float delay = 0f;

    void Update()
    {
        if (Input.GetKeyDown(restartKey))
        {
            if (delay <= 0f) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else StartCoroutine(ReloadAfter(delay));
        }
    }

    System.Collections.IEnumerator ReloadAfter(float t)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
