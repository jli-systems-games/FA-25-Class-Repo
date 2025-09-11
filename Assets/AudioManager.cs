using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clickSounds;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int rand = Random.Range(0, clickSounds.Length);
            audioSource.PlayOneShot(clickSounds[rand]);
        }
    }
}
