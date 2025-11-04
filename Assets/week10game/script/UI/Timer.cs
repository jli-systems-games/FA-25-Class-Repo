using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [Header("Refs")]
    public TimerController timer;   // GameManager에 연결된 그 TimerController
    public Slider timerSlider;
    public TextMeshProUGUI timerText;

    float roundDuration;

    void Start()
    {
        if (timer == null || timerSlider == null || timerText == null) return;

        // 라운드 시작 시점의 전체 시간 기억
        roundDuration = GameManager.Instance != null ? GameManager.Instance.roundTimeSeconds : timer.GetTimeLeft();

        // 초기화
        timerSlider.minValue = 0f;
        timerSlider.maxValue = roundDuration;

        // 이벤트 연결
        timer.OnTimeChanged += OnTimeChanged;
        OnTimeChanged(timer.GetTimeLeft()); // 즉시 1회 갱신
    }

    void OnDestroy()
    {
        if (timer != null) timer.OnTimeChanged -= OnTimeChanged;
    }

    void OnTimeChanged(float timeLeft)
    {
        // 슬라이더는 남은 시간이 줄어드는 모션을 보여주고 싶으면:
        timerSlider.value = timeLeft;                    // (max가 roundDuration이면 왼쪽으로 줄어듦)
        timerText.text = $"{timeLeft:0.0}s";            // 소수 1자리
    }
}
