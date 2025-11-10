using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CatAudioController : MonoBehaviour
{
    public Transform player;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;  // 3D sound
        audioSource.loop = true;
        if (audioSource.clip != null)
            audioSource.Play();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float volume = Mathf.Clamp01(1 - (distance / 25f));
        audioSource.volume = volume;
    }

    // Allows dynamic assignment of the player at runtime
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }
}