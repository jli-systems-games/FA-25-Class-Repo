using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;


/// <summary>
/// /is script is referencing https://stackoverflow.com/questions/71082188/how-to-take-a-screenshot-of-the-game-view-in-unity
/// </summary>
public class SaveRing : MonoBehaviour
{
    public Camera captureCamera;
    public GameObject popupText;
    public AudioClip magicSFX;
    public AudioSource audioSource;

    public void SaveImage()
    {
        int width = Screen.width;
        int height = Screen.height;

        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

        captureCamera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        string time = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = "your funeral ring_" + time + ".png";
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string path = Path.Combine(desktopPath, fileName);
        File.WriteAllBytes(path, screenshot.EncodeToPNG());

        Debug.Log("Saved to: " + path);

        StartCoroutine(ShowText());
        audioSource.PlayOneShot(magicSFX);
    }

    IEnumerator ShowText()
    {
        popupText.SetActive(true);
        yield return new WaitForSeconds(3.4f);
        popupText.SetActive(false);
    }
}
