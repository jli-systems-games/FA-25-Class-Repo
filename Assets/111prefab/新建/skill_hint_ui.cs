using UnityEngine;
using TMPro;

/// <summary>
/// 技能提示UI管理器 - 显示技能可用状态和CD信息
/// </summary>
public class SkillHintUI : MonoBehaviour
{
    [Header("UI文本组件")]
    public TextMeshProUGUI redQueenHintText;    // Red Queen 提示文本
    public TextMeshProUGUI whiteQueenHintText;  // White Queen 提示文本

    [Header("提示文本配置")]
    public string redQueenReadyText = "按下 G 启用红皇后";
    public string whiteQueenReadyText = "按下 H 启用白皇后";
    public string cooldownTextFormat = "技能冷却中... {0:F1}秒";

    [Header("颜色配置")]
    public Color readyColor = Color.green;      // 可用时的颜色
    public Color cooldownColor = Color.red;     // CD中的颜色
    public Color disabledColor = Color.gray;    // 未装备的颜色

    private bool redQueenEquipped = false;      // 是否装备了红皇后
    private bool whiteQueenEquipped = false;    // 是否装备了白皇后

    void Start()
    {
        // 检查玩家是否装备了技能
        CheckEquippedSkills();

        // 初始化UI显示
        InitializeUI();
    }

    void Update()
    {
        // 实时更新UI
        UpdateRedQueenUI();
        UpdateWhiteQueenUI();
    }

    // 检查玩家装备的技能
    void CheckEquippedSkills()
    {
        if (SkillDataStorage.instance == null) return;

        redQueenEquipped = SkillDataStorage.instance.redQueenSkillLevel > 0;
        whiteQueenEquipped = SkillDataStorage.instance.whiteQueenSkillLevel > 0;

        Debug.Log($"[技能检测] Red Queen: {(redQueenEquipped ? "已装备" : "未装备")}");
        Debug.Log($"[技能检测] White Queen: {(whiteQueenEquipped ? "已装备" : "未装备")}");
    }

    // 初始化UI显示
    void InitializeUI()
    {
        // 如果未装备技能，隐藏或显示为灰色
        if (redQueenHintText != null)
        {
            if (!redQueenEquipped)
            {
                redQueenHintText.text = "";
                redQueenHintText.gameObject.SetActive(false); // 直接隐藏未装备的技能
            }
            else
            {
                redQueenHintText.gameObject.SetActive(true);
            }
        }

        if (whiteQueenHintText != null)
        {
            if (!whiteQueenEquipped)
            {
                whiteQueenHintText.text = "";
                whiteQueenHintText.gameObject.SetActive(false);
            }
            else
            {
                whiteQueenHintText.gameObject.SetActive(true);
            }
        }
    }

    // 更新 Red Queen UI
    void UpdateRedQueenUI()
    {
        if (redQueenHintText == null || !redQueenEquipped) return;

        if (AbilityTriggerSystem.instance == null) return;

        // 检查技能是否在激活中
        if (AbilityTriggerSystem.instance.redQueenSkillActive)
        {
            redQueenHintText.text = "Activated!";
            redQueenHintText.color = Color.yellow;
        }
        // 检查是否在CD中
        else if (AbilityTriggerSystem.instance.redQueenCDRemaining > 0)
        {
            float cdTime = AbilityTriggerSystem.instance.redQueenCDRemaining;
            redQueenHintText.text = string.Format(cooldownTextFormat, cdTime);
            redQueenHintText.color = cooldownColor;
        }
        // 技能可用
        else
        {
            redQueenHintText.text = redQueenReadyText;
            redQueenHintText.color = readyColor;
        }
    }

    // 更新 White Queen UI
    void UpdateWhiteQueenUI()
    {
        if (whiteQueenHintText == null || !whiteQueenEquipped) return;

        if (AbilityTriggerSystem.instance == null) return;

        // 检查技能是否在激活中
        if (AbilityTriggerSystem.instance.whiteQueenSkillActive)
        {
            whiteQueenHintText.text = "Activated!";
            whiteQueenHintText.color = Color.yellow;
        }
        // 检查是否在CD中
        else if (AbilityTriggerSystem.instance.whiteQueenCDRemaining > 0)
        {
            float cdTime = AbilityTriggerSystem.instance.whiteQueenCDRemaining;
            whiteQueenHintText.text = string.Format(cooldownTextFormat, cdTime);
            whiteQueenHintText.color = cooldownColor;
        }
        // 技能可用
        else
        {
            whiteQueenHintText.text = whiteQueenReadyText;
            whiteQueenHintText.color = readyColor;
        }
    }
}