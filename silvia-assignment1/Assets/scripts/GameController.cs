using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;
    public int winScore = 11;

    int leftScore = 0;
    int rightScore = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        UpdateUI();
    }

    public void AddScoreLeft()
    {
        leftScore++;
        UpdateUI();
        CheckWin();
    }

    public void AddScoreRight()
    {
        rightScore++;
        UpdateUI();
        CheckWin();
    }

    void UpdateUI()
    {
        if (leftScoreText) leftScoreText.text = leftScore.ToString();
        if (rightScoreText) rightScoreText.text = rightScore.ToString();
    }

    void CheckWin()
    {
        if (leftScore >= winScore || rightScore >= winScore)
        {
            Time.timeScale = 0f;
            string winner = leftScore > rightScore ? "Left Player Wins!" : "Right Player Wins!";
            Debug.Log(winner);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            leftScore = rightScore = 0;
            UpdateUI();
           
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
