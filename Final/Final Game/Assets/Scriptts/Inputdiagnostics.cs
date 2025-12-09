using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

/// <summary>
/// 运行时输入诊断工具 - 显示所有玩家的输入状态
/// 挂在场景中的任意GameObject上，或创建一个空对象挂载
/// </summary>
public class InputDiagnostics : MonoBehaviour
{
    [Header("配置")]
    public bool ShowDiagnostics = true;
    public KeyCode ToggleKey = KeyCode.F1;

    [Header("玩家列表")]
    public GameObject[] players = new GameObject[4];

    private GUIStyle _boxStyle;
    private GUIStyle _labelStyle;
    private GUIStyle _headerStyle;
    private bool _stylesInitialized = false;

    void Update()
    {
        if (Input.GetKeyDown(ToggleKey))
        {
            ShowDiagnostics = !ShowDiagnostics;
        }
    }

    void OnGUI()
    {
        if (!ShowDiagnostics) return;

        InitializeStyles();

        // 主容器
        GUILayout.BeginArea(new Rect(10, 10, 800, Screen.height - 20));
        GUILayout.BeginVertical(_boxStyle);

        GUILayout.Label("=== 输入诊断工具 ===", _headerStyle);
        GUILayout.Label($"按 {ToggleKey} 开关显示", _labelStyle);
        GUILayout.Space(10);

        // 显示所有玩家的输入状态
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                ShowPlayerDiagnostics(players[i], i + 1);
                GUILayout.Space(10);
            }
        }

        // 显示原始Unity输入
        ShowRawInputs();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void InitializeStyles()
    {
        if (_stylesInitialized) return;

        _boxStyle = new GUIStyle(GUI.skin.box);
        _boxStyle.padding = new RectOffset(10, 10, 10, 10);

        _labelStyle = new GUIStyle(GUI.skin.label);
        _labelStyle.fontSize = 12;

        _headerStyle = new GUIStyle(GUI.skin.label);
        _headerStyle.fontSize = 16;
        _headerStyle.fontStyle = FontStyle.Bold;

        _stylesInitialized = true;
    }

    void ShowPlayerDiagnostics(GameObject player, int playerNum)
    {
        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label($"玩家 {playerNum}: {player.name}", _headerStyle);

        InputManager inputManager = player.GetComponent<InputManager>();
        if (inputManager == null)
        {
            GUILayout.Label("❌ 没有 InputManager 组件", _labelStyle);
            GUILayout.EndVertical();
            return;
        }

        // 使用反射读取私有字段
        var type = typeof(InputManager);

        string playerID = GetFieldValue(inputManager, "PlayerID", type);
        string axisH = GetFieldValue(inputManager, "AxisHorizontal", type);
        string axisV = GetFieldValue(inputManager, "AxisVertical", type);
        string axisSecH = GetFieldValue(inputManager, "AxisSecondaryHorizontal", type);
        string axisSecV = GetFieldValue(inputManager, "AxisSecondaryVertical", type);
        string axisShootH = GetFieldValue(inputManager, "AxisShootHorizontal", type);
        string axisShootV = GetFieldValue(inputManager, "AxisShootVertical", type);

        GUILayout.Label($"PlayerID: {playerID}", _labelStyle);
        GUILayout.Space(5);

        // 主要移动轴
        GUILayout.Label("主要移动轴:", _labelStyle);
        GUILayout.Label($"  Horizontal: '{axisH}' {(string.IsNullOrEmpty(axisH) ? "✓ (已禁用)" : "⚠️ (应为空)")}", _labelStyle);
        GUILayout.Label($"  Vertical: '{axisV}' {(string.IsNullOrEmpty(axisV) ? "✓ (已禁用)" : "⚠️ (应为空)")}", _labelStyle);

        GUILayout.Space(5);

        // 次要移动轴
        GUILayout.Label("次要移动轴 (手柄):", _labelStyle);
        GUILayout.Label($"  Secondary H: '{axisSecH}'", _labelStyle);
        GUILayout.Label($"  Secondary V: '{axisSecV}'", _labelStyle);

        // 显示实时输入值
        if (!string.IsNullOrEmpty(axisSecH))
        {
            float secH = Input.GetAxis(axisSecH);
            GUILayout.Label($"    实时值: {secH:F2}", _labelStyle);
        }
        if (!string.IsNullOrEmpty(axisSecV))
        {
            float secV = Input.GetAxis(axisSecV);
            GUILayout.Label($"    实时值: {secV:F2}", _labelStyle);
        }

        GUILayout.Space(5);

        // 射击轴
        GUILayout.Label("射击/瞄准轴:", _labelStyle);
        GUILayout.Label($"  Shoot H: '{axisShootH}'", _labelStyle);
        GUILayout.Label($"  Shoot V: '{axisShootV}'", _labelStyle);

        GUILayout.EndVertical();
    }

    void ShowRawInputs()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("=== 原始 Unity 输入 ===", _headerStyle);

        // 手柄1
        GUILayout.Label("手柄1:", _labelStyle);
        ShowJoystickInput(1);

        GUILayout.Space(5);

        // 手柄2
        GUILayout.Label("手柄2:", _labelStyle);
        ShowJoystickInput(2);

        GUILayout.Space(5);

        // 键盘
        GUILayout.Label("键盘:", _labelStyle);
        GUILayout.Label($"  Horizontal: {Input.GetAxis("Horizontal"):F2}", _labelStyle);
        GUILayout.Label($"  Vertical: {Input.GetAxis("Vertical"):F2}", _labelStyle);

        GUILayout.EndVertical();
    }

    void ShowJoystickInput(int joystickNum)
    {
        string prefix = $"Joystick{joystickNum}";

        try
        {
            float axis1 = Input.GetAxis($"{prefix}Axis1");
            float axis2 = Input.GetAxis($"{prefix}Axis2");
            float axis4 = Input.GetAxis($"{prefix}Axis4");
            float axis5 = Input.GetAxis($"{prefix}Axis5");

            GUILayout.Label($"  左摇杆: ({axis1:F2}, {axis2:F2})", _labelStyle);
            GUILayout.Label($"  右摇杆: ({axis4:F2}, {axis5:F2})", _labelStyle);

            // 显示按钮状态
            List<int> pressedButtons = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                if (Input.GetButton($"{prefix}Button{i}"))
                {
                    pressedButtons.Add(i);
                }
            }
            if (pressedButtons.Count > 0)
            {
                GUILayout.Label($"  按下按钮: {string.Join(", ", pressedButtons)}", _labelStyle);
            }
        }
        catch
        {
            GUILayout.Label($"  ⚠️ 输入轴未配置", _labelStyle);
        }
    }

    string GetFieldValue(object obj, string fieldName, System.Type type)
    {
        var field = type.GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            var value = field.GetValue(obj);
            return value != null ? value.ToString() : "null";
        }
        return "未找到字段";
    }

    // 在Editor模式下手动拖入玩家对象，不要使用自动查找
    // 自动查找可能导致反射错误
    /*
    void OnValidate()
    {
        // 如果需要自动查找，请在编辑器模式下手动运行
        // Tools -> 查找玩家对象
    }
    */
}