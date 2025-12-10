using UnityEngine;
using UnityEditor;
using MoreMountains.TopDownEngine;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 多人朝向自动修复工具
/// 修复CharacterOrientation和WeaponAim配置
/// </summary>
public class FixMultiplayerOrientation : EditorWindow
{
    [MenuItem("Tools/🎯 修复多人朝向问题")]
    public static void ShowWindow()
    {
        GetWindow<FixMultiplayerOrientation>("修复多人朝向");
    }

    private GameObject player1;
    private GameObject player2;
    private GameObject player3;
    private GameObject player4;

    private Vector2 scrollPosition;

    void OnGUI()
    {
        GUILayout.Label("=== 多人朝向自动修复 ===", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "此工具会自动修复P3/P4的朝向问题：\n" +
            "1. 设置CharacterOrientation使用Secondary Input\n" +
            "2. 设置WeaponAim使用Secondary Movement\n" +
            "3. 禁用CharacterRotateCamera（可选）",
            MessageType.Info);

        GUILayout.Space(10);

        // 拖入玩家
        player1 = EditorGUILayout.ObjectField("P1 (键盘)", player1, typeof(GameObject), true) as GameObject;
        player2 = EditorGUILayout.ObjectField("P2 (键盘)", player2, typeof(GameObject), true) as GameObject;
        player3 = EditorGUILayout.ObjectField("P3 (手柄1)", player3, typeof(GameObject), true) as GameObject;
        player4 = EditorGUILayout.ObjectField("P4 (手柄2)", player4, typeof(GameObject), true) as GameObject;

        GUILayout.Space(20);

        if (GUILayout.Button("🔧 修复所有朝向问题", GUILayout.Height(50)))
        {
            FixAllOrientationIssues();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("🔍 诊断当前配置", GUILayout.Height(40)))
        {
            DiagnoseAll();
        }

        GUILayout.Space(20);

        // 滚动区域显示信息
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.EndScrollView();
    }

    void FixAllOrientationIssues()
    {
        int fixCount = 0;

        Debug.Log("=== 开始修复多人朝向问题 ===");

        // 修复P1和P2（键盘玩家）
        if (player1 != null)
        {
            fixCount += FixKeyboardPlayer(player1, "Player1");
        }
        if (player2 != null)
        {
            fixCount += FixKeyboardPlayer(player2, "Player2");
        }

        // 修复P3和P4（手柄玩家）
        if (player3 != null)
        {
            fixCount += FixGamepadPlayer(player3, "Player3");
        }
        if (player4 != null)
        {
            fixCount += FixGamepadPlayer(player4, "Player4");
        }

        Debug.Log($"=== 修复完成！总共修复了 {fixCount} 个组件 ===");

        EditorUtility.DisplayDialog("修复完成",
            $"已修复 {fixCount} 个组件的配置！\n" +
            "请查看Console获取详细信息。",
            "确定");
    }

    int FixKeyboardPlayer(GameObject player, string playerID)
    {
        int fixes = 0;
        Debug.Log($"\n--- 修复 {player.name} (键盘玩家) ---");

        // CharacterOrientation2D
        var orientation2D = player.GetComponent<CharacterOrientation2D>();
        if (orientation2D != null)
        {
            fixes += FixCharacterOrientation2D(orientation2D, false);
        }

        // CharacterOrientation3D
        var orientation3D = player.GetComponent<CharacterOrientation3D>();
        if (orientation3D != null)
        {
            fixes += FixCharacterOrientation3D(orientation3D, false);
        }

        // Weapons
        fixes += FixPlayerWeapons(player, false);

        return fixes;
    }

    int FixGamepadPlayer(GameObject player, string playerID)
    {
        int fixes = 0;
        Debug.Log($"\n--- 修复 {player.name} (手柄玩家) ---");

        // CharacterOrientation2D
        var orientation2D = player.GetComponent<CharacterOrientation2D>();
        if (orientation2D != null)
        {
            fixes += FixCharacterOrientation2D(orientation2D, true);
        }

        // CharacterOrientation3D
        var orientation3D = player.GetComponent<CharacterOrientation3D>();
        if (orientation3D != null)
        {
            fixes += FixCharacterOrientation3D(orientation3D, true);
        }

        // Weapons
        fixes += FixPlayerWeapons(player, true);

        // CharacterRotateCamera - 禁用或修复
        var rotateCamera = player.GetComponent<CharacterRotateCamera>();
        if (rotateCamera != null)
        {
            rotateCamera.enabled = false;
            Debug.Log($"  ✓ 已禁用 CharacterRotateCamera");
            fixes++;
        }

        return fixes;
    }

