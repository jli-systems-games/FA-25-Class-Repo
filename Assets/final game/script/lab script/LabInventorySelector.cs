using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabInventorySelector : MonoBehaviour
{
    [Header("참조")]
    public Button mixButton;                 // 섞기 버튼
    public LabMixingManager mixingManager;   // 믹싱 매니저

    [Header("설정")]
    public int requiredCount = 5;            // 필요한 아이템 개수(기본 5개)

    // 현재 선택된 아이템들
    List<ItemData> _selectedItems = new List<ItemData>();

    void Start()
    {
        if (mixButton != null)
            mixButton.interactable = false;

        // 처음 상태도 믹싱 매니저에 알려주기
        if (mixingManager != null)
            mixingManager.SetSelectedItems(_selectedItems);
    }

    /// <summary>
    /// 인벤토리 슬롯에서 어떤 ItemData가 클릭되었는지 전달받는 함수.
    /// InventoryUI에서 직접 호출해준다.
    /// </summary>
    public void OnItemClicked(ItemData data)
    {
        if (data == null) return;

        // 이미 선택되어 있으면 해제
        if (_selectedItems.Contains(data))
        {
            _selectedItems.Remove(data);
        }
        else
        {
            // 이미 requiredCount개 있으면 더 이상 추가 안 함
            if (_selectedItems.Count >= requiredCount)
                return;

            _selectedItems.Add(data);
        }

        // 섞기 버튼은 정확히 requiredCount개일 때만 활성화
        if (mixButton != null)
            mixButton.interactable = (_selectedItems.Count == requiredCount);

        // 믹싱 매니저에도 선택 상태 전달
        if (mixingManager != null)
            mixingManager.SetSelectedItems(_selectedItems);
    }

    /// <summary>
    /// 믹싱이 끝난 뒤 선택 상태를 싹 지우고 싶을 때 호출.
    /// LabMixingManager에서 성공/실패 후에 쓰면 좋음.
    /// </summary>
    public void ClearSelection()
    {
        _selectedItems.Clear();

        if (mixButton != null)
            mixButton.interactable = false;

        if (mixingManager != null)
            mixingManager.SetSelectedItems(_selectedItems);
    }
}
