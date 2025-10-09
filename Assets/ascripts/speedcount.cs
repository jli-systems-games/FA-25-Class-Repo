using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;  
    private int score = 0;
    public GameObject thislevel;
    public GameObject nextlevel;
    public GameObject nextUI;
    public GameObject thisUI;

    void Start()
    {
        UpdateScoreText();
        InvokeRepeating(nameof(DecreaseScore), 1f, 1f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            AddScore(1);
        }
        if (score == 80)
        {
            thislevel.SetActive(false);
            nextlevel.SetActive(true);
            thisUI.SetActive(false);
            nextUI.SetActive(true);
        }
    }

    void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    void DecreaseScore()
    {
        score = Mathf.Max(0, score - 1);
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Speed: " + score.ToString() + "/km";
    }
}
