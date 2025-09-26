using UnityEngine;

public class Knockable : MonoBehaviour
{
    public float triggerSpeed = 2.0f; // 相对速度阈值
    public float noiseRadius = 18f;
    public AudioSource audioSource;
    public AudioClip knockSfx;

    void OnCollisionEnter(Collision c)
    {
        if ((c.collider.CompareTag("Player")) && c.relativeVelocity.magnitude >= triggerSpeed)
        {
            if (audioSource && knockSfx) audioSource.PlayOneShot(knockSfx);
            NoiseSystem.Broadcast(transform.position, noiseRadius);
        }
    }
}
