using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
    //Ball
    public Rigidbody2D ballrb;
    public float ballSpeed = 10f;
    public float maxAngle = 0.5f;
    public float minAngle = 0.2f;
    public float speedMultiplier = 1.1f;
    [Space(10)]

    //Ball Sprites
    public Sprite pingSprite;
    public Sprite tennisSprite;
    public Sprite basketSprite;
    public Sprite bowlSprite;
    public Sprite rugbySprite;
    public Sprite shuttleSprite;
    public Sprite beachSprite;
    public Sprite poolSprite;

    private bool hasChangedSprite = false;
    [Space(10)]

    //Score
    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;
    public int winningPoint = 3;

    private int leftScore = 0;
    private int rightScore = 0;
    private bool rightWon = false;
    private bool leftWon = false;
    [Space(10)]

    //Game Over
    public Canvas gameOverLeftCanvas;
    public Canvas gameOverRightCanvas;
    public bool hasGameOver = false;

    void Start()
    {
        gameOverLeftCanvas.gameObject.SetActive(false);
        gameOverRightCanvas.gameObject.SetActive(false);

        InitialBallMovement();
    }

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
        hasGameOver = false;
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
        //Earn points when ball leaves on the sides
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

        //Ball randomly changes when it reaches the middle line
        if (collision.CompareTag("Middle Line"))
        {
            int randomBall = Random.Range(0, 9);

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            string ballName;

            if (!hasChangedSprite) //Make sure ball doesn't change numerous times when it's on the middle line for serveal frames
            {
                if (randomBall == 0) //Bowling Ball
                {
                    ballName = "Bowling";

                    ballSpeed = 3;
                    spriteRenderer.sprite = bowlSprite;
                    transform.localScale = Vector3.one * 0.044f;

                    hasChangedSprite = true;
                }
                else if (randomBall == 1) //Beach Ball
                {
                    ballName = "Beach";

                    ballSpeed = 4;
                    spriteRenderer.sprite = beachSprite;
                    transform.localScale = Vector3.one * 0.08f;

                    hasChangedSprite = true;
                }
                else if (randomBall == 2) //Rugby Ball
                {
                    ballName = "Rugby";

                    ballSpeed = 5;
                    spriteRenderer.sprite = rugbySprite;
                    transform.localScale = Vector3.one * 0.06f;

                    hasChangedSprite = true;
                }
                else if (randomBall == 3) //Pool Ball
                {
                    ballName = "Pool";

                    ballSpeed = 6;
                    spriteRenderer.sprite = poolSprite;
                    transform.localScale = Vector3.one * 0.0114f;

                    hasChangedSprite = true;
                }
                else if (randomBall == 4) //Basketball
                {
                    ballName = "Basket";

                    ballSpeed = 7;
                    spriteRenderer.sprite = basketSprite;
                    transform.localScale = Vector3.one * 0.048f;

                    hasChangedSprite = true;
                }
                else if (randomBall == 5) //Shuttlecock
                {
                    ballName = "Shuttle";

                    ballSpeed = 9;
                    spriteRenderer.sprite = shuttleSprite;
                    transform.localScale = Vector3.one * 0.012f;

                    hasChangedSprite = true;
                }
                else if (randomBall == 6) //Tennis Ball
                {
                    ballName = "Tennis";

                    ballSpeed = 10;
                    spriteRenderer.sprite = tennisSprite;
                    transform.localScale = Vector3.one * 0.0134f;

                    hasChangedSprite = true;
                }
                else //Ping Pong Ball
                {
                    ballName = "Ping";

                    ballSpeed = 12;
                    spriteRenderer.sprite = pingSprite;
                    transform.localScale = Vector3.one * 0.008f;

                    hasChangedSprite = true;
                }

                Debug.Log(ballSpeed + ballName);

                Vector2 currentDirection = ballrb.linearVelocity.normalized;
                ballrb.linearVelocity = currentDirection * ballSpeed;
            }
        }

        if (collision.CompareTag("Exit Boundary"))
        {
            hasChangedSprite = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Ball speed increases everytime it hits the paddle
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ballrb.linearVelocity *= speedMultiplier;
        }
    }

    void CheckScores()
    {
        if (leftScore == winningPoint)
        {
            hasGameOver = true;
            gameOverLeftCanvas.gameObject.SetActive(true);
        }
        else if (rightScore == winningPoint)
        {
            hasGameOver = true;
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

        int randomHeight = Random.Range(0, 2);

        //Make sure ball starts off at a reasonable random angle (not straight either)
        if (randomHeight == 0)
        {
            ballDirection.y = Random.Range(-minAngle, -maxAngle);
        }
        else
        {
            ballDirection.y = Random.Range(maxAngle, minAngle);
        }

        ballrb.linearVelocity = ballDirection * ballSpeed;
    }

    void StartRight()
    {
        Vector2 ballDirection = Vector2.right;

        int randomHeight = Random.Range(0, 2);

        if (randomHeight == 0)
        {
            ballDirection.y = Random.Range(-minAngle, -maxAngle);
        }
        else
        {
            ballDirection.y = Random.Range(maxAngle, minAngle);
        }

        ballrb.linearVelocity = ballDirection * ballSpeed;
    }

    void InitialBallMovement()
    {
        transform.position = new Vector2(0, 0);

        int randomStart = Random.Range(0, 2);

        if (randomStart == 0)
        {
            StartLeft();
        }
        else
        {
            StartRight();
        }
    }
}
