using UnityEngine;

/// <summary>
/// 示例小游戏：按下空格即视为完成。
/// 实际项目里你会在胜利/失败/通关时调用 RaiseCompleted()。
/// </summary>
public class SimpleMiniGame : MiniGameBase
{
    private bool _running;

    public override void Begin()
    {
        _running = true;
        Debug.Log("[SimpleMiniGame] Begin");
        // 这里可以做重置、生成关卡元素等
    }

    public override void Cleanup()
    {
        _running = false;
        Debug.Log("[SimpleMiniGame] Cleanup");
        // 这里可以回收对象、停协程等
    }

    void Update()
    {
        if (!_running) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("[SimpleMiniGame] Completed by SPACE");
            // 通知管理器推进到下一个
            // RaiseCompleted() 是基类受保护方法，这里可以包一层
            var raiseCompletedMethod = typeof(MiniGameBase)
                .GetMethod("RaiseCompleted", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            raiseCompletedMethod.Invoke(this, null);
        }
    }
}