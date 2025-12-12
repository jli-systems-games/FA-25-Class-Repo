using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 批量将 Built-in 渲染管线材质升级到 URP
/// </summary>
public class MaterialUpgradeToURP : EditorWindow
{
    [MenuItem("Tools/升级材质到 URP")]
    static void UpgradeAllMaterialsToURP()
    {
        if (!EditorUtility.DisplayDialog("升级材质到 URP", 
            "这将把项目中所有 Built-in 材质升级到 URP。\n建议先备份项目。\n\n是否继续？", 
            "升级", "取消"))
        {
            return;
        }

        // 查找所有材质
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material");
        int upgraded = 0;
        int total = materialGUIDs.Length;

        for (int i = 0; i < materialGUIDs.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(materialGUIDs[i]);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (material == null || material.shader == null)
                continue;

            EditorUtility.DisplayProgressBar("升级材质到 URP", 
                $"处理: {material.name} ({i + 1}/{total})", 
                (float)i / total);

            bool needsUpgrade = false;
            string newShaderName = "";

            // 检查并转换常见的 Built-in 着色器
            if (material.shader.name.Contains("Sprites/Default") || 
                material.shader.name == "Sprites-Default")
            {
                newShaderName = "Universal Render Pipeline/2D/Sprite-Lit-Default";
                needsUpgrade = true;
            }
            else if (material.shader.name.Contains("Sprites/Diffuse"))
            {
                newShaderName = "Universal Render Pipeline/2D/Sprite-Lit-Default";
                needsUpgrade = true;
            }
            else if (material.shader.name.Contains("UI/Default"))
            {
                newShaderName = "UI/Default";
                needsUpgrade = false; // UI 着色器通常兼容
            }
            else if (material.shader.name.Contains("Standard") || 
                     material.shader.name.Contains("Legacy Shaders"))
            {
                newShaderName = "Universal Render Pipeline/Lit";
                needsUpgrade = true;
            }
            else if (material.shader.name.Contains("Unlit"))
            {
                newShaderName = "Universal Render Pipeline/Unlit";
                needsUpgrade = true;
            }
            else if (material.shader.name == "Hidden/InternalErrorShader" || 
                     material.shader.name.Contains("Hidden/Internal"))
            {
                // 这是粉色材质，使用默认 URP 着色器
                newShaderName = "Universal Render Pipeline/Lit";
                needsUpgrade = true;
            }

            if (needsUpgrade && !string.IsNullOrEmpty(newShaderName))
            {
                Shader newShader = Shader.Find(newShaderName);
                if (newShader != null)
                {
                    Undo.RecordObject(material, "Upgrade material to URP");
                    material.shader = newShader;
                    EditorUtility.SetDirty(material);
                    upgraded++;
                    Debug.Log($"升级材质: {path} -> {newShaderName}");
                }
                else
                {
                    Debug.LogWarning($"找不到着色器: {newShaderName} (材质: {path})");
                }
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("完成", 
            $"材质升级完成！\n\n" +
            $"总计: {total} 个材质\n" +
            $"已升级: {upgraded} 个", 
            "确定");

        Debug.Log($"<color=green>材质升级完成！升级了 {upgraded}/{total} 个材质</color>");
    }

    [MenuItem("Tools/修复粉色材质（快速）")]
    static void FixPinkMaterialsQuick()
    {
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material");
        int fixed_count = 0;

        for (int i = 0; i < materialGUIDs.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(materialGUIDs[i]);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (material == null || material.shader == null)
                continue;

            // 检查是否是粉色材质（错误着色器）
            if (material.shader.name == "Hidden/InternalErrorShader" || 
                material.shader.name.Contains("Hidden/Internal"))
            {
                Shader defaultShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default");
                if (defaultShader != null)
                {
                    material.shader = defaultShader;
                    EditorUtility.SetDirty(material);
                    fixed_count++;
                    Debug.Log($"修复粉色材质: {path}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("完成", 
            $"修复了 {fixed_count} 个粉色材质", 
            "确定");
    }
}

