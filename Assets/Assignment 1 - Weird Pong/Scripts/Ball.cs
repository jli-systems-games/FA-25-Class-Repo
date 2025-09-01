using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
    //Ball
    public Rigidbody2D ballrb;
    public float ballSpeed = 10f;
    public float maxAngle = 0.5f;

    //Score
    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;
    public int winningPoint = 3;

    private int leftScore = 0;
    private int rightScore = 0;
    private bool rightWon = false;
    private bool leftWon = false;

    //Game Over
    public Canvas gameOverLeftCanvas;
    public Canvas gameOverRightCanvas;

    //Paddles


    void Start()
    {
        gameOverLeftCanvas.gameObject.SetActive(false);
        gameOverRightCanvas.gameObject.SetActive(false);
        InitialBallMovement();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOverLeftCanvas == true || gameOverRightCanvas == true)
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                ResetGame();
            }
        }
    }

    void ResetGame()
    {
        gameOverLeftCanvas.gameObject.SetActive(false);
        gameOverRightCanvas.gameObject.SetActive(false);

        rightScore = 0;
        rightScoreText.text = rightScore.ToString();
        leftScore = 0;
        leftScoreText.text = leftScore.ToString();

        InitialBallMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Left"))
        {
            rightScore += 1;
            rightScoreText.text = rightScore.ToString();

            rightWon = true;
            leftWon = false;

            CheckScores();
        }
        else if (collision.CompareTag("Right"))
        {
            leftScore += 1;
            leftScoreText.text = leftScore.ToString();

            leftWon = true;
            rightWon= false;

            CheckScores();
        }
    }

    void CheckScores()
    {
        if (leftScore == winningPoint)
        {
            gameOverLeftCanvas.gameObject.SetActive(true);
        }
        else if (rightScore == winningPoint)
        {
            gameOverRightCanvas.gameObject.SetActive(true);
        }
        else
        {
            ResetBall();
        }
    }

    void ResetBall()
    {
        transform.position = new Vector2(0, 0);

        if (leftWon)
        {
            StartLeft();
        }
        else if (rightWon)
        {
            StartRight();
        }
    }

    void StartLeft()
    {
        Vector2 ballDirection = Vector2.left;
        ballDirection.y = Random.Range(-maxAngle, maxAngle);
        ballrb.linearVelocity = ballDirection * ballSpeed;
    }

    void StartRight()
    {
        Vector2 ballDirection = Vector2.right;
        ballDirection.y = Random.Range(-maxAngle, maxAngle);
        ballrb.linearVelocity = ballDirection * ballSpeed;
    }

    void InitialBallMovement()
    {
        transform.position = new Vector2(0, 0);
        StartLeft(); //randomize start left or right
    }
}
