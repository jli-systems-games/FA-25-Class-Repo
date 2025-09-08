using UnityEngine;

public class FP2DAim : MonoBehaviour
{
    public Transform gunPivot;   // мо GunPivot
    public Camera cam;           // мо Main Camera
    public float sensitivity = 120f;
    public float minAngle = -10f, maxAngle = 40f;

    float angleZ;

    void Start()
    {
        if (!cam) cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!gunPivot) return;
        float dy = Input.GetAxis("Mouse Y");
        angleZ += -dy * sensitivity * Time.deltaTime;
        angleZ = Mathf.Clamp(angleZ, minAngle, maxAngle);
        gunPivot.localRotation = Quaternion.Euler(0, 0, angleZ);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
