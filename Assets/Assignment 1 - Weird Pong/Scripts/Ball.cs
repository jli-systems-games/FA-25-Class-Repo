using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class Ball : MonoBehaviour
{
    //Ball
    public Rigidbody2D ballrb;
    public float ballSpeed = 10f;
    public float maxAngle = 0.5f;
    public float minAngle = 0.2f;
    public float rotationSpeed = 50f;
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

    //Audio
    public AudioClip pingAudio;
    public AudioClip tennisAudio;
    public AudioClip basketAudio;
    public AudioClip bowlAudio;
    public AudioClip rugbyAudio;
    public AudioClip shuttleAudio;
    public AudioClip beachAudio;
    public AudioClip poolAudio;
    private AudioClip ballAudio;
    [Space(10)]

    //Score
    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;
    public int winningPoint = 3;

    private int leftScore = 0;
    private int rightScore = 0;
    private bool rightWon = false;
    private bool leftWon = false;
    private bool isInPlay = false;
    [Space(10)]

    //Game Over
    public Canvas gameOverLeftCanvas;
    public Canvas gameOverRightCanvas;
    public bool hasGameOver = false;
    [Space(10)]

    public Canvas scoreCanvas;
    public Canvas startCanvas;

    void Start()
    {
        gameOverLeftCanvas.gameObject.SetActive(false);
        gameOverRightCanvas.gameObject.SetActive(false);
        scoreCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameOverLeftCanvas == true || gameOverRightCanvas == true)
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                ResetGame();
            }
        }

        if(startCanvas)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                startCanvas.gameObject.SetActive(false);
                scoreCanvas.gameObject.SetActive(true);
                InitialBallMovement();
            }
        }

        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime); //Rotate Ball
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
            int randomBall = Random.Range(0, 20);

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();

            string ballName;

            if (!hasChangedSprite && isInPlay) //Make sure ball doesn't change numerous times when it's on the middle line for serveal frames
            {
                if (randomBall == 0) //Bowling Ball
                {
                    ballName = "Bowling";

                    ballSpeed = 4;
                    spriteRenderer.sprite = bowlSprite;
                    transform.localScale = Vector3.one * 0.044f;
                    circleCollider.enabled = true;
                    capsuleCollider.enabled = false;
                    circleCollider.radius = 18.4f;
                    ballAudio = bowlAudio;

                    hasChangedSprite = true;
                }
                else if (randomBall == 1) //Beach Ball
                {
                    ballName = "Beach";

                    ballSpeed = 5;
                    spriteRenderer.sprite = beachSprite;
                    transform.localScale = Vector3.one * 0.08f;
                    circleCollider.enabled = true;
                    capsuleCollider.enabled = false;
                    circleCollider.radius = 18.4f;
                    ballAudio = beachAudio;

                    hasChangedSprite = true;
                }
                else if (randomBall == 2 || randomBall == 3) //Rugby Ball
                {
                    ballName = "Rugby";

                    ballSpeed = 6;
                    spriteRenderer.sprite = rugbySprite;
                    transform.localScale = Vector3.one * 0.038f;
                    circleCollider.enabled = false;
                    capsuleCollider.enabled = true;
                    capsuleCollider.size = new Vector2(66.7f, 35.4f);
                    ballAudio = rugbyAudio;

                    hasChangedSprite = true;
                }
                else if (randomBall == 4 || randomBall == 5) //Pool Ball
                {
                    ballName = "Pool";

                    ballSpeed = 7;
                    spriteRenderer.sprite = poolSprite;
                    transform.localScale = Vector3.one * 0.0114f;
                    circleCollider.enabled = true;
                    capsuleCollider.enabled = false;
                    circleCollider.radius = 18.4f;
                    ballAudio = poolAudio;

                    hasChangedSprite = true;
                }
                else if (randomBall >= 6 && randomBall <= 8) //Basketball
                {
                    ballName = "Basket";

                    ballSpeed = 8;
                    spriteRenderer.sprite = basketSprite;
                    transform.localScale = Vector3.one * 0.048f;
                    circleCollider.enabled = true;
                    capsuleCollider.enabled = false;
                    circleCollider.radius = 18.4f;
                    ballAudio = basketAudio;

                    hasChangedSprite = true;
                }
                else if (randomBall >= 9 && randomBall <= 11) //Shuttlecock
                {
                    ballName = "Shuttle";

                    ballSpeed = 10;
                    spriteRenderer.sprite = shuttleSprite;
                    transform.localScale = Vector3.one * 0.018f;
                    circleCollider.enabled = false;
                    capsuleCollider.enabled = true;
                    capsuleCollider.size = new Vector2(43.7f, 35.4f);
                    ballAudio = shuttleAudio;

                    hasChangedSprite = true;
                }
                else if (randomBall >= 12 && randomBall <= 14) //Tennis Ball
                {
                    ballName = "Tennis";

                    ballSpeed = 11;
                    spriteRenderer.sprite = tennisSprite;
                    transform.localScale = Vector3.one * 0.0134f;
                    circleCollider.enabled = true;
                    capsuleCollider.enabled = false;
                    circleCollider.radius = 19.82f;
                    ballAudio = tennisAudio;

                    hasChangedSprite = true;
                }
                else //Ping Pong Ball
                {
                    ballName = "Ping";

                    ballSpeed = 12;
                    spriteRenderer.sprite = pingSprite;
                    transform.localScale = Vector3.one * 0.008f;
                    circleCollider.enabled = true;
                    capsuleCollider.enabled = false;
                    circleCollider.radius = 18.4f;
                    ballAudio = pingAudio;

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
            isInPlay = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = ballAudio;
        audioSource.Play();
    }

    void CheckScores()
    {
        isInPlay = false;

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
        isInPlay = true;

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
        ballAudio = pingAudio;

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
