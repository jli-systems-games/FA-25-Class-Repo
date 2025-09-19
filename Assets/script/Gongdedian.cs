using UnityEngine;
using TMPro;

public class ClickScoreboardTMP : MonoBehaviour
{
    public TMP_Text scoreText;    
  
    private int score = 0;

    void Start()
    {
        UpdateScoreUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            score++; // add 1
            UpdateScoreUI();

  
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Moral: " + score;
    }
}

