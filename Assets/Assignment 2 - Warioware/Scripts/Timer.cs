using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float maxTime;
    private Image timerImage;
    private float timeRemaining;

    void Start()
    {
        timeRemaining = maxTime;
        timerImage = GetComponent<Image>();
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerImage.fillAmount = timeRemaining / maxTime;
            //Code from https://www.youtube.com/watch?v=4g7YY9tLxEE&ab_channel=TheUltimateDeveloper
        }
        else
        {
            Debug.Log("Time ended");
        }

        if (timeRemaining > maxTime * 2 / 3)
        {
            timerImage.color = new Color(186, 255, 95);
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
