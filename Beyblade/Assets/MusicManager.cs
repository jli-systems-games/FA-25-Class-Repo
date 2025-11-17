using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton logic: destroy extra copies if they exist
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.loop = true; // Background music usually loops
                audioSource.Play();
            }
        }
        else
        {
            Destroy(gameObject); // Only keep one MusicManager ever
        }
    }
}