#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class InputSetupTools : EditorWindow
{
    [MenuItem("Tools/【最终修复】配置P1P2键盘_P3P4手柄")]
    public static void SetupInputs()
    {
        SerializedObject inputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axes = inputManager.FindProperty("m_Axes");

        // =========================================================
        // P3 配置 (使用手柄 1)
        // =========================================================
        AddAxis(axes, "Player3_Horizontal", 1, 0, AxisType.Axis); // 左摇杆 X
        AddAxis(axes, "Player3_Vertical", 1, 1, AxisType.Axis);   // 左摇杆 Y
        AddAxis(axes, "Player3_Pause", 1, 7, AxisType.Button);    // Start键 (通常是Button 7)
        AddAxis(axes, "Player3_Interact", 1, 0, AxisType.Button); // A键 (Button 0)
        AddAxis(axes, "Player3_Jump", 1, 0, AxisType.Button);     // 防报错，设为A键
        AddAxis(axes, "Player3_Run", 1, 2, AxisType.Button);      // 防报错，设为X键

        // =========================================================
        // P4 配置 (使用手柄 2)
        // =========================================================
        AddAxis(axes, "Player4_Horizontal", 2, 0, AxisType.Axis); // 左摇杆 X
        AddAxis(axes, "Player4_Vertical", 2, 1, AxisType.Axis);   // 左摇杆 Y
        AddAxis(axes, "Player4_Pause", 2, 7, AxisType.Button);    // Start键
        AddAxis(axes, "Player4_Interact", 2, 0, AxisType.Button); // A键
        AddAxis(axes, "Player4_Jump", 2, 0, AxisType.Button);     // 防报错
        AddAxis(axes, "Player4_Run", 2, 2, AxisType.Button);      // 防报错

        inputManager.ApplyModifiedProperties();
        Debug.Log("<color=green>✔ 完美配置完成！已修复 Pause/Jump 报错。P3->手柄1，P4->手柄2。</color>");
    }

    enum AxisType { Axis, Button }

    private static void AddAxis(SerializedProperty axes, string name, int joyNum, int axisOrBtnNum, AxisType type)
    {
        // 检查是否存在
        SerializedProperty targetAxis = null;
        for (int i = 0; i < axes.arraySize; i++)
        {
            SerializedProperty axis = axes.GetArrayElementAtIndex(i);
            if (axis.FindPropertyRelative("m_Name").stringValue == name)
            {
                targetAxis = axis;
                break;
            }
        }

        if (targetAxis == null)
        {
            axes.InsertArrayElementAtIndex(axes.arraySize);
            targetAxis = axes.GetArrayElementAtIndex(axes.arraySize - 1);
        }

        // 通用设置
        targetAxis.FindPropertyRelative("m_Name").stringValue = name;
        targetAxis.FindPropertyRelative("gravity").floatValue = 0;
        targetAxis.FindPropertyRelative("dead").floatValue = 0.19f;
        targetAxis.FindPropertyRelative("sensitivity").floatValue = 1;
        targetAxis.FindPropertyRelative("snap").boolValue = false;
        
        targetAxis.FindPropertyRelative("type").intValue = 2; // Joystick Axis
        targetAxis.FindPropertyRelative("joyNum").intValue = joyNum; // 手柄编号

        if (type == AxisType.Axis)
        {
            // 摇杆方向设置
            targetAxis.FindPropertyRelative("axis").intValue = axisOrBtnNum;
            targetAxis.FindPropertyRelative("invert").boolValue = (axisOrBtnNum == 1); // Y轴反转
        }
        else
        {
            // 按钮设置 (虽然Type是Joystick Axis，但Unity把按钮映射为KeyOrMouseButton)
            // 修正：对于Unity Input Manager，手柄按钮其实是 key
            targetAxis.FindPropertyRelative("type").intValue = 0; // Key or Mouse Button
            string btnName = "joystick " + joyNum + " button " + axisOrBtnNum;
            targetAxis.FindPropertyRelative("positiveButton").stringValue = btnName;
            targetAxis.FindPropertyRelative("altPositiveButton").stringValue = "";
            targetAxis.FindPropertyRelative("negativeButton").stringValue = "";
            targetAxis.FindPropertyRelative("axis").intValue = 0; // 不使用轴
            targetAxis.FindPropertyRelative("invert").boolValue = false;
        }
    }
}
#endif