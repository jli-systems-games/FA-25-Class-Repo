using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    public AudioClip landingSfx;
    private AudioSource audioSource;
    private bool counted = false;
    private GameControl counter;

    private void Start()
    {
        counter = Object.FindAnyObjectByType<GameControl>();
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!counted && other.CompareTag("Ground"))
        {
            
            if (counter != null) counter.AddCount();
            
            if (landingSfx) audioSource.PlayOneShot(landingSfx);
            
            gameObject.tag = "Untagged";
            counted = true;
        }
    }
}