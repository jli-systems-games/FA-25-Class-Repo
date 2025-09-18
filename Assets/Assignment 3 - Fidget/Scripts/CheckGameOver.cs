using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CheckGameOver : MonoBehaviour
{
    public Canvas damageCanvas;
    public GameObject confettiParticle;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Black"))
        {
            Debug.Log("Triggered Black");
            damageCanvas.gameObject.SetActive(true);
            StartCoroutine(DelayBeforeDeath(1f));
        }
        else if (other.gameObject.CompareTag("Button"))
        {
            Debug.Log("Triggered Button");
            confettiParticle.SetActive(true);
            StartCoroutine(DelayBeforeCompletion(1f));
        }
    }

    private IEnumerator DelayBeforeCompletion(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("Game Complete Scene");
    }

    private IEnumerator DelayBeforeDeath(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("Game Over Scene");
    }
}
