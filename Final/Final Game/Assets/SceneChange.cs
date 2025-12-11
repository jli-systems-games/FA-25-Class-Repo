using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用这个才能切换场景
using UnityEngine.UI;              // 引用这个来识别 Button

public class ClickChangeScene : MonoBehaviour
{
    [Header("配置")]
    [Tooltip("你要跳转的目标场景名字 (必须完全一致)")]
    public string TargetSceneName = "MyGame";

    void Start()
    {
        // 尝试获取物体上的 Button 组件
        Button btn = GetComponent<Button>();

        // 如果这个物体是个按钮，自动绑定点击事件
        if (btn != null)
        {
            btn.onClick.AddListener(LoadTargetScene);
        }
    }

    // 你也可以在其他地方手动调用这个方法
    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(TargetSceneName))
        {
            Debug.LogError("❌ 场景名字没填！请在 Inspector 里设置 TargetSceneName");
            return;
        }

        // 检查场景是否在 Build Settings 里 (仅编辑器有效)
#if UNITY_EDITOR
        if (!IsSceneInBuildSettings(TargetSceneName))
        {
            Debug.LogError($"❌ 场景 '{TargetSceneName}' 没有添加到 Build Settings！请去 File -> Build Settings 添加它。");
            return;
        }
#endif

        Debug.Log($"🚀 正在跳转到场景: {TargetSceneName} ...");
        SceneManager.LoadScene(TargetSceneName);
    }

#if UNITY_EDITOR
    // 辅助检查：防止你忘了加场景到 Build Settings
    bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < UnityEditor.EditorBuildSettings.scenes.Length; i++)
        {
            string path = UnityEditor.EditorBuildSettings.scenes[i].path;
            if (path.Contains(sceneName)) return true;
        }
        return false;
    }
#endif
}