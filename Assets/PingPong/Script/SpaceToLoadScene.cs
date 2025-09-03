using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceToLoadScene : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "Ping Pong"; // 이동할 씬 이름

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
