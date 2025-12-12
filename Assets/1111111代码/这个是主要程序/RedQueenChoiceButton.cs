using UnityEngine;
using UnityEngine.UI;

public class SimpleRedQueenChoice : MonoBehaviour
{
    [Header("选择类型")]
    public ChoiceType choiceType;

    public enum ChoiceType
    {
        GetRedQueen,
        RestoreHP
    }

    [Header("调试")]
    public bool showDebugInfo = true;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        if (showDebugInfo)
        {
            Debug.Log($"========== 按钮被点击 ==========");
            Debug.Log($"选择类型: {choiceType}");
        }

        if (choiceType == ChoiceType.GetRedQueen)
        {
            if (PlayerAbilityManager.Instance != null)
            {
                PlayerAbilityManager.Instance.redQueenLevel = 1;
                if (showDebugInfo)
                {
                    Debug.Log("✨ 获得红皇后能力 Lv.1！");
                }
            }
        }
        else if (choiceType == ChoiceType.RestoreHP)
        {
            if (LifeManager.Instance != null)
            {
                int beforeHP = LifeManager.Instance.GetCurrentLives();
                LifeManager.Instance.ResetLives();  // 这里会自动调用UI更新
                int afterHP = LifeManager.Instance.GetCurrentLives();

                if (showDebugInfo)
                {
                    Debug.Log($"💖 生命值已恢复！{beforeHP} → {afterHP}");
                }
            }
        }
    }
}