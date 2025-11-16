using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 场景切换管理器 - 挂载到按钮或空物体上
/// </summary>
public class SceneSwitcher : MonoBehaviour
{
    [Header("场景名称")]
    public string targetSceneName = "GameScene";

    [Header("可选：延迟跳转")]
    public float delaySeconds = 0f;

    // 通过按钮调用这个方法
    public void LoadScene()
    {
        if (delaySeconds > 0)
        {
            Invoke("DoLoadScene", delaySeconds);
        }
        else
        {
            DoLoadScene();
        }
    }

    // 加载指定场景（可以在 Inspector 中输入场景名）
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("场景名称为空！");
            return;
        }

        Debug.Log($"正在加载场景: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // 重新加载当前场景
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"重新加载场景: {currentScene}");
        SceneManager.LoadScene(currentScene);
    }

    // 退出游戏
    public void QuitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();

        // 在编辑器中停止播放
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // 内部方法：执行场景加载
    void DoLoadScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("目标场景名称未设置！");
            return;
        }

        Debug.Log($"正在加载场景: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);
    }

    // 可选：Start时自动绑定按钮
    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(LoadScene);
            Debug.Log($"已自动绑定按钮到场景切换功能");
        }
    }
}
