using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    public Texture2D screenCapture;
    public Image photoDisplayArea;
    public GameObject photoFrame;

    public GameObject cameraFlash;
    public float flashTime;
    public float imageDisappearTime = 3f;

    public Animator fadingAnimation;

    private PhotoPlaceOnResearch photoPlaceOnReasearch;

    void Start()
    {
        screenCapture = new Texture2D(Screen.width,Screen.height, TextureFormat.RGB24, false);
        photoPlaceOnReasearch = GetComponent<PhotoPlaceOnResearch>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!Data.viewingPhoto)
            {
                StartCoroutine(CapturePhoto());
            }
            else
            {
                return;
            }
        }
    }

    IEnumerator CapturePhoto()
    {
        Data.viewingPhoto = true;

        yield return new WaitForEndOfFrame();

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();
        ShowPhoto();
    }

    void ShowPhoto()
    {
        Data.photoSprite = Sprite.Create(screenCapture, new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100f);
        photoDisplayArea.sprite = Data.photoSprite;

        Data.newPhotoTaken = true;

        photoFrame.SetActive(true);

        StartCoroutine(CameraFlashEffect(flashTime));
        fadingAnimation.Play("PhotoFade");

        StartCoroutine(ImageDisappear(imageDisappearTime));
    }

    IEnumerator CameraFlashEffect(float delay)
    {
        cameraFlash.SetActive(true);
        yield return new WaitForSeconds(delay);
        cameraFlash.SetActive(false);
    }

    IEnumerator ImageDisappear(float delay)
    {
        yield return new WaitForSeconds(delay);

        RemovePhoto();
    }

    void RemovePhoto()
    {
        Data.viewingPhoto = false;
        photoFrame.SetActive(false);
    }
}
