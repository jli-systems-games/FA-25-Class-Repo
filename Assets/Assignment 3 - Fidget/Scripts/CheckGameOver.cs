using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckGameOver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Black"))
        {
            SceneManager.LoadScene("Game Over Scene");
        }
        else if (other.gameObject.CompareTag("Button"))
        {
            SceneManager.LoadScene("Game Complete Scene");
        }
    }
}
