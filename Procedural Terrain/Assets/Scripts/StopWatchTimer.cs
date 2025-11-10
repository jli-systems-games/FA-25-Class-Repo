using UnityEngine;
using TMPro;

public class StopwatchTimer : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text timerText;      
    public bool isRunning = true;

    private float elapsedTime = 0f;

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);

        timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

   
    public void StopTimer()
    {
        isRunning = false;
    }

    
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}

