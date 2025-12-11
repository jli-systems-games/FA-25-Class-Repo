using UnityEngine;
using MoreMountains.TopDownEngine;

public class FanButton : MonoBehaviour
{
    [Header("绑定设置")]
    [Tooltip("请把挂着 AreaWindZone 脚本的那个 Empty 物体拖到这里")]
    public AreaWindZone TargetWindZone; // ★ 这里改成了你的新脚本类型

    [Header("参数")]
    [Tooltip("冷却时间 (防止玩家站在按钮上时频繁鬼畜切换)")]
    public float Cooldown = 1.0f;

    [Header("反馈 (可选)")]
    public Animator ButtonAnimator; // 如果有按压动画
    public string PressAnimationParam = "Press";

    private float _lastPressTime = -100f;

    private void OnTriggerEnter(Collider other)
    {
        // 1. 检查冷却
        if (Time.time - _lastPressTime < Cooldown) return;

        // 2. 检查是否是 Player (TDE 标准检查方法)
        Character character = other.GetComponent<Character>();

        if (character != null && character.CharacterType == Character.CharacterTypes.Player)
        {
            TriggerButton();
        }
    }

    void TriggerButton()
    {
        _lastPressTime = Time.time;

        // ★ 核心修改：切换 AreaWindZone 的状态
        if (TargetWindZone != null)
        {
            // 直接修改它的布尔值开关
            TargetWindZone.IsActive = !TargetWindZone.IsActive;

            Debug.Log($"按钮 {name} 被按下，风区 {TargetWindZone.name} 状态变为: {TargetWindZone.IsActive}");
        }
        else
        {
            Debug.LogWarning($"⚠️ 按钮 {name} 还没绑定 TargetWindZone！请在 Inspector 里把风区拖进去。");
        }

        // 播放动画 (可选)
        if (ButtonAnimator != null)
        {
            ButtonAnimator.SetTrigger(PressAnimationParam);
        }
    }
}