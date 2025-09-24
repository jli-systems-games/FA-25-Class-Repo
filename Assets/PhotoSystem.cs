using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PhotoSystem : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject photoPrefab;
    public float photoSize = 10f;
    public AudioSource shutterSound;

    private List<Texture2D> album = new List<Texture2D>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            shutterSound.PlayOneShot(shutterSound.clip);
            TakePhoto();
        }
    }

    void TakePhoto()
    {
        RenderTexture rt = new RenderTexture(512, 512, 24);
        mainCamera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(512, 512, TextureFormat.RGB24, false);
        mainCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        screenShot.Apply();

        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        album.Add(screenShot);

        Vector3 spawnPos = transform.position + transform.forward * -10f;
        GameObject photo = Instantiate(photoPrefab, spawnPos, Quaternion.identity);

        Material mat = photo.GetComponent<Renderer>().material;
        mat.mainTexture = screenShot;
        photo.transform.localScale = Vector3.one * photoSize;

        StartCoroutine(FacePlayer(photo));
    }

    System.Collections.IEnumerator FacePlayer(GameObject photo)
    {
        Transform playerCam = Camera.main.transform;
        while (photo != null)
        {
            photo.transform.LookAt(playerCam);
            photo.transform.forward = -photo.transform.forward;
            yield return null;
        }
    }

    public Texture2D GetPhoto(int index)
    {
        if (index >= 0 && index < album.Count)
        {
            return album[index];
        }
        return null;
    }

    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            SceneManager.LoadScene("EndScene");
        }
    }
}
