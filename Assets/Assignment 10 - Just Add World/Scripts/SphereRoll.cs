using UnityEngine;

public class SphereRoll : MonoBehaviour
{
    public float moveSpeed;
    public float rollSpeed;

    void Update()
    {
        //Movement
        Vector3 move = Vector3.forward * moveSpeed * Time.deltaTime;
        transform.position += move;

        //Rotation
        transform.Rotate(Vector3.right, rollSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hallway"))
        {
            GameObject currentHallway = other.gameObject;
            Destroy(currentHallway);
        }
    }
}
