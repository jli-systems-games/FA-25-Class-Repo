using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    void Update()
    {
        transform.Translate(-Vector3.forward * Data.groundMovementSpeed * Time.deltaTime);
    }
}
