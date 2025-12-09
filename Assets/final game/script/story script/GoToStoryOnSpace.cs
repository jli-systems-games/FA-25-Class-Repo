using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToStoryOnSpace : MonoBehaviour
{
    [Header("넘어갈 스토리 씬 이름")]
    public string storySceneName = "story"; 

    void Update()
    {
        // 스페이스바를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(storySceneName);
        }
    }
}
