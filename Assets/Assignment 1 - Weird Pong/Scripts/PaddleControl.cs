using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleControl : MonoBehaviour
{
    public float paddleSpeed = 10f;
    public float verticalLimit = 10f;
    public Ball ball;

    private Vector2 moveInput;
    
    void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        Vector2 paddlePos = transform.position;

        if (ball.hasGameOver)
        {
            paddlePos.y = 0;
            transform.position = paddlePos;
        }
        else 
        {
            paddlePos.y += moveInput.y * paddleSpeed * Time.deltaTime;
            paddlePos.y = Mathf.Clamp(paddlePos.y, -verticalLimit, verticalLimit); //Clamp so that the paddle doesn't go out of bounds
            transform.position = paddlePos;
        }
    }
}
