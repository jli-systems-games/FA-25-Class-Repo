using UnityEngine;
using UnityEditor;

/// <summary>
/// Unity编辑器扩展：快速设置场景
/// 在菜单栏添加"Tamagotchi"菜单，提供一键设置功能
/// </summary>
public class QuickSetup : MonoBehaviour
{
    [MenuItem("Tamagotchi/Quick Setup Scene")]
    static void SetupScene()
    {
        // 查找或创建GameInitializer
        GameInitializer initializer = FindFirstObjectByType<GameInitializer>();
        
        if (initializer == null)
        {
            GameObject go = new GameObject("GameInitializer");
            go.AddComponent<GameInitializer>();
            
            Debug.Log("✅ GameInitializer 已创建！场景设置完成。");
            Debug.Log("按下 Play 按钮即可运行游戏。");
            
            // 选中新创建的对象
            Selection.activeGameObject = go;
        }
        else
        {
            Debug.Log("✅ GameInitializer 已存在。场景已经设置好了。");
            Selection.activeGameObject = initializer.gameObject;
        }
        
        // 标记场景为已修改
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );
    }
    
    [MenuItem("Tamagotchi/Remove All Game Objects")]
    static void CleanupScene()
    {
        if (EditorUtility.DisplayDialog(
            "清理场景", 
            "这将删除所有游戏相关的GameObject。确定要继续吗？", 
            "确定", 
            "取消"))
        {
            // 删除所有管理器
            DestroyIfExists<GameInitializer>();
            DestroyIfExists<GameManager>();
            DestroyIfExists<CatManager>();
            DestroyIfExists<InputManager>();
            DestroyIfExists<UIManager>();
            DestroyIfExists<UIBuilder>();
            DestroyIfExists<PlaceholderSpriteGenerator>();
            DestroyIfExists<ControlsHint>();
            
            // 删除Canvas
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                DestroyImmediate(canvas.gameObject);
            }
            
            Debug.Log("✅ 场景已清理。");
        }
    }
    
    [MenuItem("Tamagotchi/Documentation/Open README")]
    static void OpenReadme()
    {
        string path = Application.dataPath + "/../README.md";
        Application.OpenURL("file://" + path);
    }
    
    [MenuItem("Tamagotchi/Documentation/Open Setup Guide")]
    static void OpenSetupGuide()
    {
        string path = Application.dataPath + "/SETUP_GUIDE.md";
        Application.OpenURL("file://" + path);
    }
    
    [MenuItem("Tamagotchi/Documentation/Open Deployment Checklist")]
    static void OpenDeploymentChecklist()
    {
        string path = Application.dataPath + "/DEPLOYMENT_CHECKLIST.md";
        Application.OpenURL("file://" + path);
    }
    
    static void DestroyIfExists<T>() where T : MonoBehaviour
    {
        T component = FindFirstObjectByType<T>();
        if (component != null)
        {
            DestroyImmediate(component.gameObject);
        }
    }
}

