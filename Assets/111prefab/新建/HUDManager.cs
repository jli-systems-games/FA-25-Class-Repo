using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("任务进度条")]
    public Slider missionBar;               // 任务完成度条
    public TextMeshProUGUI missionLabel;    // 任务数字文本（可选）

    [Header("生命值条")]
    public Slider lifeBar;                  // 剩余生命条
    public TextMeshProUGUI lifeLabel;       // 生命数字文本（可选）

    void Start()
    {
        // 禁止玩家拖动滑动条
        if (missionBar != null)
        {
            missionBar.interactable = false;
        }

        if (lifeBar != null)
        {
            lifeBar.interactable = false;
        }

        // 初始化显示
        SetupBars();
    }

    void SetupBars()
    {
        if (GameController.instance != null)
        {
            // 配置任务进度条
            if (missionBar != null)
            {
                missionBar.maxValue = GameController.instance.winTargetCount;
                missionBar.value = 0;
            }

            // 配置生命值条
            if (lifeBar != null)
            {
                lifeBar.maxValue = GameController.instance.maxAllowedEscapes;
                lifeBar.value = GameController.instance.maxAllowedEscapes; // 满生命开始
            }
        }
    }

    void Update()
    {
        RefreshMissionBar();
        RefreshLifeBar();
    }

    // 刷新任务进度
    void RefreshMissionBar()
    {
        if (GameController.instance != null && missionBar != null)
        {
            // 更新进度值
            missionBar.value = GameController.instance.hitCount;

            // 更新文字显示
            if (missionLabel != null)
            {
                missionLabel.text = $"{GameController.instance.hitCount}/{GameController.instance.winTargetCount}";
            }
        }
    }

    // 刷新生命值
    void RefreshLifeBar()
    {
        if (GameController.instance != null && lifeBar != null)
        {
            // 生命 = 最大逃脱数 - 当前逃脱数
            int remainingLives = GameController.instance.maxAllowedEscapes - GameController.instance.escapeCount;
            lifeBar.value = remainingLives;

            // 更新文字显示
            if (lifeLabel != null)
            {
                lifeLabel.text = $"{remainingLives}/{GameController.instance.maxAllowedEscapes}";
            }
        }
    }
}