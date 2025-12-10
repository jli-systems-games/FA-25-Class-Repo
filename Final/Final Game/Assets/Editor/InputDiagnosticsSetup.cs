using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEditor;
using UnityEngine;

/// <summary>
/// InputDiagnostics 设置助手
/// 放在 Assets/Editor/ 文件夹中
/// </summary>
public class InputDiagnosticsSetup : EditorWindow
{
    [MenuItem("Tools/设置 InputDiagnostics")]
    public static void ShowWindow()
    {
        GetWindow<InputDiagnosticsSetup>("InputDiagnostics 设置");
    }

    void OnGUI()
    {
        GUILayout.Label("=== InputDiagnostics 自动设置 ===", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "此工具会自动查找场景中的 InputDiagnostics 对象，\n" +
            "并将所有玩家对象添加到其中。",
            MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("🔍 自动查找并设置玩家", GUILayout.Height(40)))
        {
            AutoSetupPlayers();
        }
    }

    void AutoSetupPlayers()
    {
        // 查找场景中的 InputDiagnostics 对象
        InputDiagnosticsSetup diagnostics = null;

#if UNITY_6000_0_OR_NEWER
        diagnostics = Object.FindFirstObjectByType<InputDiagnosticsSetup>();
#elif UNITY_2023_1_OR_NEWER
        diagnostics = Object.FindFirstObjectByType<InputDiagnostics>();
#else
#pragma warning disable CS0618
        diagnostics = Object.FindObjectOfType<InputDiagnostics>();
#pragma warning restore CS0618
#endif

        if (diagnostics == null)
        {
            EditorUtility.DisplayDialog("错误",
                "场景中没有 InputDiagnostics 对象！\n请先创建一个空对象并添加 InputDiagnostics 组件。",
                "确定");
            return;
        }

        // 查找所有 InputManager
        InputManager[] managers = null;

#if UNITY_6000_0_OR_NEWER
        managers = Object.FindObjectsByType<InputManager>(FindObjectsSortMode.None);
#elif UNITY_2023_1_OR_NEWER
        managers = Object.FindObjectsByType<InputManager>(FindObjectsSortMode.None);
#else
#pragma warning disable CS0618
        managers = Object.FindObjectsOfType<InputManager>();
#pragma warning restore CS0618
#endif

        if (managers.Length == 0)
        {
            EditorUtility.DisplayDialog("错误",
                "场景中没有找到任何 InputManager！\n请确保玩家对象已经添加到场景中。",
                "确定");
            return;
        }

        // 设置玩家数组
        GameObject[] players = new GameObject[4];
        for (int i = 0; i < Mathf.Min(managers.Length, 4); i++)
        {
            players[i] = managers[i].gameObject;
        }

        // 使用反射设置私有数组（因为可能是私有的）
        var type = diagnostics.GetType();
        var playersField = type.GetField("players",
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (playersField != null)
        {
            playersField.SetValue(diagnostics, players);
            EditorUtility.SetDirty(diagnostics);

            EditorUtility.DisplayDialog("成功",
                $"已设置 {Mathf.Min(managers.Length, 4)} 个玩家到 InputDiagnostics！",
                "确定");

            Debug.Log($"✅ [InputDiagnostics] 已设置 {Mathf.Min(managers.Length, 4)} 个玩家");
        }
        else
        {
            EditorUtility.DisplayDialog("错误",
                "无法找到 players 字段！",
                "确定");
        }
    }
}