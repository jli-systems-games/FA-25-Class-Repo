using System.Collections.Generic;
using System.Reflection;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 彻底切断P3/P4与键盘的联系，清理PlayerID残留
/// 放在 Assets/Editor/ 文件夹中
/// </summary>
public class FixPlayerInputEditor : EditorWindow
{
    private GameObject player1;
    private GameObject player2;
    private GameObject player3;
    private GameObject player4;

    [MenuItem("Tools/修复玩家输入配置")]
    public static void ShowWindow()
    {
        GetWindow<FixPlayerInputEditor>("修复玩家输入");
    }

    void OnGUI()
    {
        GUILayout.Label("=== 玩家输入配置修复工具 ===", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "此工具将：\n" +
            "1. 修复P3/P4的InputManager配置（使用反射访问私有字段）\n" +
            "2. 清理所有组件上残留的PlayerID\n" +
            "3. 确保P3用手柄1，P4用手柄2\n" +
            "4. 彻底切断P3/P4与键盘的联系",
            MessageType.Info);

        GUILayout.Space(10);

        GUILayout.Label("拖入场景中的4个玩家对象：", EditorStyles.boldLabel);
        player1 = (GameObject)EditorGUILayout.ObjectField("P1 (键盘)", player1, typeof(GameObject), true);
        player2 = (GameObject)EditorGUILayout.ObjectField("P2 (键盘)", player2, typeof(GameObject), true);
        player3 = (GameObject)EditorGUILayout.ObjectField("P3 (手柄1)", player3, typeof(GameObject), true);
        player4 = (GameObject)EditorGUILayout.ObjectField("P4 (手柄2)", player4, typeof(GameObject), true);

        GUILayout.Space(20);

        if (GUILayout.Button("🔧 一键修复所有问题", GUILayout.Height(40)))
        {
            FixAllPlayers();
        }

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "修复后请检查：\n" +
            "• P3/P4的InputManager中所有轴都应该是手柄输入\n" +
            "• P3/P4身上所有组件的PlayerID应该是Player3/Player4\n" +
            "• 测试时P1/P2的键盘不应影响P3/P4",
            MessageType.Warning);
    }

    void FixAllPlayers()
    {
        if (player1 == null || player2 == null || player3 == null || player4 == null)
        {
            EditorUtility.DisplayDialog("错误", "请先拖入所有4个玩家对象！", "确定");
            return;
        }

        int fixedCount = 0;

        // 修复P1 - 键盘
        fixedCount += FixPlayer(player1, "Player1", false, 0);

        // 修复P2 - 键盘
        fixedCount += FixPlayer(player2, "Player2", false, 0);

        // 修复P3 - 手柄1
        fixedCount += FixPlayer(player3, "Player3", true, 1);

        // 修复P4 - 手柄2
        fixedCount += FixPlayer(player4, "Player4", true, 2);

        EditorUtility.DisplayDialog("完成",
            $"修复完成！\n共修复了 {fixedCount} 个组件的配置。\n\n请保存场景并测试。",
            "确定");

        Debug.Log($"✅ [输入修复] 完成！共修复 {fixedCount} 个组件");
    }

    int FixPlayer(GameObject player, string correctPlayerID, bool useGamepad, int gamepadIndex)
    {
        int fixedCount = 0;

        Debug.Log($"========== 开始修复 {player.name} (PlayerID: {correctPlayerID}) ==========");

        // 1. 修复 InputManager
        InputManager inputManager = player.GetComponent<InputManager>();
        if (inputManager != null)
        {
            fixedCount += FixInputManager(inputManager, correctPlayerID, useGamepad, gamepadIndex);
        }
        else
        {
            Debug.LogWarning($"⚠️ {player.name} 没有 InputManager 组件！");
        }

        // 2. 清理所有组件的 PlayerID
        fixedCount += CleanupAllPlayerIDs(player, correctPlayerID);

        Debug.Log($"========== {player.name} 修复完成，共修复 {fixedCount} 项 ==========\n");

        return fixedCount;
    }

    int FixInputManager(InputManager inputManager, string correctPlayerID, bool useGamepad, int gamepadIndex)
    {
        int fixedCount = 0;
        var type = typeof(InputManager);

        Debug.Log($"🎮 修复 InputManager (PlayerID: {correctPlayerID}, 手柄: {useGamepad}, 索引: {gamepadIndex})");

        // 修复 PlayerID
        var playerIDField = type.GetField("PlayerID", BindingFlags.Public | BindingFlags.Instance);
        if (playerIDField != null)
        {
            playerIDField.SetValue(inputManager, correctPlayerID);
            fixedCount++;
            Debug.Log($"  ✓ PlayerID -> {correctPlayerID}");
        }

        if (useGamepad)
        {
            // P3/P4 使用手柄
            string joystickPrefix = $"Joystick{gamepadIndex}";

            // 主要移动轴 - 清空（不使用键盘）
            SetPrivateField(inputManager, "AxisHorizontal", "");
            SetPrivateField(inputManager, "AxisVertical", "");
            fixedCount += 2;
            Debug.Log($"  ✓ 主要轴已清空（禁用键盘）");

            // 次要移动轴 - 使用手柄左摇杆
            SetPrivateField(inputManager, "AxisSecondaryHorizontal", $"{joystickPrefix}Axis1");
            SetPrivateField(inputManager, "AxisSecondaryVertical", $"{joystickPrefix}Axis2");
            fixedCount += 2;
            Debug.Log($"  ✓ 次要轴 -> {joystickPrefix}Axis1/2 (左摇杆)");

            // 射击/瞄准轴 - 使用手柄右摇杆
            SetPrivateField(inputManager, "AxisShootHorizontal", $"{joystickPrefix}Axis4");
            SetPrivateField(inputManager, "AxisShootVertical", $"{joystickPrefix}Axis5");
            fixedCount += 2;
            Debug.Log($"  ✓ 射击轴 -> {joystickPrefix}Axis4/5 (右摇杆)");

            // 相机轴 - 使用手柄右摇杆
            SetPrivateField(inputManager, "AxisCameraHorizontal", $"{joystickPrefix}Axis4");
            SetPrivateField(inputManager, "AxisCameraVertical", $"{joystickPrefix}Axis5");
            fixedCount += 2;
            Debug.Log($"  ✓ 相机轴 -> {joystickPrefix}Axis4/5");

            // 按钮配置
            SetPrivateField(inputManager, "JumpButton", $"{joystickPrefix}Button0");
            SetPrivateField(inputManager, "RunButton", $"{joystickPrefix}Button1");
            SetPrivateField(inputManager, "DashButton", $"{joystickPrefix}Button2");
            SetPrivateField(inputManager, "CrouchButton", $"{joystickPrefix}Button3");
            SetPrivateField(inputManager, "ShootButton", $"{joystickPrefix}Button5");
            SetPrivateField(inputManager, "SecondaryShootButton", $"{joystickPrefix}Button4");
            SetPrivateField(inputManager, "InteractButton", $"{joystickPrefix}Button6");
            SetPrivateField(inputManager, "ReloadButton", $"{joystickPrefix}Button7");
            fixedCount += 8;
            Debug.Log($"  ✓ 按钮配置完成 (8个按钮)");

            // 强制输入检测模式
            SetPrivateField(inputManager, "InputDetectionActive", true);
            SetPrivateField(inputManager, "IsMobile", false);
            fixedCount += 2;
        }
        else
        {
            // P1/P2 使用键盘
            SetPrivateField(inputManager, "AxisHorizontal", "Horizontal");
            SetPrivateField(inputManager, "AxisVertical", "Vertical");
            SetPrivateField(inputManager, "AxisSecondaryHorizontal", "");
            SetPrivateField(inputManager, "AxisSecondaryVertical", "");
            fixedCount += 4;
            Debug.Log($"  ✓ 配置为键盘输入");
        }

        // 标记为已修改
        EditorUtility.SetDirty(inputManager);

        return fixedCount;
    }

    int CleanupAllPlayerIDs(GameObject player, string correctPlayerID)
    {
        int fixedCount = 0;
        var allComponents = player.GetComponents<MonoBehaviour>();

        Debug.Log($"🧹 清理 PlayerID 残留 (目标: {correctPlayerID})");

        foreach (var component in allComponents)
        {
            if (component == null) continue;

            var type = component.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                // 查找所有名为 PlayerID 的字段
                if (field.Name.Contains("PlayerID") || field.Name.Contains("PlayerId") || field.Name.Contains("_playerID"))
                {
                    if (field.FieldType == typeof(string))
                    {
                        string currentValue = (string)field.GetValue(component);
                        if (currentValue != correctPlayerID)
                        {
                            field.SetValue(component, correctPlayerID);
                            fixedCount++;
                            Debug.Log($"  ✓ {type.Name}.{field.Name}: '{currentValue}' -> '{correctPlayerID}'");
                            EditorUtility.SetDirty(component);
                        }
                    }
                }

                // 特殊处理：CharacterAbility 的 InputManagerName
                if (field.Name == "InputManagerName" && field.FieldType == typeof(string))
                {
                    string currentValue = (string)field.GetValue(component);
                    if (currentValue != correctPlayerID)
                    {
                        field.SetValue(component, correctPlayerID);
                        fixedCount++;
                        Debug.Log($"  ✓ {type.Name}.InputManagerName: '{currentValue}' -> '{correctPlayerID}'");
                        EditorUtility.SetDirty(component);
                    }
                }
            }
        }

        if (fixedCount > 0)
        {
            Debug.Log($"  清理完成，共修复 {fixedCount} 个字段");
        }

        return fixedCount;
    }

    void SetPrivateField(object obj, string fieldName, object value)
    {
        var type = obj.GetType();
        var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(obj, value);
        }
        else
        {
            Debug.LogWarning($"⚠️ 未找到字段: {fieldName}");
        }
    }
}