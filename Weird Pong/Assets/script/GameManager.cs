using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [Header("Ball")]
    public GameObject ball;
    public GameObject ball2;
    public GameObject ball3;
    public GameObject ball4;
    [Header("Player1")]
    public GameObject playerOne;
    public GameObject player1Goal;

    [Header("Player2")]
    public GameObject playerTwo;
    public GameObject player2Goal;

    [Header("Score UI")]
    public GameObject Player1Text;
    public GameObject Player2Text;

    private int Player1Score;
    private int Player2Score;

    public void Player1Scored()
    {
        Player1Score++;
        Player1Text.GetComponent<TextMeshProUGUI>().text = Player1Score.ToString();
        ResetPosition();
    }
    public void Player2Scored()
    {
        Player2Score++;
        Player2Text.GetComponent<TextMeshProUGUI>().text = Player2Score.ToString();
        ResetPosition();
    }
    private void ResetPosition()
    {
        if(ball.gameObject.activeInHierarchy)
        ball.GetComponent<ball>().Reset();
        else if (ball2.gameObject.activeInHierarchy)
        ball2.GetComponent<ball>().Reset();
        else if (ball3.gameObject.activeInHierarchy)
            ball3.GetComponent<ball>().Reset();
        else if (ball4.gameObject.activeInHierarchy)
            ball4.GetComponent<ball>().Reset();
    }
    public class RestartScene : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
