using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static event Action<Coin> OnCollected;

    [Header("Audio")]
    public AudioClip collectSfx;
    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        var col = GetComponentInChildren<Collider>();
        if (!col) col = gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;

        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (collectSfx)
        {
            AudioSource.PlayClipAtPoint(collectSfx, transform.position, volume);
        }

        OnCollected?.Invoke(this);
        Destroy(gameObject);
    }
}