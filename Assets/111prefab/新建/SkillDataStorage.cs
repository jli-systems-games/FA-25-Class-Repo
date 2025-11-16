using UnityEngine;

/// <summary>
/// 技能数据存储 - 跨场景保存玩家选择的技能
/// </summary>
public class SkillDataStorage : MonoBehaviour
{
    public static SkillDataStorage instance;

    [Header("主动技能等级")]
    public int aliceSkillLevel = 0;          // 0=未选择, 1=Lv0, 2=Lv1, 3=Lv2
    public int redQueenSkillLevel = 0;       // 0=未选择, 1=Lv0, 2=Lv1, 3=Lv2
    public int whiteQueenSkillLevel = 0;     // 0=未选择, 1=Lv0, 2=Lv1, 3=Lv2

    [Header("被动技能等级（预留）")]
    public int passiveBonus1 = 0;
    public int passiveBonus2 = 0;
    public int passiveBonus3 = 0;
    public int passiveBonus4 = 0;
    public int passiveBonus5 = 0;

    void Awake()
    {
        // 单例模式 + 跨场景保持
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 重置所有技能（新游戏时调用）
    public void ResetAllSkills()
    {
        aliceSkillLevel = 0;
        redQueenSkillLevel = 0;
        whiteQueenSkillLevel = 0;
        passiveBonus1 = 0;
        passiveBonus2 = 0;
        passiveBonus3 = 0;
        passiveBonus4 = 0;
        passiveBonus5 = 0;

        Debug.Log("所有技能已重置");
    }

    // 调试显示当前技能
    public void PrintCurrentSkills()
    {
        Debug.Log($"===== 当前技能状态 =====");
        Debug.Log($"Alice: Lv{aliceSkillLevel}");
        Debug.Log($"Red Queen: Lv{redQueenSkillLevel}");
        Debug.Log($"White Queen: Lv{whiteQueenSkillLevel}");
        Debug.Log($"========================");
    }
}
