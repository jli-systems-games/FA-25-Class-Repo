using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SwimController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float sprintMultiplier = 1.5f;
    public float verticalSpeed = 3f;
    public float acceleration = 6f;
    public float rotationSpeed = 120f;
    public float pitchSpeed = 80f;

    public Transform cameraPivot;
    public float minPitch = -60f, maxPitch = 60f;

    CharacterController cc;
    Vector3 currentVel;
    float pitch;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }

    void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.up, mx * rotationSpeed * Time.deltaTime);
        pitch = Mathf.Clamp(pitch - my * pitchSpeed * Time.deltaTime, minPitch, maxPitch);
        if (cameraPivot) cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = (transform.forward * v + transform.right * h).normalized;
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);

        Vector3 target = dir * speed;
        if (Input.GetKey(KeyCode.Space)) target += Vector3.up * verticalSpeed;
        if (Input.GetKey(KeyCode.LeftControl)) target += Vector3.down * verticalSpeed;

        currentVel = Vector3.Lerp(currentVel, target, 1 - Mathf.Exp(-acceleration * Time.deltaTime));
        float bob = Mathf.Sin(Time.time * 0.8f) * 0.05f;
        cc.Move((currentVel * Time.deltaTime) + new Vector3(0, bob, 0));
    }
}
