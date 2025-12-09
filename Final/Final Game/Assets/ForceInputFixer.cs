#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using MoreMountains.TopDownEngine;

public class ForceFillInput : EditorWindow
{
    [MenuItem("Tools/⚡ 强制填充 P3P4 输入 (Force Fill)")]
    public static void ShowWindow()
    {
        GetWindow<ForceFillInput>("Force Fill");
    }

    void OnGUI()
    {
        GUILayout.Label("选中 Project 里的 P3 或 P4 预制体，然后点按钮", EditorStyles.boldLabel);
        GUILayout.Label("此工具会无视属性是否私有，强制写入正确的值。", EditorStyles.miniLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("👉 填充 P3 (手柄1)"))
        {
            FillInput("Player3", "Player3_Horizontal", "Player3_Vertical");
        }

        if (GUILayout.Button("👉 填充 P4 (手柄2)"))
        {
            FillInput("Player4", "Player4_Horizontal", "Player4_Vertical");
        }
    }

    void FillInput(string pid, string h, string v)
    {
        // 获取选中的对象
        Object[] selection = Selection.objects;

        if (selection.Length == 0)
        {
            Debug.LogError("❌ 未选中任何文件！请先在 Project 窗口选中预制体。");
            return;
        }

        foreach (Object obj in selection)
        {
            GameObject prefab = obj as GameObject;
            if (prefab == null) continue;

            InputManager im = prefab.GetComponent<InputManager>();
            if (im == null)
            {
                Debug.LogWarning($"⚠️ {prefab.name} 没有 InputManager 组件");
                continue;
            }

            // === 核心：使用 FindProperty 强制写入，不依赖代码 API ===
            SerializedObject so = new SerializedObject(im);

            // 1. 强制写入 PlayerID
            SetProp(so, "PlayerID", pid);

            // 2. 强制写入 移动轴
            SetProp(so, "AxisHorizontal", h);
            SetProp(so, "AxisVertical", v);

            // 3. 【关键】强制写入 瞄准轴 (填入和移动轴一样的值，防止干扰)
            SetProp(so, "AxisSecondaryHorizontal", h);
            SetProp(so, "AxisSecondaryVertical", v);

            // 4. 强制写入 按钮 (防止 Pause 报错)
            SetProp(so, "AxisShoot", pid + "_Interact");
            SetProp(so, "AxisJump", pid + "_Jump");

            // 5. 禁用鼠标
            SerializedProperty mouseProp = so.FindProperty("MouseInput");
            if (mouseProp != null) mouseProp.boolValue = false;

            // 保存修改
            so.ApplyModifiedProperties();

            // 标记脏数据并保存
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();

            Debug.Log($"<color=green>✅ 成功强制填充: {prefab.name} | 瞄准轴已锁定为 {h}</color>");
        }
    }

    void SetProp(SerializedObject so, string propName, string value)
    {
        SerializedProperty sp = so.FindProperty(propName);
        if (sp != null)
        {
            sp.stringValue = value;
        }
        else
        {
            // 如果没找到属性，可能名字不对，但这通常不会发生
            Debug.LogWarning($"⚠️ 未找到属性: {propName} (可能版本差异，但核心轴通常没变)");
        }
    }
}
#endif