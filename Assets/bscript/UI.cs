using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitInfoUI : MonoBehaviour
{
    public GameObject panel;
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text statsText;

    public void ShowInfo(UnitStats stats)
    {
        if (stats == null) return;

        panel.SetActive(true);
        icon.sprite = stats.unitIcon;
        nameText.text = stats.unitName;
        statsText.text = $"HP: {stats.maxHealth}\nDamage: {stats.damage}\nCost: {stats.cost}";
    }

    public void HideInfo()
    {
        panel.SetActive(false);
    }
}
