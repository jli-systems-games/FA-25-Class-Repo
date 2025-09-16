using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    // 这个方法会在 Button 的 OnClick 里调用
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 如果你喜欢用 BuildIndex 也可以
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // 退出游戏（可选）
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
