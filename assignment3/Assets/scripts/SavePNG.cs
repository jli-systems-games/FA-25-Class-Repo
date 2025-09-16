using UnityEngine;
using System.IO;

public class SavePNG : MonoBehaviour
{
    public Camera captureCamera;
    public RenderTexture rt;

    public string fileName = "RelaxPaint.png";

    public void Save()
    {
        if (!captureCamera || !rt) return;

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = prev;

        byte[] png = tex.EncodeToPNG();
#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, "../" + fileName);
#else
        string path = Path.Combine(Application.persistentDataPath, fileName);
#endif
        File.WriteAllBytes(path, png);
        Debug.Log("Saved to: " + path);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.RevealInFinder(path);
#endif
        Object.Destroy(tex);
    }
}
