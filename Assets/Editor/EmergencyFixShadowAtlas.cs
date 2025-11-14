using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using System.Reflection;

/// <summary>
/// 紧急修复阴影图集 - 使用反射确保正确设置
/// </summary>
public class EmergencyFixShadowAtlas
{
    [MenuItem("工具/🚨紧急修复阴影图集🚨")]
    public static void EmergencyFix()
    {
        Debug.Log("========== 开始紧急修复 ==========");
        
        // 查找所有URP资源
        string[] guids = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset");
        
        if (guids.Length == 0)
        {
            Debug.LogError("❌ 未找到任何URP资源！");
            return;
        }
        
        int fixedCount = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UniversalRenderPipelineAsset urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            
            if (urpAsset != null)
            {
                Debug.Log($"\n正在检查: {path}");
                
                SerializedObject so = new SerializedObject(urpAsset);
                
                // 打印所有属性以便调试
                SerializedProperty prop = so.GetIterator();
                Debug.Log("--- 可用属性列表 ---");
                while (prop.NextVisible(true))
                {
                    if (prop.name.ToLower().Contains("shadow"))
                    {
                        Debug.Log($"  {prop.name} = {prop.intValue} (type: {prop.propertyType})");
                    }
                }
                
                // 尝试多个可能的属性名
                string[] possibleProps = new string[]
                {
                    "m_AdditionalLightsShadowmapResolution",
                    "m_AdditionalLightsShadowResolution", 
                    "m_ShadowAtlasResolution",
                    "m_AdditionalLightsShadowAtlasResolution"
                };
                
                bool success = false;
                
                foreach (string propName in possibleProps)
                {
                    SerializedProperty shadowProp = so.FindProperty(propName);
                    if (shadowProp != null)
                    {
                        int oldValue = shadowProp.intValue;
                        shadowProp.intValue = 4; // ShadowResolution.4096
                        
                        Debug.Log($"✅ 找到属性 '{propName}': {oldValue} → 4 (4096x4096)");
                        success = true;
                    }
                }
                
                // 使用反射直接修改
                try
                {
                    FieldInfo field = typeof(UniversalRenderPipelineAsset).GetField(
                        "m_AdditionalLightsShadowmapResolution", 
                        BindingFlags.NonPublic | BindingFlags.Instance
                    );
                    
                    if (field != null)
                    {
                        object oldValue = field.GetValue(urpAsset);
                        field.SetValue(urpAsset, 4); // 4 = 4096
                        Debug.Log($"✅ 通过反射设置: {oldValue} → 4");
                        success = true;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"反射失败: {e.Message}");
                }
                
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(urpAsset);
                
                if (success)
                {
                    fixedCount++;
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"\n========== 修复完成: {fixedCount} 个文件 ==========");
        
        if (fixedCount > 0)
        {
            EditorUtility.DisplayDialog(
                "修复完成",
                $"已修复 {fixedCount} 个URP资源\n\n请检查Console查看详细信息，然后重新运行场景。",
                "确定"
            );
        }
    }
    
    [MenuItem("工具/显示URP资源详细信息")]
    public static void ShowURPDetails()
    {
        string[] guids = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset");
        
        Debug.Log("========== URP 资源详细信息 ==========");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UniversalRenderPipelineAsset urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            
            if (urpAsset != null)
            {
                Debug.Log($"\n📁 文件: {path}");
                
                // 使用反射读取所有shadow相关字段
                FieldInfo[] fields = typeof(UniversalRenderPipelineAsset).GetFields(
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                );
                
                foreach (FieldInfo field in fields)
                {
                    if (field.Name.ToLower().Contains("shadow"))
                    {
                        object value = field.GetValue(urpAsset);
                        Debug.Log($"  {field.Name} = {value} ({field.FieldType.Name})");
                    }
                }
            }
        }
        
        Debug.Log("\n========================================");
    }
}

