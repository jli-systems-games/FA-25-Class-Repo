using UnityEngine;

public class FootstepSFXSimple : MonoBehaviour
{
    public Rigidbody rb;
    public AudioSource audioSource;
    public AudioClip[] clips;
    public float minInterval = 0.22f;
    public float speedThreshold = 1.2f;
    public float stepFreqAtRun = 2.8f;

    float t;
    float lastStep;

    void Update()
    {
        if (!rb || !audioSource || clips == null || clips.Length == 0) return;

        Vector3 v = rb.linearVelocity;
        float speed = new Vector3(v.x, 0f, v.z).magnitude;
        if (speed < speedThreshold) { t = 0f; return; }

        float freq = Mathf.Lerp(1.6f, stepFreqAtRun, Mathf.Clamp01((speed - speedThreshold) / 4f));
        t += Time.deltaTime * freq;
        if (t >= 1f)
        {
            if (Time.time - lastStep >= minInterval)
            {
                var clip = clips[Random.Range(0, clips.Length)];
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(clip);
                lastStep = Time.time;
            }
            t = 0f;
        }
    }
}
