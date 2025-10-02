using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarControl : MonoBehaviour
{
    //Car Forward Variables
    public float forwardForce = 500f;
    public float extraGravity = 30f;
    public float forwardAngle = 2f;

    private float speedMultiplier = 1f;

    private Rigidbody rb;
    private bool inBuilding = true;

    public PlayerEnter playerEnter;

    public GameObject carCam;
    public GameObject jumpCam;
    public GameObject airCam;
    public GameObject sideCam;
    public GameObject poolCam;

    public GameObject windowCollider;

    public GameObject splashEffect;

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
        if (playerEnter.hasEnteredCar)
        {
            if (Input.GetKeyDown(KeyCode.Space) && inBuilding)
            {
                windowCollider.SetActive(false);

                rb.AddForce(transform.forward * forwardForce * speedMultiplier, ForceMode.Impulse);

                speedMultiplier *= 1.1f;

                Debug.Log("Force pushed.");
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("No more speed.");
            }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car Jump"))
        {
            carCam.SetActive(false);
            jumpCam.SetActive(true);
        }
        else if (other.gameObject.CompareTag("Car Air"))
        {
            jumpCam.SetActive(false);
            airCam.SetActive(true);
        }
        else if (other.gameObject.CompareTag("Car Side"))
        {
            airCam.SetActive(false);
            sideCam.SetActive(true);
        }
        else if (other.gameObject.CompareTag("Game Over"))
        {
            SceneManager.LoadScene("Game Over Scene");
        }
        else if (other.gameObject.CompareTag("Pool"))
        {
            sideCam.SetActive(false);
            poolCam.SetActive(true);

            splashEffect.SetActive(true);

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            StartCoroutine(DelayBeforeWin(2f));
        }
    }
    private IEnumerator DelayBeforeWin(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("Win Scene");
    }
}
