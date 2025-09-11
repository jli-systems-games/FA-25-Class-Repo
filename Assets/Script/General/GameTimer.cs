using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CountdownTimer : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;                  // 显示倒计时的 Slider（必填）

    [Header("Timing")]
    [Min(0.01f)] public float duration = 5f;   // 总时长（秒）
    public bool startOnEnable = false;         // 脚本启用时是否自动开始
    public bool useUnscaledTime = false;       // 是否用 unscaledDeltaTime（不受 Time.timeScale 影响）
    public bool showRemaining = true;          // true: slider 显示“剩余”；false: 显示“已用”

    [Header("On Finish Actions")]
    public GameObject objectToEnable;          // 时间结束时要启用的物体（可选）
    public Animator targetAnimator;            // 时间结束时要触发的 Animator（可选）
    public string triggerName;                 // 触发的 Trigger 名（可选）
    public bool disableSelfAfterFinish = false;// 结束后把计时器对象自己关掉（可选）

    [Header("Events")]
    public UnityEvent onStarted;
    public UnityEvent<float> onTick;           // 每帧把“剩余时间”回调出去
    public UnityEvent onFinished;

    // 运行时
    public bool IsRunning { get; private set; }
    public float Remaining { get; private set; }
    public float Elapsed => Mathf.Clamp(duration - Remaining, 0f, duration);
    public float Normalized01 => duration <= 0f ? 1f : Mathf.Clamp01(Elapsed / duration);

    void OnEnable()
    {
        // 初始化 slider 边界
        if (slider)
        {
            slider.minValue = 0f;
            slider.maxValue = duration;
            slider.value = showRemaining ? duration : 0f;
        }

        if (startOnEnable)
            StartTimer(duration);
    }

    void Update()
    {
        if (!IsRunning) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        Remaining -= dt;

        if (slider)
            slider.value = showRemaining ? Mathf.Max(0f, Remaining)
                                         : Mathf.Min(duration, duration - Mathf.Max(0f, Remaining));

        onTick?.Invoke(Mathf.Max(0f, Remaining));

        if (Remaining <= 0f)
        {
            Finish();
        }
    }

    // —— 对外 API —— //
    public void StartTimer(float seconds)
    {
        duration = Mathf.Max(0.01f, seconds);
        Remaining = duration;
        IsRunning = true;

        if (slider)
        {
            slider.minValue = 0f;
            slider.maxValue = duration;
            slider.value = showRemaining ? duration : 0f;
        }

        onStarted?.Invoke();
    }

    public void StartTimer() => StartTimer(duration);

    public void Pause() => IsRunning = false;
    public void Resume() => IsRunning = true;

    /// <summary>把计时器重置到seconds，可选是否立刻开始。</summary>
    public void ResetTo(float seconds, bool start = true)
    {
        duration = Mathf.Max(0.01f, seconds);
        Remaining = duration;
        if (slider)
        {
            slider.minValue = 0f;
            slider.maxValue = duration;
            slider.value = showRemaining ? duration : 0f;
        }
        IsRunning = start;
        if (start) onStarted?.Invoke();
    }


    public void AddTime(float delta)
    {
        Remaining = Mathf.Clamp(Remaining + delta, 0f, duration);
    }

    // —— 完成时的默认动作 —— //
    void Finish()
    {
        IsRunning = false;
        Remaining = 0f;

        if (objectToEnable) objectToEnable.SetActive(true);

        if (targetAnimator && !string.IsNullOrEmpty(triggerName))
            targetAnimator.SetTrigger(triggerName);

        onFinished?.Invoke();

        if (disableSelfAfterFinish)
            gameObject.SetActive(false);
    }
}