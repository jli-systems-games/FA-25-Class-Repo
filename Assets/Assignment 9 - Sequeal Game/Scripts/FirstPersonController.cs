using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour
{
    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public LayerMask groundedMask;

    private Transform cameraT;
    private float verticalLookRotation;

    private Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;

    private Rigidbody rb;

    private bool grounded;
    void Start()
    {
        cameraT = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);

        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90); //clamping the upwards rotation so that the player cant 360
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetKey(KeyCode.LeftShift)) {
            Vector3 targetMoveAmount = moveDirection * runSpeed;
            moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);
        }
        else
        {
            Vector3 targetMoveAmount = moveDirection * walkSpeed;
            moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                rb.AddForce(transform.up * jumpForce); //only jump when the player is on the ground
            }
        }

        grounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1 + 1f, groundedMask)) {
            grounded = true;
        }
  
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
