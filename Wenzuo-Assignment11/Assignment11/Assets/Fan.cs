using UnityEngine;

public class Fan : MonoBehaviour
{
    public float force = 60f;
    public float duration = 6f;
    public Transform rotor;
    public float rotorSpeed = 720f;
    float t;

    void Update()
    {
        t += Time.deltaTime;
        if (t >= duration) Destroy(gameObject);
        if (rotor) rotor.Rotate(0f, rotorSpeed * Time.deltaTime, 0f, Space.Self);
    }

    void OnTriggerStay(Collider other)
    {
        Rigidbody r = other.attachedRigidbody;
        if (r == null) return;
        Vector3 dir = transform.forward;
        r.AddForce(dir * force, ForceMode.Acceleration);
    }
}
