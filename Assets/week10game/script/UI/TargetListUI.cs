using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TargetListUI : MonoBehaviour
{
    [Header("Refs")]
    public TargetItemManager targetManager;
    public TextMeshProUGUI[] targetTexts; // 크기 3로 맞추기

    Dictionary<InteractableItem, int> indexByItem = new Dictionary<InteractableItem, int>();

    void Start()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        if (targetManager == null || targetTexts == null) return;

        var list = targetManager.GetCurrentTargets();
        indexByItem.Clear();

        for (int i = 0; i < targetTexts.Length; i++)
        {
            if (i < list.Count && list[i] != null)
            {
                targetTexts[i].text = $"• {list[i].GetName()}";
                targetTexts[i].alpha = 1f;
                indexByItem[list[i]] = i;
            }
            else
            {
                targetTexts[i].text = "";
                targetTexts[i].alpha = 0.6f;
            }
        }
    }

    // TargetItemManager에서 아이템 찾을 때 이 메서드를 호출해주면 체크 표시 가능
    public void MarkFound(InteractableItem item)
    {
        if (item == null) return;
        if (indexByItem.TryGetValue(item, out int idx))
        {
            if (idx >= 0 && idx < targetTexts.Length)
            {
                targetTexts[idx].text = $"✓ {item.GetName()}";
                targetTexts[idx].color = Color.red;
                targetTexts[idx].alpha = 0.5f;
            }
        }
    }
}
