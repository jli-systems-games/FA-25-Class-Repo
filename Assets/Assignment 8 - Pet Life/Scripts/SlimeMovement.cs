using UnityEngine;

public class SlimeMovement : MonoBehaviour
{
    public float movementRange = 1.85f;
    public float movementSpeed = 1.5f;

    void Update()
    {
        MoveSlimeSideToSide();
    }

    void MoveSlimeSideToSide()
    {
        float pingPongValue = Mathf.PingPong(Time.time * movementSpeed, movementRange * 2f);
        float newX = pingPongValue - movementRange;

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);
        transform.position = newPosition;
    }
}
