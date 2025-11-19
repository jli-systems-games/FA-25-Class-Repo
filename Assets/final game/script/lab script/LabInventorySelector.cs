using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabInventorySelector : MonoBehaviour
{
    public Button mixButton;
    public LabMixingManager mixingManager;
    public int requiredCount = 5;   // 최대 선택 개수 (5개까지)

    List<ItemData> _selectedItems = new List<ItemData>();

    void Start()
    {
        if (mixButton != null)
            mixButton.interactable = false;
    }

    public void OnItemClicked(ItemData data)
    {
        if (data == null) return;

        // 이미 선택되어 있었으면 제거 (취소)
        if (_selectedItems.Contains(data))
        {
            _selectedItems.Remove(data);
        }
        else
        {
            // 최대 5개까지만 허용
            if (_selectedItems.Count >= requiredCount)
                return;

            _selectedItems.Add(data);
        }

        // 1개 이상 선택돼 있으면 mix 버튼 켜기
        if (mixButton != null)
            mixButton.interactable = (_selectedItems.Count > 0);

        if (mixingManager != null)
            mixingManager.SetSelectedItems(_selectedItems);
    }

    public void ClearSelection()
    {
        _selectedItems.Clear();

        if (mixButton != null)
            mixButton.interactable = false;

        if (mixingManager != null)
            mixingManager.SetSelectedItems(_selectedItems);

        // 🔴 인벤토리 슬롯 색도 전부 초기화
        var invUI = FindFirstObjectByType<InventoryUI>();
        if (invUI != null)
            invUI.ResetAllSlotColors();
    }
}
