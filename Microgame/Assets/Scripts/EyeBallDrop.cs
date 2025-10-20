using UnityEngine;
using TMPro;

public class EyeBallDrop : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D eyeballRb;       
    public Transform eyeballStartPoint; 
    public TMP_Text timerText;          
    public TMP_Text feedbackText;       
    [Header("Settings")]
    public float dropForce = 8f;
    public float timeLimit = 4f;
    public float horizontalSpeed = 5f;
    public float minX = -8f; 
    public float maxX = 8f;

    private bool hasDropped = false;
    private float timer = 0f;
    private bool gameEnded = false;

    void Start()
    {
        ResetGame();
    }

    void Update()
    {
        if (gameEnded) return;

       
        timer += Time.deltaTime;
        if (timerText != null)
            timerText.text = "Time: " + (timeLimit - timer).ToString("F2");

        
        if (!hasDropped)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            Vector2 newPos = eyeballRb.position + Vector2.right * moveInput * horizontalSpeed * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            eyeballRb.MovePosition(newPos);
        }

       
        if (timer > timeLimit)
        {
            EndGame(false);
        }

        
        if (!hasDropped && Input.GetKeyDown(KeyCode.Space))
        {
            DropEyeball();
        }
    }

    void DropEyeball()
    {
        hasDropped = true;
        eyeballRb.bodyType = RigidbodyType2D.Dynamic;
        eyeballRb.linearVelocity = Vector2.zero;
        eyeballRb.AddForce(Vector2.down * dropForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameEnded) return;

        if (other.CompareTag("Cauldron"))
        {
            EndGame(true);
        }
    }

    public void EndGame(bool success)
    {
        if (gameEnded) return;
        gameEnded = true;

        if (feedbackText != null)
            feedbackText.text = success ? "Success!" : "Fail!";

       
        if (success)
        {
            Invoke(nameof(GoNext), 1.5f);
        }
        else
        {
            Invoke(nameof(GoFail), 1.5f);
        }
    }

    void GoNext()
    {
        GameManager.Instance.LoadNextGame();
    }

    void GoFail()
    {
        GameManager.Instance.GameOver();
    }

    public void ResetGame()
    {
        hasDropped = false;
        gameEnded = false;
        timer = 0f;

        eyeballRb.bodyType = RigidbodyType2D.Kinematic;
        eyeballRb.linearVelocity = Vector2.zero;
        eyeballRb.angularVelocity = 0f;
        eyeballRb.position = eyeballStartPoint.position;

        if (timerText != null)
            timerText.text = "Time: " + timeLimit.ToString("F2");
        if (feedbackText != null)
            feedbackText.text = "";
    }
}
