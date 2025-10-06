using UnityEngine;
using TMPro;

public class Hud : MonoBehaviour
{
    public static Hud I;

    [Header("UI 引用")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI kissText;

    [Header("倒计时设置")]
    public float startSeconds = 30f;
    public bool autoStart = true;

    int kissCount;
    float timeLeft;
    bool running;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        timeLeft = Mathf.Max(0f, startSeconds);
        running = autoStart;
        UpdateKissLabel();
        UpdateTimerLabel();
    }

    void Update()
    {
        if (!running) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            running = false;
        }
        UpdateTimerLabel();
    }


    public void RegisterKiss(float shownDurationSeconds)
    {
        if (!running) return;
        kissCount++;
        UpdateKissLabel();
    }


    public void ResetStats()
    {
        kissCount = 0;
        timeLeft = Mathf.Max(0f, startSeconds);
        running = true;
        UpdateKissLabel();
        UpdateTimerLabel();
    }

    public void PauseTimer()  { running = false; }
    public void ResumeTimer() { running = true;  }
    public bool IsRunning() => running;

    void UpdateKissLabel()
    {
        if (kissText) kissText.text = $"{kissCount}";
    }

    void UpdateTimerLabel()
    {
        if (!timerText) return;
        int seconds = Mathf.CeilToInt(timeLeft);
        timerText.text = $"{seconds:00}";
    }
}