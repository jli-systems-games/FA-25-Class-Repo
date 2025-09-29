using UnityEngine;
using UnityEngine.Rendering;

public class Explosion : MonoBehaviour
{
    [Header("Explosion")]
    public float radius = 12f;
    public float force = 1200f;
    public float upwardModifier = 0f;
    public LayerMask affectedLayers = ~0;
    public bool ignoreSelf = true;

    [Header("Trigger")]
    public KeyCode testKey = KeyCode.E;
    public float cooldown = 1.5f;
    bool ready = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sfxExplosion;
    public AudioClip sfxReady;
    public float sfxVolume = 1f;

    [Header("Post-Processing Pulse (URP/HDRP)")]
    public Volume postPulseVolume;
    public float ppPeakWeight = 0.8f;
    public float ppDuration = 0.35f;
    public AnimationCurve ppCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);

    Rigidbody selfRb;

    void Awake()
    {
        selfRb = GetComponent<Rigidbody>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        if (postPulseVolume) postPulseVolume.weight = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(testKey)) TryExplode();
    }

    public void TryExplode()
    {
        if (!ready) return;
        Explode();
        StartCooldown();
    }

    void Explode()
    {
        var cols = Physics.OverlapSphere(transform.position, radius, affectedLayers, QueryTriggerInteraction.Ignore);
        foreach (var c in cols)
        {
            var rb = c.attachedRigidbody;
            if (rb == null) continue;
            if (ignoreSelf && (rb == selfRb || rb.transform.root == transform.root)) continue;
            rb.AddExplosionForce(force, transform.position, radius, upwardModifier, ForceMode.Impulse);
        }
        if (sfxExplosion) audioSource.PlayOneShot(sfxExplosion, sfxVolume);
        if (postPulseVolume && ppDuration > 0f && ppPeakWeight > 0f)
            StartCoroutine(PulsePostFX());
    }

    void StartCooldown()
    {
        ready = false;
        StopAllCoroutines();
        if (sfxReady) audioSource.PlayOneShot(sfxReady, sfxVolume);
        if (postPulseVolume && ppDuration > 0f && ppPeakWeight > 0f)
            StartCoroutine(PulsePostFX());
        StartCoroutine(CooldownRoutine());
    }

    System.Collections.IEnumerator CooldownRoutine()
    {
        float t = 0f;
        while (t < cooldown)
        {
            t += Time.deltaTime;
            yield return null;
        }
        ready = true;
    }

    System.Collections.IEnumerator PulsePostFX()
    {
        float t = 0f;
        while (t < ppDuration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / ppDuration);
            float w = ppCurve.Evaluate(n) * ppPeakWeight;
            postPulseVolume.weight = w;
            yield return null;
        }
        postPulseVolume.weight = 0f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.15f);
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}