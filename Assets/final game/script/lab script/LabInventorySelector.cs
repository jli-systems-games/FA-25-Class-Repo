using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabInventorySelector : MonoBehaviour
{
    public Button mixButton;
    public LabMixingManager mixingManager;
    public int requiredCount = 5;

    List<ItemData> _selectedItems = new List<ItemData>();



    void Start()
    {
        if (mixButton != null)
            mixButton.interactable = false;
    }

    public void OnItemClicked(ItemData data)
    {
        if (data == null) return;

        if (_selectedItems.Contains(data))
        {
            _selectedItems.Remove(data);
        }
        else
        {
            if (_selectedItems.Count >= requiredCount)
                return;

            _selectedItems.Add(data);
        }

        if (mixButton != null)
            mixButton.interactable = (_selectedItems.Count > 0);

        if (mixingManager != null)
            mixingManager.SetSelectedItems(_selectedItems);
    }

    public void CloseInventoryUI()
    {
        // FindFirstObjectByType<InventoryUI>()는 씬에서 InventoryUI 컴포넌트를 찾습니다.
        var invUI = FindFirstObjectByType<InventoryUI>();
        if (invUI != null)
        {
            // InventoryUI.cs에 정의된 CloseInventory() 함수를 호출합니다.
            invUI.CloseInventory();
        }
    }

    public void ClearSelection()
    {
        _selectedItems.Clear();

        if (mixButton != null)
            mixButton.interactable = false;

        if (mixingManager != null)
            mixingManager.SetSelectedItems(_selectedItems);

        // ❌ [삭제된 부분]: 이 코드를 삭제해야 무한 루프가 발생하지 않습니다.
        // var invUI = FindFirstObjectByType<InventoryUI>();
        // if (invUI != null)
        // {
        //     invUI.ResetAllSlotColors(); 
        // }
    }
}