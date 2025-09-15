using Unity.VisualScripting;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float rotationSpeed;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
