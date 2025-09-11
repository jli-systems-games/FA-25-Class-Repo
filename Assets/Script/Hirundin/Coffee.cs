using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 长按 Space 使滑条上升，松开下降；点击按钮时若数值在给定范围内则成功完成小游戏。
/// 继承你项目里的 MiniGameBase（需要 Complete() 来通知 GameSequenceManager）。
/// </summary>
public class MiniGame_PressInRange : MiniGameBase
{
    [Header("UI")]
    public Slider powerSlider;          // 必填：显示数值的 Slider
    public Button actionButton;         // 必填：判定按钮（玩家点击它）

    [Header("Key")]
    public KeyCode holdKey = KeyCode.Space;  // 长按的键

    [Header("Value Range")]
    public float minValue = 1f;         // 滑条下限
    public float maxValue = 10f;        // 滑条上限
    [Tooltip("成功窗口的下限（含）")]
    public float successMin = 6f;
    [Tooltip("成功窗口的上限（含）")]
    public float successMax = 9f;

    [Header("Speed (units per second)")]
    public float riseSpeed = 5f;        // 按住时每秒上升的量
    public float fallSpeed = 5f;        // 松开时每秒下降的量

    [Header("Events (Optional)")]
    public UnityEvent onPressed;        // 检测到按住
    public UnityEvent onReleased;       // 检测到松开
    public UnityEvent onSuccess;        // 点击且值在区间内
    public UnityEvent onFail;           // 点击但不在区间内（不结束或结束看 needEndOnFail）
    public bool endOnFail = false;      // 失败是否结束该小游戏

    // 运行时
    float _value;
    bool _wasHolding;

    public override void Begin()
    {
        base.Begin();

        // 初始化 Slider
        if (powerSlider != null)
        {
            powerSlider.minValue = minValue;
            powerSlider.maxValue = maxValue;
        }
        _value = Mathf.Clamp(_value <= 0 ? minValue : _value, minValue, maxValue);
        ApplySlider();

        // 监听按钮
        if (actionButton != null)
        {
            actionButton.onClick.RemoveListener(OnActionClicked);
            actionButton.onClick.AddListener(OnActionClicked);
        }
        else
        {
            Debug.LogWarning("[MiniGame_PressInRange] actionButton 未设置。");
        }
    }

    void Update()
    {
        bool holding = Input.GetKey(holdKey);

        if (holding)
        {
            _value += riseSpeed * Time.deltaTime;
            if (!_wasHolding) onPressed?.Invoke();
        }
        else
        {
            _value -= fallSpeed * Time.deltaTime;
            if (_wasHolding) onReleased?.Invoke();
        }
        _wasHolding = holding;

        _value = Mathf.Clamp(_value, minValue, maxValue);
        ApplySlider();
    }

    void ApplySlider()
    {
        if (powerSlider) powerSlider.value = _value;
    }

    void OnActionClicked()
    {
        bool inRange = _value >= successMin && _value <= successMax;

        if (inRange)
        {
            onSuccess?.Invoke();
            Complete(); // ✅ 通知管理器：小游戏成功
        }
        else
        {
            onFail?.Invoke();
            if (endOnFail) Complete();   // 如需失败也结束，打开此开关
        }
    }

    public override void Cleanup()
    {
        base.Cleanup();
        if (actionButton != null)
            actionButton.onClick.RemoveListener(OnActionClicked);
    }
}