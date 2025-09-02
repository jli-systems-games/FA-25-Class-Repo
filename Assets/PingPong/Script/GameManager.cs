using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Ball ball;
    public Paddle playerPaddle;
    public Paddle computerPaddle;

    public TMPro.TMP_Text playerScoreText;
    public TMPro.TMP_Text computerScoreText;

    private int _playerScore;

    private int _computerScore;

    public void PlayerScores()
    {
        _playerScore++;
        Debug.Log(_playerScore);

        this.playerScoreText.text = _playerScore.ToString();
        ResetRound();
    }
    public void ComputerScores()
    {
        _computerScore++;
        Debug.Log(_computerScore);

        this.computerScoreText.text = _computerScore.ToString();
        ResetRound();
    }
    private void ResetRound()
    {
        this.playerPaddle.ResetPosition();
        this.computerPaddle.ResetPosition();
        this.ball.ResetPosition();
        this.ball.AddStartingForce();
    }
}


