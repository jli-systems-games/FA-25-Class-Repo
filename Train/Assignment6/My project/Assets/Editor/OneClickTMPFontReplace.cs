using UnityEngine;
using UnityEditor;
using TMPro;

public class OneClickTMPFontReplace : Editor
{
    [MenuItem("Tools/TMP/One-Click Replace Font (Use Selected)")]
    public static void ReplaceAll()
    {
        Object sel = Selection.activeObject;
        if (sel == null)
        {
            EditorUtility.DisplayDialog("TMP Font", "Select a TTF/OTF or a TMP_FontAsset.", "OK");
            return;
        }

        TMP_FontAsset target = null;

        if (sel is TMP_FontAsset)
        {
            target = sel as TMP_FontAsset;
        }
        else if (sel is Font)
        {
            var font = sel as Font;
            string path = AssetDatabase.GetAssetPath(font);
            string dir = System.IO.Path.GetDirectoryName(path);
            string name = System.IO.Path.GetFileNameWithoutExtension(path) + "_TMP";
            string assetPath = System.IO.Path.Combine(dir, name + ".asset").Replace("\\", "/");
            target = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (target == null)
            {
                target = TMP_FontAsset.CreateFontAsset(font);
                target.atlasPopulationMode = AtlasPopulationMode.Dynamic;
                AssetDatabase.CreateAsset(target, assetPath);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            EditorUtility.DisplayDialog("TMP Font", "Select a TTF/OTF or a TMP_FontAsset.", "OK");
            return;
        }

        if (target == null)
        {
            EditorUtility.DisplayDialog("TMP Font", "Failed to get or create TMP_FontAsset.", "OK");
            return;
        }

        int sceneCount = 0;
#if UNITY_2023_1_OR_NEWER || UNITY_6000_0_OR_NEWER
        var texts = GameObject.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        var texts = GameObject.FindObjectsOfType<TMP_Text>(true);
#endif
        Undo.RecordObjects(texts, "Replace TMP Font In Scene");
        foreach (var t in texts) t.font = target;
        sceneCount = texts.Length;

        int prefabCount = 0;
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        for (int i = 0; i < guids.Length; i++)
        {
            string p = AssetDatabase.GUIDToAssetPath(guids[i]);
            var root = PrefabUtility.LoadPrefabContents(p);
            if (root != null)
            {
                var tmps = root.GetComponentsInChildren<TMP_Text>(true);
                foreach (var t in tmps) t.font = target;
                prefabCount += tmps.Length;
                PrefabUtility.SaveAsPrefabAsset(root, p);
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        TMP_Settings.defaultFontAsset = target;
        if (TMP_Settings.instance != null) EditorUtility.SetDirty(TMP_Settings.instance);

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("TMP Font", "Replaced in Scene: " + sceneCount + "\nReplaced in Prefabs: " + prefabCount + "\nDefault TMP font updated.", "OK");
    }
}
