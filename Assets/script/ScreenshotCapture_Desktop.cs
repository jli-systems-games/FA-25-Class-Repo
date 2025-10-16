using UnityEngine;
using System.Collections;

public class ScreenshotCapture_Desktop : MonoBehaviour
{
    [Header("分辨率")]
    public int superSize = 1;

    [Header("截图")]
    public KeyCode screenshotKey = KeyCode.P;

    void Update()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            StartCoroutine(CaptureScreenshot());
        }
    }

    IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width * superSize;
        int height = Screen.height * superSize;


        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tex.Apply();
        byte[] pngData = tex.EncodeToPNG();

 
        string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        DownloadFile(pngData, fileName);

        Destroy(tex);
    }


    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, string fileName);
}