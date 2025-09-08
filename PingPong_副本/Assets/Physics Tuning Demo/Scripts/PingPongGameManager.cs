using TMPro;
using UnityEngine;

public class PingPongGameManager : MonoBehaviour
{
    [Header("Score")]
    public TextMeshProUGUI scoreUI;
    private int score = 0;

    [Header("Ball")]
    public Rigidbody ball;
    private Vector3 ballStartPos;

    private void Start()
    {
        ballStartPos = ball.transform.position;
        SetCursorVisibility(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetBall();
            ResetScore();
        }
        if (Input.GetMouseButtonDown(0))
            SetCursorVisibility(false);
        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursorVisibility(true);
    }

    private void SetCursorVisibility(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void IncrementScore()
    {
        score += 1;
        scoreUI.text = score + "";
    }

    public void ResetScore()
    {
        score = 0;
        scoreUI.text = score + "";
    }

    private void ResetBall()
    {
        ball.transform.position = ballStartPos;
        ball.transform.rotation = Quaternion.identity;
        ball.linearVelocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
    }
}
