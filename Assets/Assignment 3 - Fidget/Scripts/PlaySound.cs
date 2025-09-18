using Unity.VisualScripting;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioClip popAudio;
    public AudioClip blackAudio;
    public AudioClip glassAudio;
    public GameObject balls;

    private bool isBallShowing = false;
    public AudioSource audioSource;
    public AudioSource loudAudioSource;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pop"))
        {
            loudAudioSource.PlayOneShot(popAudio);
        }
        else if (other.gameObject.CompareTag("Black"))
        {
            audioSource.PlayOneShot(blackAudio);
        }
        else if (other.gameObject.CompareTag("Stair"))
        {
            audioSource.PlayOneShot(glassAudio);
        }
        else if (other.gameObject.CompareTag("Ball") && !isBallShowing)
        {
            balls.SetActive(true);
            isBallShowing = true;
        }
        else if (other.gameObject.CompareTag("Ball") && isBallShowing)
        {
            balls.SetActive(false);
            isBallShowing = false;
        }
    }
}
