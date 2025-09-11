using System;
using UnityEngine;

public abstract class MiniGameBase : MonoBehaviour
{
    /// <summary>小游戏完成事件。必须以 (MiniGameBase) 为签名。</summary>
    public event Action<MiniGameBase> Completed;

    /// <summary>由 GameSequenceManager 调用，开始本小游戏。</summary>
    public virtual void Begin()
    {
        Debug.Log($"[MiniGameBase] Begin -> {name}");
    }

    /// <summary>由 GameSequenceManager 调用，清理本小游戏。</summary>
    public virtual void Cleanup()
    {
        Debug.Log($"[MiniGameBase] Cleanup -> {name}");
    }

    /// <summary>供子类调用，宣布完成。</summary>
    protected void Complete()
    {
        Debug.Log($"[MiniGameBase] COMPLETE -> {name}");
        Completed?.Invoke(this);
    }

    // 为了 Manager/Timer 订阅：提供安全的 add/remove 包装（可选）
    public void SubscribeCompleted(Action<MiniGameBase> handler)  => Completed += handler;
    public void UnsubscribeCompleted(Action<MiniGameBase> handler)=> Completed -= handler;
}