using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    [Tooltip("失败场景名（记得放到 Build Settings 里）")]
    public string failSceneName = "FailScene";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player 撞到 KillZone，切换到失败场景: " + failSceneName);
            SceneManager.LoadScene(failSceneName);
        }
    }
}
