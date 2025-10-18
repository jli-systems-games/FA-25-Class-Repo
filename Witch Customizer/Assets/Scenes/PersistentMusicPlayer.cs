using UnityEngine;

public class PersistentMusicPlayer : MonoBehaviour
{
    public AudioClip musicClip; // Assign your track in the inspector
    private AudioSource audioSource;
    private static PersistentMusicPlayer instance;

    void Awake()
    {
        // Singleton: only one music player at a time!
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.Play();
    }
}