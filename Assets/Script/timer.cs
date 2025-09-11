using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class timer : MonoBehaviour
{
    public GameObject failScreen;   
    public float waitBeforeFail = 10f;
    public float waitBeforeRestart = 10f;
    public GameObject restart;
    void Start()
    {
        StartCoroutine(FailSequence());
    }

    IEnumerator FailSequence()
    {
        
        yield return new WaitForSeconds(waitBeforeFail);

       
        if (failScreen != null)
        {
            failScreen.SetActive(true);
            restart.SetActive(false);
            // make sure its Animator runs even when timeScale = 0
            Animator anim = failScreen.GetComponent<Animator>();
            if (anim != null)
            {
                anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
        }

       
        Time.timeScale = 0f;

       
        yield return new WaitForSecondsRealtime(waitBeforeRestart);

       
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
