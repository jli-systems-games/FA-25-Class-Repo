using UnityEngine;
using UnityEngine.InputSystem;


public class Toggle : MonoBehaviour
{
    public float rotationSpeed = 40f;
    private float rotateInput;

    public void OnRotate(InputAction.CallbackContext context)
    {
        rotateInput = context.ReadValue<float>();
    }

    void Update()
    {
        transform.Rotate(0f, 0f, rotateInput * rotationSpeed * Time.deltaTime);
    }
}
