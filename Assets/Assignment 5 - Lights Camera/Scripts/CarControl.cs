using UnityEngine;

public class CarControl : MonoBehaviour
{
    //Car Forward Variables
    public float forwardForce = 500f;
    public float extraGravity = 30f;
    public float forwardAngle = 2f;

    private float speedMultiplier = 1f;

    private Rigidbody rb;
    private bool inBuilding = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Building"))
        {
            inBuilding = false;
        }    
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && inBuilding)
        {
            rb.AddForce(transform.forward * forwardForce * speedMultiplier, ForceMode.Impulse);

            speedMultiplier *= 1.1f;

            Debug.Log("Force pushed.");
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("No more speed.");
        }
    }

    private void FixedUpdate()
    {
        if (!inBuilding)
        {
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);

            rb.AddTorque(Vector3.right * forwardAngle, ForceMode.Acceleration);
        }
    }
}
