using UnityEngine;
using UnityEngine.SceneManagement;

public class FishControllerBehindCamera : MonoBehaviour
{
    public float rotationSpeed = 5f;

    public float maxVerticalRotation = 30f; 
    public float maxHorizontalRotation = 45f; 

    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    public GameObject truncate;
    public GameObject rounded;
    public GameObject forked;
    public GameObject lunate;

    private void Start()
    {
        if (Data.isTruncate)
        {
            truncate.SetActive(true);
            rounded.SetActive(false);
            forked.SetActive(false);
            lunate.SetActive(false);
        }
        else if (Data.isForked)
        {
            truncate.SetActive(false);
            rounded.SetActive(false);
            forked.SetActive(true);
            lunate.SetActive(false);
        }
        else if (Data.isLunate)
        {
            truncate.SetActive(false);
            rounded.SetActive(false);
            forked.SetActive(false);
            lunate.SetActive(true);
        }
        else if (Data.isRounded)
        {
            truncate.SetActive(false);
            rounded.SetActive(true);
            forked.SetActive(false);
            lunate.SetActive(false);
        }
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); 
        float vertical = Input.GetAxisRaw("Vertical"); 

        Vector3 move = new Vector3(horizontal, vertical, 0f).normalized;
        transform.Translate(move * Data.movementSpeed * Time.deltaTime, Space.World);

        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -0.222f, 0.227f);
        clampedPos.y = Mathf.Clamp(clampedPos.y, -0.065f, 0.096f);
        transform.position = clampedPos;

        verticalRotation = Mathf.Lerp(verticalRotation, -vertical * maxVerticalRotation, rotationSpeed * Time.deltaTime);

        horizontalRotation = Mathf.Lerp(horizontalRotation, horizontal * maxHorizontalRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Seaweed"))
        {
            SceneManager.LoadScene("Complete Scene");
        }
    }
}
