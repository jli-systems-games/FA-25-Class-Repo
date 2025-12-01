using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("인벤토리 전체 패널(켜지고 꺼지는 박스)")]
    public GameObject inventoryPanel;

    [Header("아이콘을 보여줄 슬롯 이미지들")]
    public Image[] slotImages;
    bool[] _slotSelected;

    [Header("랩 선택 (랩에서만 쓰고, 아니면 비워둬도 됨)")]
    public LabInventorySelector labSelector;   // 랩에서 섞을 때 선택 전달용

    bool _isOpen = false;

    void Awake()
    {
        if (slotImages != null)
            _slotSelected = new bool[slotImages.Length];
    }

    void Start()
    {
        // 처음에는 닫아두기
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        // 인벤토리 변경 이벤트에 반응해서 자동으로 새로고침
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += Refresh;

        // 슬롯 버튼들에 클릭 리스너 자동으로 연결
        SetupSlotButtons();

        // 처음 한 번 그려주기
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
            Refresh();
        }
    }

    public void Refresh()
    {
        if (slotImages == null || slotImages.Length == 0)
            return;

        var mgr = InventoryManager.Instance;
        var list = (mgr != null) ? mgr.items : null;

        for (int i = 0; i < slotImages.Length; i++)
        {
            Image img = slotImages[i];
            if (img == null) continue;

            if (list != null && i < list.Count && list[i] != null)
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


    void SetupSlotButtons()
    {
        if (slotImages == null) return;

        for (int i = 0; i < slotImages.Length; i++)
        {
            Image img = slotImages[i];
            if (img == null) continue;

            int slotIndex = i;

            // 같은 오브젝트에 붙어있는 Button 찾기
            Button btn = img.GetComponent<Button>();
            if (btn == null)
                btn = img.GetComponentInParent<Button>();

            if (btn == null) continue;

            // 중복 방지로 기존 리스너 제거하고 다시 달기
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnSlotClicked(slotIndex));
        }
    }

    void OnSlotClicked(int slotIndex)
    {
        // 0. 슬롯 인덱스/배열 체크
        if (slotImages == null || slotIndex < 0 || slotIndex >= slotImages.Length)
            return;

        // 1. 색 토글 (선택 ↔ 해제)
        if (_slotSelected[slotIndex])
        {
            // 이미 선택되어 있었으면 → 해제
            _slotSelected[slotIndex] = false;
            slotImages[slotIndex].color = Color.white;
        }
        else
        {
            // 아직 선택 안 되어 있으면 → 선택 + 빨강
            _slotSelected[slotIndex] = true;
            slotImages[slotIndex].color = new Color(1f, 0.3f, 0.3f, 1f);
        }

        // 2. 실제 아이템을 LabSelector에 전달 (기존 로직 유지)
        if (labSelector == null)
            return;

        var mgr = InventoryManager.Instance;
        if (mgr == null || mgr.items == null) return;

        var list = mgr.items;
        if (slotIndex < 0 || slotIndex >= list.Count) return;

        ItemData data = list[slotIndex];
        if (data == null) return;

        // LabInventorySelector가 내부 리스트를 토글(add/remove) 하도록
        labSelector.OnItemClicked(data);
    }

    public void ResetAllSlotColors()
    {
        if (slotImages == null || _slotSelected == null) return;

        for (int i = 0; i < slotImages.Length; i++)
        {
            _slotSelected[i] = false;
            if (slotImages[i] != null)
                slotImages[i].color = Color.white;
        }
    }



}
