using UnityEngine;

public class FirstPersonAim : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float sensitivity = 100f;
    public float maxLookX = 60f;
    public float minLookX = -60f;

    [Header("References")]
    public Transform hand;
    public Animator handAnimator;

    private float rotationX;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleClick();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minLookX, maxLookX);

        transform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y, 0);
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (handAnimator != null)
                handAnimator.SetTrigger("Click");

       
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

            foreach (RaycastHit hit in hits)
            {
               
                if (hit.collider.CompareTag("npc"))
                    hit.collider.gameObject.SetActive(true);
            }
        }
    }
}
