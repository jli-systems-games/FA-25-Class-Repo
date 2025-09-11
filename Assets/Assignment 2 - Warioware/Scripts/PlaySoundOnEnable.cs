using UnityEngine;

public class PlaySoundOnEnable : MonoBehaviour
{
    private AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        audioSource.Play();
    }
}
