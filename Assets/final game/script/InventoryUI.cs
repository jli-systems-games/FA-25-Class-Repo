using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("인벤토리 전체 패널(켜지고 꺼지는 박스)")]
    public GameObject inventoryPanel;

    [Header("아이콘을 보여줄 슬롯 이미지들")]
    public Image[] slotImages;     // Inspector에서 슬롯들 드래그해서 넣기

    bool _isOpen = false;

    void Start()
    {
        // 처음에는 닫아두기
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        // 인벤토리 내용이 바뀌면 자동으로 UI 갱신
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += Refresh;

        // 혹시 이미 아이템이 있다면 한 번 그려주기
        Refresh();
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= Refresh;
    }

    public void ToggleInventory()
    {
        _isOpen = !_isOpen;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(_isOpen);

        if (_isOpen)
        {
            Refresh();   // 열릴 때 최신 내용으로 갱신
        }
    }

    void Refresh()
    {
        if (InventoryManager.Instance == null || slotImages == null) return;

        var list = InventoryManager.Instance.items;

        for (int i = 0; i < slotImages.Length; i++)
        {
            var img = slotImages[i];
            if (img == null) continue;

            if (i < list.Count && list[i] != null && list[i].icon != null)
            {
                // 아이템이 있는 슬롯
                img.sprite = list[i].icon;
                img.color = Color.white;     // 완전 보이게
            }
            else
            {
                // 비어 있는 슬롯
                img.sprite = null;
                img.color = new Color(1f, 1f, 1f, 0f); // 완전 투명하게
            }
        }
    }
}
