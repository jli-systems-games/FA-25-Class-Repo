using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// 被子弹调用的减速控制器：
/// 子弹只会做 hitSlowController.ApplySlow(duration, factor)
/// 这个脚本会自动去找「当前角色真正的 CharacterMovement」
/// （包括你的 P1KeyboardMovement 这种子类），
/// 然后通过 MovementSpeedMultiplier 来减速。
/// </summary>
public class PlayerSlowController : MonoBehaviour
{
    [Header("要减速的移动组件 (可以留空, 会自动找)")]
    public CharacterMovement TargetMovement;   // CharacterMovement 或 P1KeyboardMovement
    public CharacterRun TargetRun;

    [Header("减速叠加规则")]
    [Tooltip("true = 多个减速相乘, false = 取最慢的那个(最小 factor)")]
    public bool StackSlows = false;

    private float _baseRunSpeed;
    private float _currentSlowFactor = 1f;
    private float _slowEndTime = 0f;

    void Awake()
    {
        // 自动找当前物体上的移动脚本
        if (TargetMovement == null)
            TargetMovement = GetComponent<CharacterMovement>();   // 这里会拿到 P1KeyboardMovement

        if (TargetRun == null)
            TargetRun = GetComponent<CharacterRun>();

        if (TargetRun != null)
            _baseRunSpeed = TargetRun.RunSpeed;
    }

    void Update()
    {
        if (TargetMovement == null)
        {
            // 万一你后来换了组件，这里每帧再尝试找一次
            TargetMovement = GetComponent<CharacterMovement>();
            if (TargetMovement == null)
                return;
        }

        // 减速时间到了就恢复
        if (Time.time > _slowEndTime)
        {
            _currentSlowFactor = 1f;
        }

        // 把减速倍率写回 MovementSpeedMultiplier
        TargetMovement.MovementSpeedMultiplier = _currentSlowFactor;

        // 跑步速度也跟着缩放（如果有跑步组件）
        if (TargetRun != null)
        {
            TargetRun.RunSpeed = _baseRunSpeed * _currentSlowFactor;
        }
    }

    /// <summary>
    /// 被子弹调用：duration 秒内，速度变为原来的 factor 倍
    /// </summary>
    public void ApplySlow(float duration, float factor)
    {
        if (factor <= 0f)
        {
            factor = 0.01f;
        }

        if (StackSlows)
        {
            // 叠加：多个减速相乘
            _currentSlowFactor *= factor;
        }
        else
        {
            // 取最慢的(越小越慢)
            _currentSlowFactor = Mathf.Min(_currentSlowFactor, factor);
        }

        // 结束时间取最长的那一个
        float newEnd = Time.time + duration;
        if (newEnd > _slowEndTime)
        {
            _slowEndTime = newEnd;
        }
    }
}
