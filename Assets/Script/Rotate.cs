using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotateSpeed = 50f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}