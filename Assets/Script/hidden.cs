using UnityEngine;

public class hidden : MonoBehaviour
{
    public GameObject canvasObject;
    public AudioClip landingSfx;
    private bool triggered = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();
        if (canvasObject) canvasObject.SetActive(false);
    }

    private void Update()
    {
        if (canvasObject && canvasObject.activeInHierarchy && Input.GetKeyDown(KeyCode.B))
        {
            canvasObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Ground"))
        {
            triggered = true;
            if (landingSfx)
                audioSource.PlayOneShot(landingSfx);
            if (canvasObject)
                canvasObject.SetActive(true);
        }
    }
}