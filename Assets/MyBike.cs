using UnityEngine;
using UnityEngine.SceneManagement;

public class MyBike : MonoBehaviour
{
    public float moveForce = 800f;
    public float brakeForce = 1500f;
    public float turnTorque = 200f;
    public float maxSpeed = 20f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (v > 0)
        {
            if (rb.linearVelocity.magnitude < maxSpeed)
                rb.AddForce(transform.forward * v * moveForce * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(-rb.linearVelocity.normalized * brakeForce * Time.fixedDeltaTime);
        }

        transform.Rotate(Vector3.up * h * turnTorque * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            SceneManager.LoadScene("SceneTwo");
        }
    }
}
