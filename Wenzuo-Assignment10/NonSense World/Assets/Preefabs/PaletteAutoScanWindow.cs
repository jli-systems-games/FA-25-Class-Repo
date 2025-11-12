#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PaletteAutoScanWindow : EditorWindow
{
    [MenuItem("Tools/Nonsense/Build Palette From Folders")]
    public static void Open() => GetWindow<PaletteAutoScanWindow>("Nonsense Palette");

    public DefaultAsset folderA; // 例如: Assets/CartoonLowPolyCity/Prefabs
    public DefaultAsset folderB; // 例如: Assets/LowPolySciFiCity/Prefabs
    public PrefabPalette outputPalette;

    void OnGUI()
    {
        EditorGUILayout.HelpBox("选择两个 Prefabs 根文件夹，然后点 Scan。一切会自动把“可当平台”的 Prefab 放入 Palette，车辆会被排除。", MessageType.Info);
        folderA = (DefaultAsset)EditorGUILayout.ObjectField("Folder A", folderA, typeof(DefaultAsset), false);
        folderB = (DefaultAsset)EditorGUILayout.ObjectField("Folder B", folderB, typeof(DefaultAsset), false);
        outputPalette = (PrefabPalette)EditorGUILayout.ObjectField("Output Palette", outputPalette, typeof(PrefabPalette), false);

        if (GUILayout.Button("Create New Palette Asset 到 Assets/Palettes"))
        {
            System.IO.Directory.CreateDirectory("Assets/Palettes");
            outputPalette = ScriptableObject.CreateInstance<PrefabPalette>();
            AssetDatabase.CreateAsset(outputPalette, "Assets/Palettes/NonsensePalette.asset");
            AssetDatabase.SaveAssets();
            Selection.activeObject = outputPalette;
        }

        using (new EditorGUI.DisabledScope(outputPalette == null || (folderA == null && folderB == null)))
        {
            if (GUILayout.Button("Scan & Fill Palette"))
            {
                ScanAndFill();
            }
        }
    }

    void ScanAndFill()
    {
        var paths = new List<string>();
        if (folderA) paths.Add(AssetDatabase.GetAssetPath(folderA));
        if (folderB) paths.Add(AssetDatabase.GetAssetPath(folderB));

        var guids = AssetDatabase.FindAssets("t:Prefab", paths.ToArray());
        var all = guids.Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(g)))
                       .Where(p => p != null).ToList();

        // 简单规则：筛掉车/载具，剩下基本都可当平台
        bool IsVehicle(string n) => n.Contains("car") || n.Contains("vehicle") || n.Contains("truck") || n.Contains("bus");
        bool IsTiny(string n) => n.Contains("cup") || n.Contains("mug") || n.Contains("fork") || n.Contains("spoon");
        bool LooksRare(string n) => n.Contains("ufo") || n.Contains("tower") || n.Contains("statue") || n.Contains("monument") || n.Contains("balloon") || n.Contains("landmark");

        outputPalette.platformPrefabs.Clear();
        outputPalette.propPrefabs.Clear();
        outputPalette.rarePrefabs.Clear();

        foreach (var p in all)
        {
            var n = p.name.ToLower();
            if (IsVehicle(n) || IsTiny(n)) continue;

            // 先粗分 rare
            if (LooksRare(n)) { outputPalette.rarePrefabs.Add(p); continue; }

            // 大多数都进 platform；很小的（如路钉、小广告）归到 props
            var r = p.GetComponentInChildren<Renderer>();
            if (r && (r.bounds.size.x > 1f || r.bounds.size.z > 1f))
                outputPalette.platformPrefabs.Add(p);
            else
                outputPalette.propPrefabs.Add(p);
        }

        EditorUtility.SetDirty(outputPalette);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Palette] 平台 {outputPalette.platformPrefabs.Count} | 道具 {outputPalette.propPrefabs.Count} | 稀有 {outputPalette.rarePrefabs.Count}");
    }
}
#endif
