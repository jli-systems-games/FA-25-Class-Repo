using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshPro bombTimerText;
    public float currentTime;
    public AudioSource audioSource;

    private bool hasEnded = false;

    private int previousSecond; 

    void Start()
    {
        previousSecond = Mathf.FloorToInt(currentTime);
    }

    void Update()
    {
        if (!hasEnded)
        {
            currentTime -= Time.deltaTime;

            int currentSecond = Mathf.FloorToInt(currentTime);

            if (currentSecond < previousSecond && currentSecond >= 0)
            {
                audioSource.Play();
                previousSecond = currentSecond;
            }

            if (currentTime <= 0)
            {
                currentTime = 0;
                hasEnded = true;
            }

            int seconds = Mathf.FloorToInt(currentTime);
            timerText.text = string.Format("00.00.{0:D2}", seconds);
            bombTimerText.text = string.Format("00.00.{0:D2}", seconds);
            //Timer code from https://www.youtube.com/watch?v=POq1i8FyRyQ
        }
    }
}
