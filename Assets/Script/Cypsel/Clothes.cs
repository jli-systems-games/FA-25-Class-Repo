using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonMiniGame : MiniGameBase
{
    [SerializeField] private Button finishButton;

    [Header("结算动画相关")]
    [SerializeField] private Animator resultAnimator;   // 指定 Animator
    [SerializeField] private string triggerName = "Show"; // 要触发的 Trigger 名字
    [SerializeField] private float fallbackDelay = 2f; // 如果没法检测长度，等待的时间

    private bool isFinishing = false;

    public override void Begin()
    {
        base.Begin();
        Debug.Log("ButtonMiniGame 开始啦！");
        finishButton.onClick.AddListener(OnButtonClicked);
        isFinishing = false;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        finishButton.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (isFinishing) return;
        isFinishing = true;

        Debug.Log("按钮被按下，触发结算动画！");

        if (resultAnimator != null && !string.IsNullOrEmpty(triggerName))
        {
            resultAnimator.SetTrigger(triggerName);

            // 这里直接用 fallback 延迟，也可以通过动画事件来更精确
            StartCoroutine(FinishAfterDelay(fallbackDelay));
        }
        else
        {
            // 没有 Animator，就直接完成
            Complete();
        }
    }

    private IEnumerator FinishAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("结算动画结束，小游戏完成！");
        Complete();
    }
}