using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public GameObject achievementUIPrefab;
    public Transform achievementUIParent;
    public GameManager gameManager;

    public List<Achievement> achievements;

    public float verticalSpacing = 40f;
    private readonly List<AchievementUI> activeUIs = new List<AchievementUI>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Unlock(string title)
    {
        Achievement ach = achievements.Find(a => a.title == title);
        if (ach == null) return;

        if (gameManager != null && ach.scoreReward != 0)
        {
            gameManager.AddScore(ach.scoreReward, true);
        }

        ShowAchievementUI(ach.title, ach.rarity);
    }

    void ShowAchievementUI(string title, Rarity rarity)
    {
        if (achievementUIPrefab == null || achievementUIParent == null)
        {
            return;
        }

        GameObject ui = Instantiate(achievementUIPrefab, achievementUIParent);

        AchievementUI uiComp = ui.GetComponent<AchievementUI>();
        if (uiComp != null)
        {
            activeUIs.Add(uiComp);

            RelayoutAchievementUIs();

            uiComp.Setup(title, rarity);
        }
    }
    public void OnAchievementUIClosed(AchievementUI ui)
    {
        if (activeUIs.Remove(ui))
        {
            RelayoutAchievementUIs();
        }
    }

    void RelayoutAchievementUIs()
    {
        for (int i = 0; i < activeUIs.Count; i++)
        {
            var rt = activeUIs[i].GetComponent<RectTransform>();
            if (rt == null) continue;

            rt.anchoredPosition = new Vector2(0f, -verticalSpacing * i);
        }
    }


}
