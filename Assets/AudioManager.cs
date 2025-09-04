using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] clips;

    public AudioSource audioSource;

    void OnCollisionEnter2D(Collision2D collision)
    {
        int index = Random.Range(0, clips.Length);
        AudioClip clings = clips[index];

        audioSource.PlayOneShot(clings);
    }
}