    int FixCharacterOrientation2D(CharacterOrientation2D orientation, bool useSecondaryInput)
    {
        int fixes = 0;

        // 使用反射设置私有字段
        var type = orientation.GetType();

        // 设置UseSecondaryInput
        var useSecondaryField = type.GetField("UseSecondaryInput",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (useSecondaryField != null)
        {
            useSecondaryField.SetValue(orientation, useSecondaryInput);
            Debug.Log($"  ✓ CharacterOrientation2D.UseSecondaryInput = {useSecondaryInput}");
            fixes++;
        }

        // 检查RotationMode（使用反射，因为可能是私有枚举）
        try
        {
            var rotationModeField = type.GetField("RotationMode",
                BindingFlags.Public | BindingFlags.Instance);
            if (rotationModeField != null)
            {
                var rotationMode = rotationModeField.GetValue(orientation);
                if (rotationMode != null && rotationMode.ToString().Contains("Mouse"))
                {
                    Debug.LogWarning($"  ⚠️ CharacterOrientation2D使用Mouse模式，请手动改为Movement或Weapon模式！");
                }
            }
        }
        catch { }

        EditorUtility.SetDirty(orientation);
        return fixes;
    }

    int FixCharacterOrientation3D(CharacterOrientation3D orientation, bool useSecondaryInput)
    {
        int fixes = 0;

        // 使用反射设置私有字段
        var type = orientation.GetType();

        // 设置UseSecondaryInput
        var useSecondaryField = type.GetField("UseSecondaryInput",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (useSecondaryField != null)
        {
            useSecondaryField.SetValue(orientation, useSecondaryInput);
            Debug.Log($"  ✓ CharacterOrientation3D.UseSecondaryInput = {useSecondaryInput}");
            fixes++;
        }

        // 检查RotationMode（使用反射）
        try
        {
            var rotationModeField = type.GetField("RotationMode",
                BindingFlags.Public | BindingFlags.Instance);
            if (rotationModeField != null)
            {
                var rotationMode = rotationModeField.GetValue(orientation);
                if (rotationMode != null && rotationMode.ToString().Contains("Mouse"))
                {
                    Debug.LogWarning($"  ⚠️ CharacterOrientation3D使用Mouse模式，请手动改为Movement或Weapon模式！");
                }
            }
        }
        catch { }

        EditorUtility.SetDirty(orientation);
        return fixes;
    }

    int FixPlayerWeapons(GameObject player, bool useSecondaryMovement)
    {
        int fixes = 0;

        // 查找所有武器
        var weapons = player.GetComponentsInChildren<Weapon>(true);

        foreach (var weapon in weapons)
        {
            // WeaponAim2D
            var weaponAim2D = weapon.GetComponent<WeaponAim2D>();
            if (weaponAim2D != null)
            {
                fixes += FixWeaponAim2D(weaponAim2D, useSecondaryMovement);
            }

            // WeaponAim3D
            var weaponAim3D = weapon.GetComponent<WeaponAim3D>();
            if (weaponAim3D != null)
            {
                fixes += FixWeaponAim3D(weaponAim3D, useSecondaryMovement);
            }
        }

        return fixes;
    }

    int FixWeaponAim2D(WeaponAim2D weaponAim, bool useSecondaryMovement)
    {
        // 设置AimControl
        if (useSecondaryMovement)
        {
            weaponAim.AimControl = WeaponAim.AimControls.SecondaryMovement;
            Debug.Log($"  ✓ {weaponAim.gameObject.name}: WeaponAim2D.AimControl = SecondaryMovement");
        }
        else
        {
            // 键盘玩家可以用Mouse或PrimaryMovement
            if (weaponAim.AimControl == WeaponAim.AimControls.SecondaryMovement)
            {
                weaponAim.AimControl = WeaponAim.AimControls.PrimaryMovement;
                Debug.Log($"  ✓ {weaponAim.gameObject.name}: WeaponAim2D.AimControl = PrimaryMovement");
            }
        }

        EditorUtility.SetDirty(weaponAim);
        return 1;
    }

    int FixWeaponAim3D(WeaponAim3D weaponAim, bool useSecondaryMovement)
    {
        // 设置AimControl
        if (useSecondaryMovement)
        {
            weaponAim.AimControl = WeaponAim.AimControls.SecondaryMovement;
            Debug.Log($"  ✓ {weaponAim.gameObject.name}: WeaponAim3D.AimControl = SecondaryMovement");
        }
        else
        {
            // 键盘玩家可以用Mouse或PrimaryMovement
            if (weaponAim.AimControl == WeaponAim.AimControls.SecondaryMovement)
            {
                weaponAim.AimControl = WeaponAim.AimControls.PrimaryMovement;
                Debug.Log($"  ✓ {weaponAim.gameObject.name}: WeaponAim3D.AimControl = PrimaryMovement");
            }
        }

        EditorUtility.SetDirty(weaponAim);
        return 1;
    }

    void DiagnoseAll()
    {
        Debug.Log("\n=== 多人配置诊断 ===\n");

        if (player1 != null) DiagnosePlayer(player1, "Player1 (键盘)");
        if (player2 != null) DiagnosePlayer(player2, "Player2 (键盘)");
        if (player3 != null) DiagnosePlayer(player3, "Player3 (手柄)");
        if (player4 != null) DiagnosePlayer(player4, "Player4 (手柄)");

        Debug.Log("\n=== 诊断完成 ===");

        EditorUtility.DisplayDialog("诊断完成",
            "诊断结果已输出到Console。\n请查看详细信息。",
            "确定");
    }

    void DiagnosePlayer(GameObject player, string label)
    {
        Debug.Log($"\n--- {label}: {player.name} ---");

        // InputManager
        var inputManager = player.GetComponent<InputManager>();
        if (inputManager != null)
        {
            Debug.Log($"  InputManager.PlayerID: {inputManager.PlayerID}");
        }

        // CharacterOrientation2D
        var orientation2D = player.GetComponent<CharacterOrientation2D>();
        if (orientation2D != null)
        {
            Debug.Log($"  CharacterOrientation2D:");

            // 使用反射读取RotationMode
            try
            {
                var rotationModeField = orientation2D.GetType().GetField("RotationMode",
                    BindingFlags.Public | BindingFlags.Instance);
                if (rotationModeField != null)
                {
                    var rotationMode = rotationModeField.GetValue(orientation2D);
                    Debug.Log($"    Rotation Mode: {rotationMode}");
                }
            }
            catch { }

            var useSecondaryField = orientation2D.GetType().GetField("UseSecondaryInput",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (useSecondaryField != null)
            {
                bool useSecondary = (bool)useSecondaryField.GetValue(orientation2D);
                Debug.Log($"    Use Secondary Input: {useSecondary}");
            }
        }

        // CharacterOrientation3D
        var orientation3D = player.GetComponent<CharacterOrientation3D>();
        if (orientation3D != null)
        {
            Debug.Log($"  CharacterOrientation3D:");

            // 使用反射读取RotationMode
            try
            {
                var rotationModeField = orientation3D.GetType().GetField("RotationMode",
                    BindingFlags.Public | BindingFlags.Instance);
                if (rotationModeField != null)
                {
                    var rotationMode = rotationModeField.GetValue(orientation3D);
                    Debug.Log($"    Rotation Mode: {rotationMode}");
                }
            }
            catch { }

            var useSecondaryField = orientation3D.GetType().GetField("UseSecondaryInput",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (useSecondaryField != null)
            {
                bool useSecondary = (bool)useSecondaryField.GetValue(orientation3D);
                Debug.Log($"    Use Secondary Input: {useSecondary}");
            }
        }

        // Weapons
        var weapons = player.GetComponentsInChildren<Weapon>(true);
        if (weapons.Length > 0)
        {
            Debug.Log($"  武器 ({weapons.Length}个):");
            foreach (var weapon in weapons)
            {
                var weaponAim = weapon.GetComponent<WeaponAim>();
                if (weaponAim != null)
                {
                    Debug.Log($"    {weapon.name}: Aim Control = {weaponAim.AimControl}");
                }
            }
        }

        // CharacterRotateCamera
        var rotateCamera = player.GetComponent<CharacterRotateCamera>();
        if (rotateCamera != null)
        {
            Debug.Log($"  CharacterRotateCamera: {(rotateCamera.enabled ? "启用" : "禁用")}");
        }
    }
}