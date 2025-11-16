using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PointManager : MonoBehaviour
{
    [Header("点数设置")]
    public int maxPoints = 7;
    public int remainingPoints = 7;
    public TextMeshProUGUI pointsDisplay;

    [Header("普通按钮（被动技能）")]
    public List<NormalButton> normalBtns = new List<NormalButton>();

    [Header("特殊组（3组主动技能）")]
    public List<SpecialGroup> specialGroups = new List<SpecialGroup>();

    [Header("技能名称配置")]
    public string[] groupNames = new string[] { "Alice", "RedQueen", "WhiteQueen" };

    [Header("被动技能名称")]
    public string[] passiveNames = new string[]
    {
        "enemygeneratespeed",  // 敌人生成速度
        "enemyspeed",          // 敌人移动速度
        "force",               // 子弹力度
        "range",               // 发射范围
        "speed"                // 发射速度
    };

    void Start()
    {
        UpdateDisplay();

        // 确保 SkillDataStorage 存在
        if (SkillDataStorage.instance == null)
        {
            GameObject dataObj = new GameObject("SkillDataStorage");
            dataObj.AddComponent<SkillDataStorage>();
        }

        // 普通按钮点击事件
        foreach (var btn in normalBtns)
        {
            btn.btn.onClick.AddListener(() => ClickNormal(btn));
        }

        // 特殊组按钮点击事件
        for (int i = 0; i < specialGroups.Count; i++)
        {
            int groupIndex = i;
            var grp = specialGroups[i];
            grp.btn1.btn.onClick.AddListener(() => ClickSpecial(grp, 1, groupIndex));
            grp.btn2.btn.onClick.AddListener(() => ClickSpecial(grp, 2, groupIndex));
            grp.btn3.btn.onClick.AddListener(() => ClickSpecial(grp, 3, groupIndex));
        }
    }

    // 普通按钮逻辑（被动技能）
    void ClickNormal(NormalButton btn)
    {
        if (btn.picked)
        {
            // 取消选择
            remainingPoints += btn.price;
            btn.picked = false;
            btn.btn.GetComponent<Image>().color = Color.white;

            // 取消被动技能
            SavePassiveSkill(btn, false);
        }
        else
        {
            // 选择
            if (remainingPoints >= btn.price)
            {
                remainingPoints -= btn.price;
                btn.picked = true;
                btn.btn.GetComponent<Image>().color = Color.green;

                // 保存被动技能
                SavePassiveSkill(btn, true);
            }
            else
            {
                Debug.Log("点数不够啦！");
            }
        }

        UpdateDisplay();
    }

    // 保存被动技能到 SkillDataStorage
    void SavePassiveSkill(NormalButton btn, bool isActive)
    {
        if (SkillDataStorage.instance == null) return;

        string buttonName = btn.btn.name.ToLower();
        int value = isActive ? 1 : 0;

        // 根据按钮名称匹配被动技能
        if (buttonName.Contains("enemygeneratespeed") || buttonName.Contains("生成速度"))
        {
            SkillDataStorage.instance.passiveBonus4 = value;
            Debug.Log($"💡 被动技能 - 敌人生成速度减慢: {(isActive ? "激活" : "取消")}");
        }
        else if (buttonName.Contains("enemyspeed") || buttonName.Contains("敌人速度") || buttonName.Contains("移动速度"))
        {
            SkillDataStorage.instance.passiveBonus5 = value;
            Debug.Log($"💡 被动技能 - 敌人移动速度减慢: {(isActive ? "激活" : "取消")}");
        }
        else if (buttonName.Contains("force") || buttonName.Contains("力度"))
        {
            SkillDataStorage.instance.passiveBonus1 = value;
            Debug.Log($"💡 被动技能 - 子弹力度提升: {(isActive ? "激活" : "取消")}");
        }
        else if (buttonName.Contains("range") || buttonName.Contains("范围"))
        {
            SkillDataStorage.instance.passiveBonus2 = value;
            Debug.Log($"💡 被动技能 - 发射范围提升: {(isActive ? "激活" : "取消")}");
        }
        else if (buttonName.Contains("speed") || buttonName.Contains("发射速度"))
        {
            SkillDataStorage.instance.passiveBonus3 = value;
            Debug.Log($"💡 被动技能 - 发射速度加快: {(isActive ? "激活" : "取消")}");
        }
        else
        {
            Debug.LogWarning($"⚠️ 未识别的被动技能按钮: {btn.btn.name}");
        }
    }

    // 特殊组按钮逻辑（主动技能）
    void ClickSpecial(SpecialGroup grp, int wantLevel, int groupIndex)
    {
        int nowLevel = grp.nowLevel;

        // 点同一个 = 取消
        if (nowLevel == wantLevel)
        {
            int back = GetPrice(wantLevel);
            remainingPoints += back;
            grp.nowLevel = 0;
            ChangeColor(grp);
            SaveSkillData(groupIndex, 0);
            UpdateDisplay();
            return;
        }

        // 升级
        if (wantLevel > nowLevel)
        {
            int needPay = wantLevel - nowLevel;

            if (remainingPoints >= needPay)
            {
                remainingPoints -= needPay;
                grp.nowLevel = wantLevel;
                ChangeColor(grp);
                SaveSkillData(groupIndex, wantLevel);
            }
            else
            {
                Debug.Log("点数不够啦！");
            }
        }
        // 降级
        else
        {
            int back = nowLevel - wantLevel;
            remainingPoints += back;
            grp.nowLevel = wantLevel;
            ChangeColor(grp);
            SaveSkillData(groupIndex, wantLevel);
        }

        UpdateDisplay();
    }

    // 保存主动技能到 SkillDataStorage
    void SaveSkillData(int groupIndex, int level)
    {
        if (SkillDataStorage.instance == null) return;

        string skillName = groupIndex < groupNames.Length ? groupNames[groupIndex] : "";

        switch (skillName)
        {
            case "Alice":
                SkillDataStorage.instance.aliceSkillLevel = level;
                Debug.Log($"⚔️ 主动技能 - Alice Lv{level}");
                break;
            case "RedQueen":
                SkillDataStorage.instance.redQueenSkillLevel = level;
                Debug.Log($"⚔️ 主动技能 - Red Queen Lv{level}");
                break;
            case "WhiteQueen":
                SkillDataStorage.instance.whiteQueenSkillLevel = level;
                Debug.Log($"⚔️ 主动技能 - White Queen Lv{level}");
                break;
        }
    }

    int GetPrice(int lv)
    {
        if (lv == 1) return 1;
        if (lv == 2) return 2;
        if (lv == 3) return 3;
        return 0;
    }

    void ChangeColor(SpecialGroup grp)
    {
        grp.btn1.btn.GetComponent<Image>().color = Color.white;
        grp.btn2.btn.GetComponent<Image>().color = Color.white;
        grp.btn3.btn.GetComponent<Image>().color = Color.white;

        if (grp.nowLevel >= 1)
            grp.btn1.btn.GetComponent<Image>().color = Color.green;
        if (grp.nowLevel >= 2)
            grp.btn2.btn.GetComponent<Image>().color = Color.green;
        if (grp.nowLevel >= 3)
            grp.btn3.btn.GetComponent<Image>().color = Color.green;
    }

    void UpdateDisplay()
    {
        if (pointsDisplay != null)
        {
            pointsDisplay.text = $"points left: {remainingPoints} / {maxPoints}";
        }
    }
}

[System.Serializable]
public class NormalButton
{
    public Button btn;
    public int price;
    public bool picked = false;
}

[System.Serializable]
public class SpecialGroup
{
    public string name;
    public SpecialBtn btn1;
    public SpecialBtn btn2;
    public SpecialBtn btn3;
    public int nowLevel = 0;
}

[System.Serializable]
public class SpecialBtn
{
    public Button btn;
}