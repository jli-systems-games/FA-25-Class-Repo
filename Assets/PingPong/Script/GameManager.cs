using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Ball ball;
    public Paddle playerPaddle;
    public Paddle computerPaddle;

    public TMP_Text playerScoreText;
    public TMP_Text computerScoreText;

    public int winningScore = 15;
    public Image playerWinImage;
    public Image computerWinImage;

    private int _playerScore;
    private int _computerScore;
    private bool _gameOver = false;

    private void Start()
    {
        if (playerWinImage) playerWinImage.gameObject.SetActive(false);
        if (computerWinImage) computerWinImage.gameObject.SetActive(false);

        if (playerScoreText) playerScoreText.text = "0";
        if (computerScoreText) computerScoreText.text = "0";
    }

    private void Update()
    {
        if (_gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            NewGame();
        }
    }

    public void PlayerScores()
    {
        if (_gameOver) return;

        _playerScore++;
        Debug.Log(_playerScore);
        if (playerScoreText) playerScoreText.text = _playerScore.ToString();

        if (_playerScore >= winningScore) EndGame(true);
        else ResetRound();
    }

    public void ComputerScores()
    {
        if (_gameOver) return;

        _computerScore++;
        Debug.Log(_computerScore);
        if (computerScoreText) computerScoreText.text = _computerScore.ToString();

        if (_computerScore >= winningScore) EndGame(false);
        else ResetRound();
    }

    private void ResetRound()
    {
        if (_gameOver) return;

        playerPaddle.ResetPosition();
        computerPaddle.ResetPosition();
        ball.ResetPosition();
        ball.AddStartingForce();
    }

    private void EndGame(bool isPlayerWinner)
    {
        _gameOver = true;

        if (isPlayerWinner)
        {
            if (playerWinImage) playerWinImage.gameObject.SetActive(true);
        }
        else
        {
            if (computerWinImage) computerWinImage.gameObject.SetActive(true);
        }

        StopAndFreeze(ball ? ball.GetComponent<Rigidbody2D>() : null, ball ? ball.gameObject : null);
        StopAndFreeze(playerPaddle ? playerPaddle.GetComponent<Rigidbody2D>() : null, playerPaddle ? playerPaddle.gameObject : null);
        StopAndFreeze(computerPaddle ? computerPaddle.GetComponent<Rigidbody2D>() : null, computerPaddle ? computerPaddle.gameObject : null);
    }

    private void StopAndFreeze(Rigidbody2D rb, GameObject go)
    {
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (go)
        {
            var ballComp = go.GetComponent<Ball>();
            if (ballComp) ballComp.enabled = false;

            var paddleComp = go.GetComponent<Paddle>();
            if (paddleComp) paddleComp.enabled = false;
        }
    }

    private void NewGame()
    {
        _gameOver = false;

        _playerScore = 0;
        _computerScore = 0;
        if (playerScoreText) playerScoreText.text = "0";
        if (computerScoreText) computerScoreText.text = "0";

        if (playerWinImage) playerWinImage.gameObject.SetActive(false);
        if (computerWinImage) computerWinImage.gameObject.SetActive(false);

        UnfreezeAndEnable(ball ? ball.GetComponent<Rigidbody2D>() : null, ball ? ball.gameObject : null);
        UnfreezeAndEnable(playerPaddle ? playerPaddle.GetComponent<Rigidbody2D>() : null, playerPaddle ? playerPaddle.gameObject : null);
        UnfreezeAndEnable(computerPaddle ? computerPaddle.GetComponent<Rigidbody2D>() : null, computerPaddle ? computerPaddle.gameObject : null);

        playerPaddle.ResetPosition();
        computerPaddle.ResetPosition();
        ball.ResetPosition();
        ball.AddStartingForce();
    }

    private void UnfreezeAndEnable(Rigidbody2D rb, GameObject go)
    {
        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (go)
        {
            var ballComp = go.GetComponent<Ball>();
            if (ballComp) ballComp.enabled = true;

            var paddleComp = go.GetComponent<Paddle>();
            if (paddleComp) paddleComp.enabled = true;
        }
    }
}
