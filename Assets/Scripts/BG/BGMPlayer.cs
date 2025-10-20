using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer Instance;
    public AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    public void Play(AudioClip clip, float volume = 0.5f)
    {
        if (audioSource.isPlaying) return;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }
}