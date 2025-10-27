using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
{
    public int sceneIndex = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}