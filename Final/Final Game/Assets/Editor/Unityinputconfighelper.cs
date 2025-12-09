using UnityEngine;
using UnityEditor;

/// <summary>
/// Unity 输入配置助手 - 显示需要手动添加的输入轴
/// 放在 Assets/Editor/ 文件夹中
/// </summary>
public class UnityInputConfigHelper : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("Tools/Unity 输入配置助手")]
    public static void ShowWindow()
    {
        GetWindow<UnityInputConfigHelper>("输入配置助手");
    }

    void OnGUI()
    {
        GUILayout.Label("=== Unity Input Manager 配置助手 ===", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "由于 Unity API 限制，无法自动添加输入轴。\n" +
            "请根据下面的配置，手动在 Input Manager 中添加这些输入轴。\n\n" +
            "路径: Edit -> Project Settings -> Input Manager",
            MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("📂 打开 Input Manager", GUILayout.Height(30)))
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/InputManager.asset");
            EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
        }

        GUILayout.Space(10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        ShowGamepadConfig(1, "手柄1 (P3)");
        GUILayout.Space(20);
        ShowGamepadConfig(2, "手柄2 (P4)");

        EditorGUILayout.EndScrollView();
    }

    void ShowGamepadConfig(int joystickNum, string label)
    {
        GUILayout.Label($"=== {label} ===", EditorStyles.boldLabel);
        GUILayout.Space(5);

        string prefix = $"Joystick{joystickNum}";

        // 左摇杆 X
        ShowAxis($"{prefix}Axis1", "左摇杆 X", "X axis", joystickNum);

        // 左摇杆 Y
        ShowAxis($"{prefix}Axis2", "左摇杆 Y", "Y axis", joystickNum, true);

        // 右摇杆 X
        ShowAxis($"{prefix}Axis4", "右摇杆 X", "4th axis", joystickNum);

        // 右摇杆 Y
        ShowAxis($"{prefix}Axis5", "右摇杆 Y", "5th axis", joystickNum, true);

        GUILayout.Space(10);

        // 按钮
        GUILayout.Label("按钮配置:", EditorStyles.boldLabel);
        for (int i = 0; i < 10; i++)
        {
            ShowButton($"{prefix}Button{i}", $"手柄{joystickNum}按钮{i}", i, joystickNum);
        }
    }

    void ShowAxis(string name, string description, string axis, int joystickNum, bool invert = false)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Label($"Name: {name} ({description})", EditorStyles.boldLabel);
        GUILayout.Label($"• Type: Joystick Axis");
        GUILayout.Label($"• Axis: {axis}");
        GUILayout.Label($"• Joy Num: Joystick {joystickNum}");
        if (invert)
            GUILayout.Label($"• Invert: ✓");
        GUILayout.Label($"• Sensitivity: 1");
        GUILayout.Label($"• Dead: 0.19");
        GUILayout.Label($"• Type: Joystick Axis");

        EditorGUILayout.EndVertical();
        GUILayout.Space(5);
    }

    void ShowButton(string name, string description, int buttonNum, int joystickNum)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        GUILayout.Label($"Name: {name}", GUILayout.Width(150));
        GUILayout.Label($"Positive Button: joystick {joystickNum} button {buttonNum}", GUILayout.Width(250));
        GUILayout.Label($"Type: Key or Mouse Button");

        EditorGUILayout.EndHorizontal();
    }
}