using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveMusic : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    public float minSpeed = 0.1f;
    public float baseVolume = 0.8f;
    public float fadeIn = 0.15f;
    public float fadeOut = 0.25f;

    Rigidbody rb;
    float vol;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        if (clip) audioSource.clip = clip;
        audioSource.volume = 0f;
        vol = 0f;
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;
        bool moving = speed >= minSpeed;

        if (moving)
        {
            if (!audioSource.isPlaying && clip) audioSource.Play();
            vol = Mathf.MoveTowards(vol, baseVolume, Time.deltaTime / Mathf.Max(0.0001f, fadeIn) * baseVolume);
        }
        else
        {
            vol = Mathf.MoveTowards(vol, 0f, Time.deltaTime / Mathf.Max(0.0001f, fadeOut) * baseVolume);
            if (audioSource.isPlaying && vol <= 0.001f) audioSource.Stop();
        }

        audioSource.volume = vol;
    }
}
