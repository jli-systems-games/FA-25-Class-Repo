using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;

public class CaptureAndDownloadJPG : MonoBehaviour
{

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void SaveJPG(string base64, string filename);
#endif

    [Range(1, 100)]
    public int jpgQuality = 95;  
    public string filenamePrefix = "poem_";

    public void CaptureNow() => StartCoroutine(CaptureRoutine());

    private IEnumerator CaptureRoutine()
    {

        yield return new WaitForEndOfFrame();

        int w = Screen.width;
        int h = Screen.height;

        var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0, false);
        tex.Apply(false, false);

        byte[] jpg = tex.EncodeToJPG(jpgQuality);
        UnityEngine.Object.Destroy(tex);

        string fname = $"{filenamePrefix}{DateTime.Now:yyyyMMdd_HHmmss}.jpg";

#if UNITY_WEBGL && !UNITY_EDITOR
            // WebGL：转 base64，交给 JS 触发浏览器下载
            string base64 = Convert.ToBase64String(jpg);
            SaveJPG(base64, fname);
#else
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fname);
        File.WriteAllBytes(path, jpg);
        Debug.Log($"Saved screenshot: {path}");
#endif
    }
}