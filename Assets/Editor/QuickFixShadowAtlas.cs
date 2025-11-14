using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 一键修复阴影图集问题
/// </summary>
public class QuickFixShadowAtlas
{
    [MenuItem("工具/一键修复阴影问题 (增加图集到4096) &F")]
    public static void QuickFix()
    {
        // 查找所有URP资源
        string[] guids = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset");
        
        if (guids.Length == 0)
        {
            Debug.LogError("❌ 未找到任何URP资源文件！请确保项目使用了Universal Render Pipeline。");
            return;
        }
        
        int successCount = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UniversalRenderPipelineAsset urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            
            if (urpAsset != null)
            {
                SerializedObject so = new SerializedObject(urpAsset);
                
                // 修改Additional Lights Shadow Atlas Resolution
                SerializedProperty shadowAtlasProp = so.FindProperty("m_AdditionalLightsShadowmapResolution");
                
                if (shadowAtlasProp != null)
                {
                    string oldSize = GetSizeString(shadowAtlasProp.intValue);
                    shadowAtlasProp.intValue = 4; // 4 = 4096x4096
                    so.ApplyModifiedProperties();
                    
                    Debug.Log($"✅ 已修改 URP资源: {path}\n" +
                             $"   阴影图集: {oldSize} → 4096x4096");
                    successCount++;
                }
                
                EditorUtility.SetDirty(urpAsset);
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        if (successCount > 0)
        {
            Debug.Log($"<color=green>✅✅✅ 修复完成！已更新 {successCount} 个URP资源文件。</color>\n" +
                     $"<color=yellow>请重新运行场景，阴影警告应该消失了。</color>");
            
            EditorUtility.DisplayDialog(
                "修复成功！",
                $"已将 {successCount} 个URP资源的阴影图集大小增加到 4096x4096\n\n" +
                "请重新运行场景，问题应该已解决！",
                "好的"
            );
        }
        else
        {
            Debug.LogWarning("⚠️ 未能修改任何URP资源。");
        }
    }
    
    private static string GetSizeString(int enumValue)
    {
        return enumValue switch
        {
            0 => "256x256",
            1 => "512x512",
            2 => "1024x1024",
            3 => "2048x2048",
            4 => "4096x4096",
            5 => "8192x8192",
            _ => "未知"
        };
    }
}

