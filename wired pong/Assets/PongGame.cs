using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PongGame : MonoBehaviour
{
    
    public GameObject leftPaddle;
    public GameObject rightPaddle;
    public float paddleSpeed = 5f;
    public float paddleMoveLimit = 5f;

  
    public GameObject ball;
    public float ballSpeed = 5f;
    public Vector2 initialBallDirection = new Vector2(1f, 0f);

   
    public GameObject leftWall;     
    public GameObject rightWall;  
    public GameObject topWall;
    public GameObject bottomWall;
    public GameObject leftGoal;      
    public GameObject rightGoal;     


    public TMP_Text leftScoreText;
    public TMP_Text rightScoreText;

   
    [Range(0f, 1f)] public float verticalInfluence = 0.5f;

 
    private int leftScore = 0;
    private int rightScore = 0;

    
    [HideInInspector] public float leftPaddleInput;
    [HideInInspector] public float rightPaddleInput;
    private Rigidbody2D ballRigidbody;
    private bool isGameRunning = false;

    void Start()
    {
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
        ballRigidbody.gravityScale = 0f;
        ballRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        InitializeScores();
        ResetBall();
    }

    void Update()
    {
        MovePaddles();
        HandleReset();

        if (!isGameRunning && Keyboard.current.spaceKey.wasPressedThisFrame)
            StartBall();
    }

    void MovePaddles()
    {
        Vector3 lp = leftPaddle.transform.position;
        lp.y = Mathf.Clamp(lp.y + leftPaddleInput * paddleSpeed * Time.deltaTime, -paddleMoveLimit, paddleMoveLimit);
        leftPaddle.transform.position = lp;

        Vector3 rp = rightPaddle.transform.position;
        rp.y = Mathf.Clamp(rp.y + rightPaddleInput * paddleSpeed * Time.deltaTime, -paddleMoveLimit, paddleMoveLimit);
        rightPaddle.transform.position = rp;
    }

    void InitializeScores()
    {
        leftScore = 0;
        rightScore = 0;
        UpdateScore();
    }

    void UpdateScore()
    {
        if (leftScoreText != null) leftScoreText.text = leftScore.ToString();
        if (rightScoreText != null) rightScoreText.text = rightScore.ToString();
    }

    void ResetBall()
    {
        ball.transform.position = Vector2.zero;
        ballRigidbody.linearVelocity = Vector2.zero;
        isGameRunning = false;
    }

    void StartBall()
    {
        
        Vector2 dir = initialBallDirection.normalized;
        dir.y += Random.Range(-0.2f, 0.2f);
        ballRigidbody.linearVelocity = dir.normalized * ballSpeed;
        isGameRunning = true;
    }

    
    public void AddLeftScore() { leftScore++; UpdateScore(); }
    public void AddRightScore() { rightScore++; UpdateScore(); }

    public void ResetAndStartNewRound()
    {
        ResetBall();
        Invoke(nameof(StartBall), 1f);
    }
    // ========================================

    
    public void OnLeftPaddleMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        => leftPaddleInput = ctx.ReadValue<float>();

    public void OnRightPaddleMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        => rightPaddleInput = ctx.ReadValue<float>();

    void HandleReset()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            InitializeScores();
            ResetBall();
        }
    }
}
