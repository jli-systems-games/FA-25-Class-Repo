using System.Collections.Generic;
using UnityEngine;

public class PingPongBall : MonoBehaviour
{
    public float gravity;
    public List<AudioSource> poks;
    private float lastPokPlayedAt = 0f;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        _rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (lastPokPlayedAt + 0.1f > Time.time)
            return;

        int randy = Random.Range(0, poks.Count);
        poks[randy].Play();
        lastPokPlayedAt =  Time.time;
    }
}
