#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using MoreMountains.TopDownEngine;

public class FixGhostInput : EditorWindow
{
    [MenuItem("Tools/💀 修复 Assets 里的预制体 (Fix Prefabs)")]
    public static void ShowWindow()
    {
        GetWindow<FixGhostInput>("Fix Prefabs");
    }

    void OnGUI()
    {
        GUILayout.Label("请在 Project 窗口选中 P3 或 P4 的预制体文件", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Label("选好文件后，点击下方对应的按钮进行修复", EditorStyles.miniLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("👉 修复选中文件 -> 绑定为 P3 (手柄1)"))
        {
            FixSelectedPrefab("Player3", "Player3_Horizontal", "Player3_Vertical");
        }

        if (GUILayout.Button("👉 修复选中文件 -> 绑定为 P4 (手柄2)"))
        {
            FixSelectedPrefab("Player4", "Player4_Horizontal", "Player4_Vertical");
        }
    }

    void FixSelectedPrefab(string playerID, string hAxis, string vAxis)
    {
        // 获取当前鼠标选中的所有文件
        Object[] selection = Selection.objects;

        if (selection.Length == 0)
        {
            Debug.LogError("❌ 你没有选中任何文件！请在 Project 窗口里选中 P3 或 P4 的 Prefab。");
            return;
        }

        foreach (Object obj in selection)
        {
            GameObject prefab = obj as GameObject;
            if (prefab == null) continue; // 如果选的不是游戏物体跳过

            // 获取 InputManager 组件
            InputManager im = prefab.GetComponent<InputManager>();
            if (im == null)
            {
                Debug.LogWarning($"⚠️ 文件 {prefab.name} 上没有 InputManager 组件。");
                continue;
            }

            // === 核心修复逻辑 (无视隐藏属性) ===
            SerializedObject so = new SerializedObject(im);
            SerializedProperty prop = so.GetIterator();
            bool enterChildren = true;
            int fixCount = 0;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                // 扫描所有字符串属性
                if (prop.propertyType == SerializedPropertyType.String)
                {
                    // 如果发现值是 P2 的键盘输入 ("Horizontal" 或 "Mouse X")
                    if (prop.stringValue == "Horizontal" || prop.stringValue == "Mouse X")
                    {
                        prop.stringValue = hAxis; // 强制改成 P3/P4 的手柄
                        fixCount++;
                    }
                    else if (prop.stringValue == "Vertical" || prop.stringValue == "Mouse Y")
                    {
                        prop.stringValue = vAxis; // 强制改成 P3/P4 的手柄
                        fixCount++;
                    }
                }
            }

            // 顺便修复 PlayerID 和 鼠标干扰
            SerializedProperty pidProp = so.FindProperty("PlayerID");
            if (pidProp != null) pidProp.stringValue = playerID;

            SerializedProperty mouseProp = so.FindProperty("MouseInput");
            if (mouseProp != null) mouseProp.boolValue = false;

            // 顺便修复 Pause 报错需要的键
            SerializedProperty shootProp = so.FindProperty("AxisShoot");
            if (shootProp != null) shootProp.stringValue = playerID + "_Interact";

            SerializedProperty jumpProp = so.FindProperty("AxisJump");
            if (jumpProp != null) jumpProp.stringValue = playerID + "_Jump";

            // 保存修改到 Assets 文件
            so.ApplyModifiedProperties();

            // 标记文件已脏，强制保存
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();

            Debug.Log($"<color=green>✔ 成功修复文件: {prefab.name} | 已绑定为 {playerID} | 修正了 {fixCount} 个干扰项</color>");
        }
    }
}
#endif