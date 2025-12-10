using UnityEngine;
using UnityEditor;

/// <summary>
/// Plant组件的自定义Inspector编辑器
/// 提供快捷按钮和预览功能
/// </summary>
[CustomEditor(typeof(Plant))]
public class PlantEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Plant plant = (Plant)target;
        
        // 绘制默认Inspector
        DrawDefaultInspector();
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("预设状态快捷设置", EditorStyles.boldLabel);
        
        if (plant.plantData == null)
        {
            EditorGUILayout.HelpBox("请先设置 Plant Data", MessageType.Warning);
            return;
        }
        
        // 快捷按钮
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("设为幼苗"))
        {
            SetPresetState(plant, 0f, false, 0f);
        }
        
        if (GUILayout.Button("设为成熟"))
        {
            SetPresetState(plant, plant.plantData.lifespanMin, true, 0f);
        }
        
        if (GUILayout.Button("设为老年"))
        {
            SetPresetState(plant, plant.plantData.lifespanMax * 0.8f, true, 0f);
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("健康成熟体"))
        {
            SetPresetState(plant, plant.plantData.lifespanMin, true, 0f);
        }
        
        if (GUILayout.Button("轻伤成熟体"))
        {
            SetPresetState(plant, plant.plantData.lifespanMin, true, plant.plantData.witherTime * 0.3f);
        }
        
        if (GUILayout.Button("濒死成熟体"))
        {
            SetPresetState(plant, plant.plantData.lifespanMin, true, plant.plantData.witherTime * 0.9f);
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button("清除预设（从幼苗开始）", GUILayout.Height(30)))
        {
            ClearPreset(plant);
        }
        
        // 显示预设状态预览
        if (GetUsePresetState(plant))
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(GetPresetStateDescription(plant), MessageType.Info);
        }
    }
    
    private void SetPresetState(Plant plant, float age, bool mature, float witherDamage)
    {
        SerializedObject so = new SerializedObject(plant);
        
        so.FindProperty("usePresetState").boolValue = true;
        so.FindProperty("presetAge").floatValue = age;
        so.FindProperty("startMature").boolValue = mature;
        so.FindProperty("presetWitherDamage").floatValue = witherDamage;
        
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(plant);
    }
    
    private void ClearPreset(Plant plant)
    {
        SerializedObject so = new SerializedObject(plant);
        
        so.FindProperty("usePresetState").boolValue = false;
        so.FindProperty("presetAge").floatValue = 0f;
        so.FindProperty("startMature").boolValue = false;
        so.FindProperty("presetWitherDamage").floatValue = 0f;
        
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(plant);
    }
    
    private bool GetUsePresetState(Plant plant)
    {
        SerializedObject so = new SerializedObject(plant);
        return so.FindProperty("usePresetState").boolValue;
    }
    
    private string GetPresetStateDescription(Plant plant)
    {
        SerializedObject so = new SerializedObject(plant);
        
        float age = so.FindProperty("presetAge").floatValue;
        bool mature = so.FindProperty("startMature").boolValue;
        float witherDamage = so.FindProperty("presetWitherDamage").floatValue;
        
        string desc = $"预设状态：\n";
        desc += $"• 年龄: {age:F1}秒";
        
        if (mature)
        {
            desc += " (成熟)";
        }
        else if (age >= plant.plantData.lifespanMin)
        {
            desc += " (即将成熟)";
        }
        else
        {
            desc += " (幼年)";
        }
        
        desc += $"\n• 枯萎伤害: {witherDamage:F1}/{plant.plantData.witherTime}秒";
        
        if (witherDamage == 0f)
        {
            desc += " (健康)";
        }
        else if (witherDamage < plant.plantData.witherTime * 0.5f)
        {
            desc += " (轻伤)";
        }
        else if (witherDamage < plant.plantData.witherTime * 0.9f)
        {
            desc += " (重伤)";
        }
        else
        {
            desc += " (濒死)";
        }
        
        float remainingLife = plant.plantData.lifespanMax - age;
        float remainingHealth = plant.plantData.witherTime - witherDamage;
        
        desc += $"\n• 剩余寿命: {remainingLife:F1}秒";
        desc += $"\n• 剩余枯萎容错: {remainingHealth:F1}秒";
        
        return desc;
    }
}
