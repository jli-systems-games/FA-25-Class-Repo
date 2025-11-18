using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("인벤토리 전체 패널(켜지고 꺼지는 박스)")]
    public GameObject inventoryPanel;

    [Header("아이콘을 보여줄 슬롯 이미지들")]
    public Image[] slotImages;     // Inspector에서 슬롯들 드래그해서 넣기

    [Header("랩 선택 (랩에서만 쓰고, 아니면 비워둬도 됨)")]
    public LabInventorySelector labSelector;   // 랩에서 섞을 때 선택 전달용

    bool _isOpen = false;

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

    void Update()
    {
        // 필요 없으면 무시해도 됨
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
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

    /// <summary>
    /// InventoryManager의 items 내용을 슬롯 이미지에 반영
    /// </summary>
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

    /// <summary>
    /// 시작할 때 한 번, 슬롯 이미지가 붙어있는 오브젝트에서 Button을 찾아
    /// 클릭 시 OnSlotClicked가 호출되게 자동으로 연결.
    /// 슬롯이 15개든 30개든 여기서 한 번에 처리함.
    /// </summary>
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

    /// <summary>
    /// 슬롯이 눌렸을 때 호출.
    /// 그 칸의 ItemData를 찾아서 LabInventorySelector.OnItemClicked(...) 로 넘겨줌.
    /// </summary>
    void OnSlotClicked(int slotIndex)
    {
        if (labSelector == null)
        {
            // 랩 선택을 안 쓰는 씬이면 그냥 무시
            return;
        }

        var mgr = InventoryManager.Instance;
        if (mgr == null || mgr.items == null) return;

        var list = mgr.items;
        if (slotIndex < 0 || slotIndex >= list.Count) return;

        ItemData data = list[slotIndex];
        if (data == null) return;

        // 여기서 랩 셀렉터로 전달!
        labSelector.OnItemClicked(data);
    }
}
