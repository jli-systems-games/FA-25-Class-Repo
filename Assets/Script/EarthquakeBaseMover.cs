using UnityEngine;
using System.Collections;
public class EarthquakeBaseMover : MonoBehaviour
{
    public float amplitude = 0.35f;
    public float frequency = 2.2f;
    [Range(0f,1f)] public float noise = 0.15f;
    public float quakeDuration = 1.2f;
    public Vector3 axis = Vector3.right;
    public AudioClip quakeClip;

    Rigidbody rb;
    Vector3 initPos;
    Coroutine running;
    public bool IsShaking => running != null;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        axis = axis == Vector3.zero ? Vector3.right : axis.normalized;
        initPos = transform.position;
    }

    public void TriggerOnce()
    {
        AudioSource.PlayClipAtPoint(quakeClip, transform.position);
        if (running != null) return;
        running = StartCoroutine(ShakeOnce());
    }

    IEnumerator ShakeOnce()
    {
        float t0 = Time.time;
        while (Time.time - t0 < quakeDuration)
        {
            float t = Time.time - t0;
            float s = Mathf.Sin(2f * Mathf.PI * frequency * t);
            float r = (Mathf.PerlinNoise(0f, t * frequency) * 2f - 1f) * noise;
            rb.MovePosition(initPos + axis * amplitude * (s + r));
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(initPos);
        running = null;
    }
}