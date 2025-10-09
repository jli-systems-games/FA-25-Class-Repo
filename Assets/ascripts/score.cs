using UnityEngine;
using TMPro;

public class ClickScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int score = 0;
    public GameObject nextlevel;
    public GameObject nextUI;
    public GameObject thislevel;
    public GameObject thisUI;
    public GameObject trainw;
    void Start()
    {
        UpdateScoreText();
    }

    void Update()
    {
        if(score == 100)
        {
            nextlevel.SetActive(true);
            nextUI.SetActive(true);
            thislevel.SetActive(false);
            thisUI.SetActive(false);
            trainw.SetActive(true);
        }
    }

    public void AddScore()
    {
        score += 20;
        UpdateScoreText();
        Debug.Log("Score: " + score);
    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString() + "%";
    }
}
