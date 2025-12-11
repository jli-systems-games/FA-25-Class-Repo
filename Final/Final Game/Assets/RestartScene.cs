using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    void Update()
    {
        // 检测是否按下 R 键
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 1. 恢复时间流动 (防止重开后还是暂停状态)
            Time.timeScale = 1f;

            // 2. 重新加载当前场景
            SceneManager.LoadScene("StartScene");
        }
    }
}