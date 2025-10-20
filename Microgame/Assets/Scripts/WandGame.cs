using UnityEngine;
using TMPro;

public class WandGame : MonoBehaviour
{
    [Header("Assign visible point objects in order")]
    public GameObject[] tracePoints;

    [Header("UI Elements")]
    public TMP_Text timerText;
    public TMP_Text feedbackText;

    [Header("Settings")]
    public float timeLimit = 5f;

    private int currentPoint = 0;
    private float timer;
    private bool gameEnded = false;

    void Start()
    {
        timer = timeLimit;
        feedbackText.text = "";
        UpdatePointVisibility();
    }

    void Update()
    {
        if (gameEnded) return;

        timer -= Time.deltaTime;
        timerText.text = timer.ToString("F1");

        if (timer <= 0)
        {
            EndGame(false);
        }
    }

    public void ClickPoint(int pointIndex)
    {
        if (gameEnded) return;

        if (pointIndex == currentPoint)
        {
            currentPoint++;
            UpdatePointVisibility();

            if (currentPoint >= tracePoints.Length)
            {
                EndGame(true);
            }
        }
        else
        {
            EndGame(false);
        }
    }

    void UpdatePointVisibility()
    {
        for (int i = 0; i < tracePoints.Length; i++)
        {
            tracePoints[i].SetActive(i == currentPoint);
        }
    }

    void EndGame(bool success)
    {
        gameEnded = true;

        if (success)
        {
            feedbackText.text = "Spell Cast!";
            GameManager.Instance.LoadNextGame();
        }
        else
        {
            feedbackText.text = "Spell Failed!";
            GameManager.Instance.GameOver();
        }
    }
}
