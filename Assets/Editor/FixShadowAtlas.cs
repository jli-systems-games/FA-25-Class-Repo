using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using System.IO;

/// <summary>
/// 修复Unity URP阴影图集分辨率不足的问题
/// </summary>
public class FixShadowAtlas : EditorWindow
{
    private int newAtlasSize = 4096;
    private bool showLights = false;
    
    [MenuItem("工具/修复阴影图集问题")]
    public static void ShowWindow()
    {
        GetWindow<FixShadowAtlas>("修复阴影图集");
    }
    
    void OnGUI()
    {
        GUILayout.Label("URP阴影图集修复工具", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "当前问题：6个阴影贴图无法以完整分辨率适配2048x2048的阴影图集。\n" +
            "解决方案：自动增加阴影图集大小或减少投影光源。",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        // 方案1：增加图集大小
        GUILayout.Label("方案1：增加阴影图集大小", EditorStyles.boldLabel);
        newAtlasSize = EditorGUILayout.IntPopup("新的图集大小", newAtlasSize, 
            new string[] { "2048", "4096", "8192" },
            new int[] { 2048, 4096, 8192 });
        
        if (GUILayout.Button("自动修改所有URP资源的阴影图集大小", GUILayout.Height(40)))
        {
            IncreaseAllShadowAtlas();
        }
        
        GUILayout.Space(20);
        
        // 方案2：减少光源
        GUILayout.Label("方案2：管理场景光源", EditorStyles.boldLabel);
        
        if (GUILayout.Button("显示所有投射阴影的光源", GUILayout.Height(30)))
        {
            showLights = true;
            ListAllShadowCastingLights();
        }
        
        if (GUILayout.Button("禁用非主要光源的阴影", GUILayout.Height(30)))
        {
            DisableNonEssentialShadows();
        }
        
        GUILayout.Space(20);
        
        // 方案3：优化设置
        GUILayout.Label("方案3：优化阴影设置", EditorStyles.boldLabel);
        
        if (GUILayout.Button("降低所有光源的阴影分辨率", GUILayout.Height(30)))
        {
            ReduceLightShadowResolution();
        }
    }
    
    /// <summary>
    /// 增加所有URP资源的阴影图集大小
    /// </summary>
    void IncreaseAllShadowAtlas()
    {
        // 查找所有URP资源
        string[] guids = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("错误", "未找到任何UniversalRenderPipelineAsset资源！", "确定");
            return;
        }
        
        int modifiedCount = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UniversalRenderPipelineAsset urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            
            if (urpAsset != null)
            {
                // 使用SerializedObject来修改私有字段
                SerializedObject serializedObject = new SerializedObject(urpAsset);
                
                // 查找阴影图集大小属性
                SerializedProperty additionalLightsShadowmapResolution = 
                    serializedObject.FindProperty("m_AdditionalLightsShadowmapResolution");
                
                if (additionalLightsShadowmapResolution != null)
                {
                    int currentValue = additionalLightsShadowmapResolution.intValue;
                    
                    // 将分辨率值转换为实际大小（enum值：0=256, 1=512, 2=1024, 3=2048, 4=4096）
                    int targetEnumValue = newAtlasSize switch
                    {
                        256 => 0,
                        512 => 1,
                        1024 => 2,
                        2048 => 3,
                        4096 => 4,
                        8192 => 5,
                        _ => 4
                    };
                    
                    additionalLightsShadowmapResolution.intValue = targetEnumValue;
                    serializedObject.ApplyModifiedProperties();
                    
                    Debug.Log($"✅ 已修改: {path}\n   阴影图集大小: {currentValue} → {newAtlasSize}");
                    modifiedCount++;
                }
                
                EditorUtility.SetDirty(urpAsset);
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog(
            "完成", 
            $"已成功修改 {modifiedCount} 个URP资源文件！\n新的阴影图集大小：{newAtlasSize}x{newAtlasSize}\n\n请重新运行场景查看效果。", 
            "确定"
        );
    }
    
    /// <summary>
    /// 列出所有投射阴影的光源
    /// </summary>
    void ListAllShadowCastingLights()
    {
        Light[] allLights = FindObjectsOfType<Light>();
        int shadowCastingCount = 0;
        
        Debug.Log("========== 场景中投射阴影的光源 ==========");
        
        foreach (Light light in allLights)
        {
            if (light.shadows != LightShadows.None)
            {
                shadowCastingCount++;
                string lightInfo = $"[{shadowCastingCount}] {light.gameObject.name} - " +
                                  $"类型: {light.type}, " +
                                  $"阴影类型: {light.shadows}, " +
                                  $"强度: {light.intensity}";
                Debug.Log(lightInfo);
                
                // 在Scene视图中高亮显示
                EditorGUIUtility.PingObject(light.gameObject);
            }
        }
        
        Debug.Log($"========== 总共找到 {shadowCastingCount} 个投射阴影的光源 ==========");
        
        EditorUtility.DisplayDialog(
            "光源统计",
            $"找到 {shadowCastingCount} 个投射阴影的光源\n\n详细信息已输出到Console窗口",
            "确定"
        );
    }
    
    /// <summary>
    /// 禁用非主要光源的阴影
    /// </summary>
    void DisableNonEssentialShadows()
    {
        Light[] allLights = FindObjectsOfType<Light>();
        int disabledCount = 0;
        int mainLightCount = 0;
        
        foreach (Light light in allLights)
        {
            if (light.shadows != LightShadows.None)
            {
                // 保留主方向光和前3个最亮的光源
                if (light.type == LightType.Directional || mainLightCount < 3)
                {
                    mainLightCount++;
                    continue;
                }
                
                // 禁用其他光源的阴影
                Undo.RecordObject(light, "禁用光源阴影");
                light.shadows = LightShadows.None;
                disabledCount++;
                
                Debug.Log($"已禁用阴影: {light.gameObject.name}");
            }
        }
        
        EditorUtility.DisplayDialog(
            "完成",
            $"已禁用 {disabledCount} 个次要光源的阴影\n保留了 {mainLightCount} 个主要光源的阴影",
            "确定"
        );
    }
    
    /// <summary>
    /// 降低所有光源的阴影分辨率
    /// </summary>
    void ReduceLightShadowResolution()
    {
        Light[] allLights = FindObjectsOfType<Light>();
        int reducedCount = 0;
        
        foreach (Light light in allLights)
        {
            if (light.shadows != LightShadows.None)
            {
                // 使用反射访问shadowResolution属性
                SerializedObject serializedLight = new SerializedObject(light);
                SerializedProperty shadowResolution = serializedLight.FindProperty("m_ShadowResolution");
                
                if (shadowResolution != null && shadowResolution.intValue > 1)
                {
                    Undo.RecordObject(light, "降低阴影分辨率");
                    shadowResolution.intValue = 1; // 设置为Medium (0=High, 1=Medium, 2=Low)
                    serializedLight.ApplyModifiedProperties();
                    reducedCount++;
                    
                    Debug.Log($"已降低阴影分辨率: {light.gameObject.name}");
                }
            }
        }
        
        EditorUtility.DisplayDialog(
            "完成",
            $"已降低 {reducedCount} 个光源的阴影分辨率至Medium",
            "确定"
        );
    }
}

