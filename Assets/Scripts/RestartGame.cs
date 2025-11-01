using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 按R键重启当前场景
/// </summary>
public class RestartGame : MonoBehaviour
{
    void Update()
    {
        // 按R键重启游戏
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartCurrentScene();
        }
    }

    void RestartCurrentScene()
    {
        // 获取当前活动场景并重新加载
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}


