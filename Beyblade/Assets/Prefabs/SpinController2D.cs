using UnityEngine;

public class SpinController2D : MonoBehaviour
{
    public float spinSpeed = 300f;

    private void Update()
    {
        transform.Rotate(0f, 0f, -spinSpeed * Time.deltaTime);
    }
}
