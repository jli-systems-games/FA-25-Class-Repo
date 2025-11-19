using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMP_Text scoreText;
    private int score = 0;

    private void Start()
    {
        scoreText.text = "";
    }

    public void AddScore()
    {
        score++;
        scoreText.text = score.ToString();
    }
}
