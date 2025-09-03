using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;

    [Header("Rules")]
    public int winScore = 11;

    int leftScore = 0;
    int rightScore = 0;
    bool gameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        UpdateUI();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void OnValidate()
    {
        if (winScore < 1) winScore = 1;
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
        SafeSetText(leftScoreText, leftScore.ToString());
        SafeSetText(rightScoreText, rightScore.ToString());
    }

    static void SafeSetText(TextMeshProUGUI label, string content)
    {
        if (label != null) label.SetText(content);
    }

    void CheckWin()
    {
        if (gameOver) return;

        if (leftScore >= winScore || rightScore >= winScore)
        {
            gameOver = true;
            Time.timeScale = 0f;
            AnnounceWinner();
        }
    }

    void AnnounceWinner()
    {
        string winner = leftScore > rightScore ? "Left Player Wins!" : "Right Player Wins!";
        Debug.Log(winner);
    }

    void ResetGame()
    {
        Time.timeScale = 1f;
        leftScore = 0;
        rightScore = 0;
        gameOver = false;
        UpdateUI();
    }

    void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.R))
{
    Time.timeScale = 1f;
    leftScore = rightScore = 0;
    UpdateUI();

    var overlay = FindObjectOfType<StartOverlayAuto>(true);
    if (overlay)
    {
        overlay.ShowThenHide(1.5f);
    }
    else
    {
        var b = FindObjectOfType<Ball>();
        if (b) b.LaunchRandom();
    }
}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
