using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class TMPCountdownTimer : MonoBehaviour
{
    [Header("Timer")]
    [Tooltip("시작 시간(초)")]
    public float duration = 60f;            // 기본 60초
    [Tooltip("씬이 시작되면 자동으로 타이머 시작")]
    public bool autoStart = true;
    [Tooltip("Time.timeScale의 영향을 받지 않으려면 체크")]
    public bool useUnscaledTime = false;

    [Header("TextMeshPro")]
    [Tooltip("표시할 TMP 텍스트(비우면 같은 오브젝트의 TMP_Text를 자동 사용)")]
    public TMP_Text label;

    [Header("Events")]
    public UnityEvent onTimerEnd;           // 0이 되었을 때 호출(선택)

    float remaining;
    bool running;

    void Awake()
    {
        if (!label) label = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        if (autoStart) StartTimer(duration);
        else
        {
            remaining = Mathf.Max(0f, duration);
            UpdateLabel(remaining);
        }
    }

    void Update()
    {
        if (!running) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        remaining -= dt;

        if (remaining <= 0f)
        {
            remaining = 0f;
            running = false;
            UpdateLabel(remaining);
            onTimerEnd?.Invoke();
            return;
        }

        UpdateLabel(remaining);
    }

    void UpdateLabel(float seconds)
    {
        if (!label) return;
        int s = Mathf.CeilToInt(seconds);         // 0.4초 남아도 1로 보이고 싶으면 Ceil
        int m = s / 60;
        int sec = s % 60;
        label.text = $"{m:00}:{sec:00}";
    }

    // --------- 외부에서 쓰기 좋은 메서드들 ----------
    public void StartTimer(float seconds)  // 특정 시간으로 시작
    {
        duration = Mathf.Max(0f, seconds);
        remaining = duration;
        running = true;
        UpdateLabel(remaining);
    }

    public void StartTimer() => StartTimer(duration); // 현재 duration으로 시작
    public void Pause() { running = false; }
    public void Resume() { if (remaining > 0f) running = true; }
    public void ResetTimer(float seconds) { running = false; StartTimer(seconds); }
    public float GetRemaining() => remaining;
}
