using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class timer : MonoBehaviour
{
    public GameObject failScreen;   // assign in Inspector
    public float waitBeforeFail = 10f;
    public float waitBeforeRestart = 10f;

    void Start()
    {
        StartCoroutine(FailSequence());
    }

    IEnumerator FailSequence()
    {
        // wait before failing
        yield return new WaitForSeconds(waitBeforeFail);

        // show fail screen
        if (failScreen != null)
        {
            failScreen.SetActive(true);

            // make sure its Animator runs even when timeScale = 0
            Animator anim = failScreen.GetComponent<Animator>();
            if (anim != null)
            {
                anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
        }

        // pause game
        Time.timeScale = 0f;

        // wait in realtime so pause doesn't stop the countdown
        yield return new WaitForSecondsRealtime(waitBeforeRestart);

        // unpause and restart scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
