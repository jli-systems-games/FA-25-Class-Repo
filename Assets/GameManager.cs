using TMPro;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public TMP_Text scoreText;

    private int score = 0;
    private int displayScore = 0;
    private Coroutine animRoutine;

    public bool canShoot = false;

    public int correctReward = 1;
    public int wrongPenalty = 3;

    public int CurrentScore => score;
    [HideInInspector] public bool tripleScoreActive = false;


    private void Start()
    {
        score = 0;
        displayScore = 0;
        UpdateScoreText();
    }

    public void AddScore(int amount = 1, bool fromAchievement = false)
    {
        score += amount;
        if (score < 0)
            score = 0;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(ScoreAnim(fromAchievement));
    }

    public void SpendScoreInstant(int amount)
    {
        score -= amount;
        if (score < 0) score = 0;

        displayScore = score;
        UpdateScoreText();

        if (animRoutine != null)
        {
            StopCoroutine(animRoutine);
            animRoutine = null;
        }
    }


    IEnumerator ScoreAnim(bool fast)
    {
        int step = fast ? 50 : 1;
        float delay = fast ? 0.005f : 0.02f;

        while (displayScore != score)
        {
            if (displayScore < score)
            {
                displayScore += step;
                if (displayScore > score)
                    displayScore = score;
            }
            else if (displayScore > score)
            {
                displayScore -= step;
                if (displayScore < score)
                    displayScore = score;
            }

            UpdateScoreText();
            yield return new WaitForSeconds(delay);
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = displayScore.ToString("D6");
    }

    public void AllowShooting()
    {
        canShoot = true;
    }

    public void BlockShooting()
    {
        canShoot = false;
    }

    public void ResetScore()
    {
        score = 0;
        displayScore = 0;

        if (animRoutine != null)
        {
            StopCoroutine(animRoutine);
            animRoutine = null;
        }

        UpdateScoreText();
    }
}
