using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CheckGameOver : MonoBehaviour
{
    public Canvas damageCanvas;
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
            SceneManager.LoadScene("Game Complete Scene");
        }
    }

    private IEnumerator DelayBeforeDeath(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("Game Over Scene");
    }
}
