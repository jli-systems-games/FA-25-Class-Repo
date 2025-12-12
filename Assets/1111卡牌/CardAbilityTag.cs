using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 超简单卡牌脚本 - 直接挂在每个卡牌Prefab上
/// 每个卡牌自己知道自己是什么，点击就激活
/// </summary>
public class SimpleCard : MonoBehaviour
{
    [Header("这张卡是什么能力")]
    public CardType cardType;

    public enum CardType
    {
        Alice1,
        Alice2,
        RedQueen2,
        FireRate,
        BulletSpeed,
        BulletSize
    }

    void Start()
    {
        // 绑定按钮点击
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        Debug.Log($"🎴 点击卡牌: {cardType}");

        // 直接激活对应能力
        switch (cardType)
        {
            case CardType.Alice1:
            case CardType.Alice2:
                PlayerAbilityManager.Instance.UpgradeAlice();
                Debug.Log("✅ 爱丽丝升级！");
                break;

            case CardType.RedQueen2:
                PlayerAbilityManager.Instance.UpgradeRedQueen();
                Debug.Log("✅ 红皇后升级！");
                break;

            case CardType.FireRate:
                PlayerAbilityManager.Instance.AddFireRateStack();
                Debug.Log("✅ 发射速度提升！");
                break;

            case CardType.BulletSpeed:
                PlayerAbilityManager.Instance.AddBulletSpeedStack();
                Debug.Log("✅ 子弹速度提升！");
                break;

            case CardType.BulletSize:
                PlayerAbilityManager.Instance.AddBulletSizeStack();
                Debug.Log("✅ 子弹大小提升！");
                break;
        }
    }

    // 检查这张卡现在能不能用
    public bool CanUse()
    {
        switch (cardType)
        {
            case CardType.Alice1:
                return PlayerAbilityManager.Instance.aliceLevel == 0;
            case CardType.Alice2:
                return PlayerAbilityManager.Instance.aliceLevel >= 1;
            case CardType.RedQueen2:
                return PlayerAbilityManager.Instance.redQueenLevel >= 1;
            default:
                return true;
        }
    }
}