using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;
    private AudioSource audioSource;

    void Awake()
    {
        // Singleton: keep only one music player across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;

            // If you want to start automatically, either set "Play On Awake" on the AudioSource
            // in the Inspector or uncomment the next line:
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            // Destroy duplicates created when loading a scene that also has this object
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    // Optional helpers
    public void SetVolume(float volume)
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.volume = Mathf.Clamp01(volume);
    }

    public void PlayClip(AudioClip clip, bool loop = true)
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }
}