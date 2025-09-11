using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 点击给定的所有按钮后即成功完成小游戏。
/// 继承 MiniGameBase（需要项目里已有的 Complete() / Begin() / Cleanup()）
/// </summary>
public class MiniGame_ClickAllButtons : MiniGameBase
{
    [Header("Buttons To Click")]
    [Tooltip("把需要点击的按钮都拖进来")]
    public List<Button> targetButtons = new List<Button>();

    [Header("Options")]
    public bool disableButtonAfterClick = true;   // 点击后是否禁用按钮
    public bool allowRepeatClicks = false;        // 若为 true，重复点不计数但也不报错
    public bool autoWireOnBegin = true;           // Begin 时自动挂监听

    [Header("Feedback (Optional)")]
    public UnityEvent onEachButtonClicked;        // 每点一个按钮回调
    public UnityEvent onAllButtonsClicked;        // 全部完成回调

    [Header("UI (Optional)")]
    public Text progressText;                     // 显示进度，例如 "2 / 5"

    // 运行时
    private int _clickedCount = 0;
    private readonly HashSet<Button> _clickedSet = new HashSet<Button>();

    public override void Begin()
    {
        base.Begin();
        ResetState();

        if (autoWireOnBegin)
            WireButtons(true);

        UpdateProgressUI();
    }

    public override void Cleanup()
    {
        base.Cleanup();
        WireButtons(false);
    }

    private void ResetState()
    {
        _clickedCount = 0;
        _clickedSet.Clear();

        // 重置按钮交互状态
        foreach (var btn in targetButtons)
        {
            if (!btn) continue;
            if (disableButtonAfterClick) btn.interactable = true;
        }
    }

    private void WireButtons(bool add)
    {
        foreach (var btn in targetButtons)
        {
            if (!btn) continue;

            if (add)
                btn.onClick.AddListener(() => OnTargetButtonClicked(btn));
            else
                btn.onClick.RemoveAllListeners(); // 简洁起见：该 MiniGame 独占这些按钮的点击
        }
    }

    private void OnTargetButtonClicked(Button btn)
    {
        if (!btn) return;

        // 已点过
        if (_clickedSet.Contains(btn))
        {
            if (!allowRepeatClicks) return;
            // allowRepeatClicks=true 时，重复点击不计数但可以给点反馈
            onEachButtonClicked?.Invoke();
            return;
        }

        _clickedSet.Add(btn);
        _clickedCount++;

        if (disableButtonAfterClick) btn.interactable = false;

        onEachButtonClicked?.Invoke();
        UpdateProgressUI();

        if (_clickedCount >= ValidButtonCount())
        {
            onAllButtonsClicked?.Invoke();
            Complete(); // ✅ 成功：通知 GameSequenceManager
        }
    }

    private int ValidButtonCount()
    {
        int n = 0;
        foreach (var b in targetButtons) if (b) n++;
        return n;
    }

    private void UpdateProgressUI()
    {
        if (!progressText) return;
        progressText.text = $"{_clickedCount} / {ValidButtonCount()}";
    }
}