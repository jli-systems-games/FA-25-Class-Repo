using UnityEngine;
using UnityEngine.UI;

public class PhotoCamera : MonoBehaviour
{
    public KeyCode photoKey = KeyCode.E;      // press E to snap
    public RawImage photoPreview;             // UI element for showing photo
    public RenderTexture photoTexture;        // “film” for polaroid
    public Camera mainCamera;                 // player camera
    public AudioClip shutterSound;            // assign shutter audio in Inspector
    private AudioSource audioSource;          // plays the sound

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Make sure there’s an AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(photoKey))
        {
            TakePhoto();
        }
    }

    void TakePhoto()
    {
        // Play shutter sound
        if (shutterSound != null)
        {
            audioSource.PlayOneShot(shutterSound);
        }

        // Temporarily set the camera to render into the RenderTexture
        mainCamera.targetTexture = photoTexture;

        // Make a new Texture2D and copy pixels from the RenderTexture
        Texture2D snapshot = new Texture2D(photoTexture.width, photoTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = photoTexture;
        mainCamera.Render();
        snapshot.ReadPixels(new Rect(0, 0, photoTexture.width, photoTexture.height), 0, 0);
        snapshot.Apply();

        // Reset
        mainCamera.targetTexture = null;
        RenderTexture.active = null;

        // Show snapshot in UI
        photoPreview.texture = snapshot;

        Debug.Log("📸 Took a photo!");
    }
}

