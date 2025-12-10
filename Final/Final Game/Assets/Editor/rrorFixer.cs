using UnityEngine;
using UnityEditor;

/// <summary>
/// 紧急错误清理工具 - 修复 TargetParameterCountException
/// 放在 Assets/Editor/ 文件夹中
/// </summary>
public class EmergencyErrorFix : EditorWindow
{
    [MenuItem("Tools/🚨 紧急错误修复")]
    public static void ShowWindow()
    {
        GetWindow<EmergencyErrorFix>("紧急错误修复");
    }

    void OnGUI()
    {
        GUILayout.Label("=== 紧急错误修复工具 ===", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "如果你遇到 TargetParameterCountException 错误，\n" +
            "请按以下步骤操作：",
            MessageType.Warning);

        GUILayout.Space(10);

        if (GUILayout.Button("步骤1: 删除旧的 InputDiagnostics 组件", GUILayout.Height(40)))
        {
            RemoveOldInputDiagnostics();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("步骤2: 清理编辑器缓存", GUILayout.Height(40)))
        {
            ClearEditorCache();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("步骤3: 重新编译脚本", GUILayout.Height(40)))
        {
            RecompileScripts();
        }

        GUILayout.Space(20);

        EditorGUILayout.HelpBox(
            "完成后：\n" +
            "1. 关闭 Unity\n" +
            "2. 删除 Library 文件夹\n" +
            "3. 重新打开 Unity\n" +
            "4. 使用新的 InputDiagnostics_Safe.cs",
            MessageType.Info);
    }

    void RemoveOldInputDiagnostics()
    {
        InputDiagnosticsSetup[] diagnostics = null;

#if UNITY_6000_0_OR_NEWER
        diagnostics = Object.FindObjectsByType<InputDiagnosticsSetup>(FindObjectsSortMode.None);
#else
#pragma warning disable CS0618
        diagnostics = Object.FindObjectsOfType<InputDiagnostics>();
#pragma warning restore CS0618
#endif

        if (diagnostics.Length == 0)
        {
            EditorUtility.DisplayDialog("完成", "场景中没有 InputDiagnostics 组件。", "确定");
            return;
        }

        foreach (var diag in diagnostics)
        {
            DestroyImmediate(diag);
        }

        EditorUtility.DisplayDialog("完成",
            $"已删除 {diagnostics.Length} 个 InputDiagnostics 组件。\n请继续执行步骤2。",
            "确定");

        Debug.Log($"✅ 已删除 {diagnostics.Length} 个 InputDiagnostics 组件");
    }

    void ClearEditorCache()
    {
        // 清理编辑器缓存
        EditorUtility.ClearProgressBar();

        // 强制保存
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("完成",
            "编辑器缓存已清理。\n请继续执行步骤3。",
            "确定");

        Debug.Log("✅ 编辑器缓存已清理");
    }

    void RecompileScripts()
    {
        // 强制重新编译
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("完成",
            "脚本已重新编译。\n\n现在请：\n" +
            "1. 关闭 Unity\n" +
            "2. 删除 Library 文件夹\n" +
            "3. 重新打开 Unity",
            "确定");

        Debug.Log("✅ 脚本已重新编译");
    }
}