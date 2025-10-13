using UnityEngine;
using UnityEditor;
using TMPro;

public class FixReferencePanel : Editor
{
    [MenuItem("Tools/UI/Fix Reference Panel")]
    public static void Fix()
    {
        TMP_FontAsset fontAsset = null;
        var selTMP = Selection.activeObject as TMP_FontAsset;
        if (selTMP != null) fontAsset = selTMP;
        else
        {
            var selFont = Selection.activeObject as Font;
            if (selFont != null)
            {
                string path = AssetDatabase.GetAssetPath(selFont);
                string dir = System.IO.Path.GetDirectoryName(path);
                string name = System.IO.Path.GetFileNameWithoutExtension(path) + "_TMP";
                string assetPath = System.IO.Path.Combine(dir, name + ".asset").Replace("\\", "/");
                fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
                if (fontAsset == null)
                {
                    fontAsset = TMP_FontAsset.CreateFontAsset(selFont);
                    fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
                    AssetDatabase.CreateAsset(fontAsset, assetPath);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        if (fontAsset == null)
        {
            EditorUtility.DisplayDialog("Fix Reference Panel", "Select a TMP_FontAsset or a TTF/OTF font in Project first.", "OK");
            return;
        }

        var panel = GameObject.Find("ReferencePanel");
        if (panel == null)
        {
            EditorUtility.DisplayDialog("Fix Reference Panel", "Cannot find 'ReferencePanel' in scene.", "OK");
            return;
        }

        void FixOne(string childName, int size)
        {
            var t = panel.transform.Find(childName);
            if (!t) return;
            var tmp = t.GetComponent<TMP_Text>();
            if (!tmp) return;
            Undo.RecordObject(tmp, "Fix Reference TMP");
            tmp.font = fontAsset;
            tmp.enableAutoSizing = false;
            tmp.fontSize = size;
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.enableWordWrapping = true;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.characterSpacing = 0;
            tmp.lineSpacing = 0;
            EditorUtility.SetDirty(tmp);
        }

        FixOne("Today", 24);
        FixOne("ValidDest", 20);
        FixOne("Stamp", 20);

        EditorUtility.DisplayDialog("Fix Reference Panel", "Done.", "OK");
    }
}
