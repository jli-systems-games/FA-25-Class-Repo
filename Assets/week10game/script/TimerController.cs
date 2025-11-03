using UnityEngine;
using System;
using static Readme;

public class TimerController : MonoBehaviour
{
    float timeLeft = 0f;
    bool running = false;

    public Action<float> OnTimeChanged; // UI에서 받고 싶으면 구독
    public Action OnTimeEnd;            // 끝났을 때

    public void StartTimer(float seconds)
    {
        timeLeft = seconds;
        running = true;
        OnTimeChanged?.Invoke(timeLeft);
    }

    public void StopTimer()
    {
        running = false;
    }

    void Update()
    {
        if (!running) return;

        timeLeft -= Time.deltaTime;
        OnTimeChanged?.Invoke(timeLeft);

        if (timeLeft <= 0f)
        {
            running = false;
            OnTimeEnd?.Invoke();
            GameManager.Instance.OnTimeOver();
        }
    }

    public float GetTimeLeft() => timeLeft;
}
