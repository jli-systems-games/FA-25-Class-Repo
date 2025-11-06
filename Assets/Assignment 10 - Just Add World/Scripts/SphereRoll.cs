using UnityEngine;
using UnityEngine.SceneManagement;

public class SphereRoll : MonoBehaviour
{
    public float moveSpeed;
    public float rollSpeed;
    public float speedIncrease = 0.2f;

    void Update()
    {
        //Movement
        moveSpeed += speedIncrease * Time.deltaTime;

        Vector3 move = Vector3.forward * moveSpeed * Time.deltaTime;
        transform.position += move;

        //Rotation
        rollSpeed += speedIncrease * Time.deltaTime;
        transform.Rotate(Vector3.right, rollSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Game Over Scene");
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
