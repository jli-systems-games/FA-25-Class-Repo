using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TimedDespawnOnMotion : MonoBehaviour
{
    public float speedThreshold = 0.5f;
    public float distanceThreshold = 0.15f;
    public float lifetimeAfterTriggered = 5f;
    public bool destroyObject = true;

    Rigidbody rb;
    Vector3 startPos;
    bool armed;
    float armTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    void Update()
    {
        if (!armed)
        {
            float spd = rb ? rb.linearVelocity.magnitude : 0f;
            if (spd >= speedThreshold || Vector3.Distance(transform.position, startPos) >= distanceThreshold)
            {
                armed = true;
                armTime = Time.time + lifetimeAfterTriggered;
            }
        }
        else
        {
            if (Time.time >= armTime)
            {
                if (destroyObject) Destroy(gameObject);
                else gameObject.SetActive(false);
            }
        }
    }
}
