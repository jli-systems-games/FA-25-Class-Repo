using UnityEngine;
using System.IO;

public class SavePNG : MonoBehaviour
{
    public Camera captureCamera;
    public RenderTexture rt;
    public string fileName = "assignment3.png";

    public void Save()
    {
        if (!captureCamera || !rt) return;


        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        captureCamera.Render();


        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = prev;


        string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string path = Path.Combine(desktop, fileName);

        File.WriteAllBytes(path, tex.EncodeToPNG());

        Debug.Log($"[SavePNG] 保存完成: {path}");

#if UNITY_EDITOR
        UnityEditor.EditorUtility.RevealInFinder(path);
#endif

        Object.Destroy(tex);
    }
}
