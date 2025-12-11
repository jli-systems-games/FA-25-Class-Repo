/*using UnityEngine;
using UnityEditor;

/// <summary>
/// InputDiagnostics 设置助手 - 超级简化版（完全无错误）
/// 放在 Assets/Editor/ 文件夹中
/// 
/// 这个版本不依赖任何 TopDown Engine 的特殊API
/// 只使用标准的 Unity Editor API
/// </summary>
public class InputDiagnosticsSetup : EditorWindow
{
    private GameObject diagnosticsObject;
    private GameObject player1;
    private GameObject player2;
    private GameObject player3;
    private GameObject player4;

    [MenuItem("Tools/设置 InputDiagnostics")]
    public static void ShowWindow()
    {
        GetWindow<InputDiagnosticsSetup>("InputDiagnostics 设置");
    }

    void OnGUI()
    {
        GUILayout.Label("=== InputDiagnostics 手动设置 ===", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "手动拖入 InputDiagnostics 对象和4个玩家，\n" +
            "然后点击\"应用设置\"按钮。",
            MessageType.Info);

        GUILayout.Space(10);

        // InputDiagnostics 对象
        diagnosticsObject = EditorGUILayout.ObjectField(
            "InputDiagnostics 对象",
            diagnosticsObject,
            typeof(GameObject),
            true) as GameObject;

        GUILayout.Space(10);

        // 4个玩家
        player1 = EditorGUILayout.ObjectField("Player 1", player1, typeof(GameObject), true) as GameObject;
        player2 = EditorGUILayout.ObjectField("Player 2", player2, typeof(GameObject), true) as GameObject;
        player3 = EditorGUILayout.ObjectField("Player 3", player3, typeof(GameObject), true) as GameObject;
        player4 = EditorGUILayout.ObjectField("Player 4", player4, typeof(GameObject), true) as GameObject;

        GUILayout.Space(20);

        GUI.enabled = diagnosticsObject != null;

        if (GUILayout.Button("✅ 应用设置", GUILayout.Height(40)))
        {
            ApplySettings();
        }

        GUI.enabled = true;

        GUILayout.Space(10);

        if (GUILayout.Button("🔍 自动查找玩家", GUILayout.Height(30)))
        {
            AutoFindPlayers();
        }
    }

    void ApplySettings()
    {
        if (diagnosticsObject == null)
        {
            EditorUtility.DisplayDialog("错误", "请先拖入 InputDiagnostics 对象！", "确定");
            return;
        }

        // 查找 InputDiagnostics 组件
        var diagnostics = diagnosticsObject.GetComponent<InputDiagnosticsSetup>();
        if (diagnostics == null)
        {
            EditorUtility.DisplayDialog("错误",
                "选中的对象没有 InputDiagnostics 组件！",
                "确定");
            return;
        }

        // 使用反射设置玩家字段
        var type = diagnostics.GetType();

        SetPlayerField(diagnostics, type, "player1", player1);
        SetPlayerField(diagnostics, type, "player2", player2);
        SetPlayerField(diagnostics, type, "player3", player3);
        SetPlayerField(diagnostics, type, "player4", player4);

        EditorUtility.SetDirty(diagnostics);

        int count = 0;
        if (player1 != null) count++;
        if (player2 != null) count++;
        if (player3 != null) count++;
        if (player4 != null) count++;

        EditorUtility.DisplayDialog("成功",
            $"已设置 {count} 个玩家！",
            "确定");

        Debug.Log($"✅ [InputDiagnostics] 已设置 {count} 个玩家");
    }

    void SetPlayerField(object obj, System.Type type, string fieldName, GameObject value)
    {
        var field = type.GetField(fieldName,
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }

    void AutoFindPlayers()
    {
        // 查找所有带有特定组件的对象
        Component[] allComponents = null;

#if UNITY_6000_0_OR_NEWER
        allComponents = Object.FindObjectsByType<Component>(FindObjectsSortMode.None);
#elif UNITY_2023_1_OR_NEWER
        allComponents = Object.FindObjectsByType<Component>(FindObjectsSortMode.None);
#else
#pragma warning disable CS0618
        allComponents = Object.FindObjectsOfType<Component>();
#pragma warning restore CS0618
#endif

        int playerCount = 0;

        foreach (var comp in allComponents)
        {
            // 检查对象名称
            string name = comp.gameObject.name.ToLower();

            if (playerCount == 0 && (name.Contains("player1") || name.Contains("p1")))
            {
                player1 = comp.gameObject;
                playerCount++;
            }
            else if (playerCount == 1 && (name.Contains("player2") || name.Contains("p2")))
            {
                player2 = comp.gameObject;
                playerCount++;
            }
            else if (playerCount == 2 && (name.Contains("player3") || name.Contains("p3")))
            {
                player3 = comp.gameObject;
                playerCount++;
            }
            else if (playerCount == 3 && (name.Contains("player4") || name.Contains("p4")))
            {
                player4 = comp.gameObject;
                playerCount++;
                break; // 找到4个就停止
            }
        }

        if (playerCount > 0)
        {
            Repaint(); // 刷新窗口显示
            Debug.Log($"✅ 自动找到 {playerCount} 个玩家");
        }
        else
        {
            EditorUtility.DisplayDialog("提示",
                "未找到玩家对象。\n请确保玩家对象名称包含 \"Player1\", \"Player2\" 等。",
                "确定");
        }
    }
}*/