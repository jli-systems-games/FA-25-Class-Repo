using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float maxTime;
    private Image timerImage;
    private float timeRemaining;

    public bool isTimeOver;
    public bool isTimerRunning = true;

    public GameManager gameManager;

    void Start()
    {
        isTimeOver = false; 

        timeRemaining = maxTime;
        timerImage = GetComponent<Image>();
    }

    void Update()
    {
        if (!isTimerRunning) return; //Stop time when player completed game

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerImage.fillAmount = timeRemaining / maxTime;
            //Code from https://www.youtube.com/watch?v=4g7YY9tLxEE&ab_channel=TheUltimateDeveloper
        }
        else
        {
            Debug.Log("Time ended");
            isTimeOver = true;

            gameManager.LoadRandomGame();
        }

        if (timeRemaining > maxTime * 2 / 3)
        {
            timerImage.color = Color.green;
        }
        else if (timeRemaining > maxTime * 1 / 3)
        {
            timerImage.color = Color.yellow;
        }
        else
        {
            timerImage.color = Color.red;
        }
    }
}
