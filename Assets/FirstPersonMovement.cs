using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float sprintSpeed;
    public float lookSpeed = 2f;
    public AudioSource speedUpAudio;

    private CharacterController controller;
    private Vector3 moveDirection;
    //public CharacterController fly;
    //private float xRot;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        sprintSpeed = moveSpeed * 5f;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if ((Input.GetKey(KeyCode.LeftArrow)|| Input.GetKey(KeyCode.A)) && (Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.D)))
        {
            moveDirection = Camera.main.transform.forward;
            if (Input.GetKey(KeyCode.Space))
            {
                speedUpAudio.pitch = 1.3f;
                controller.Move(moveDirection * sprintSpeed * Time.deltaTime);
            }
            else
            {
                speedUpAudio.pitch = 1f;
                controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            }


        }

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.Rotate(Vector3.left * mouseY);
    }
}
