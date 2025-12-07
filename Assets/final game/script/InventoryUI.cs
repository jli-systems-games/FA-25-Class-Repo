using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("인벤토리 전체 패널(켜지고 꺼지는 박스)")]
    public GameObject inventoryPanel;

    // ============================
    // 1) 일반 슬롯 (믹스 가능한 슬롯)
    // ============================
    [Header("일반 슬롯 (믹스 가능한 슬롯들)")]
    public Image[] slotImages;              // 기존 슬롯들
    bool[] _slotSelected;                   // 일반 슬롯 선택 여부
    ItemData[] _normalSlotData;             // 각 일반 슬롯에 어떤 ItemData가 들어있는지

    // ============================
    // 2) 고정 슬롯 (특정 아이템 전용, 믹스 불가)
    // ============================
    [Header("고정 슬롯 (특정 아이템만 들어가는 6칸, 믹스 불가)")]
    public Image[] fixedSlotImages;         // 6개 고정 슬롯 이미지들
    [Tooltip("각 고정 슬롯에 들어갈 ItemData의 itemId (fixedSlotImages와 길이 같게 설정)")]
    public string[] fixedSlotItemIds;       // 인스펙터에서 직접 적을 itemId

    bool[] _fixedSlotSelected;              // 고정 슬롯 선택 여부
    ItemData[] _fixedSlotData;              // 각 고정 슬롯에 들어간 ItemData

    [Header("랩 선택 (랩에서만 쓰고, 아니면 비워둬도 됨)")]
    public LabInventorySelector labSelector;   // 랩에서 섞을 때 선택 전달용

    [Header("Tooltip Text Fields")]
    public TMP_Text tooltipNameText;
    public TMP_Text tooltipDescriptionText;

    bool _isOpen = false;

    void Awake()
    {
        // 일반 슬롯 선택 배열
        if (slotImages != null && slotImages.Length > 0)
        {
            _slotSelected = new bool[slotImages.Length];
            _normalSlotData = new ItemData[slotImages.Length];
        }

        // 고정 슬롯 선택 배열
        if (fixedSlotImages != null && fixedSlotImages.Length > 0)
        {
            _fixedSlotSelected = new bool[fixedSlotImages.Length];
            _fixedSlotData = new ItemData[fixedSlotImages.Length];
        }
    }

    void Start()
    {
        // 처음에는 인벤토리 닫기
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        HideTooltip();

        // 인벤토리 변경 시 자동 새로고침
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += Refresh;

        // 슬롯 버튼들 세팅
        SetupSlotButtons();        // 일반 슬롯
        SetupFixedSlotButtons();   // 고정 슬롯

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
        if (Input.GetKeyDown(KeyCode.Tab))
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

    // ============================
    //  인벤토리 슬롯 채우기 로직
    // ============================
    public void Refresh()
    {
        var mgr = InventoryManager.Instance;
        var list = (mgr != null) ? mgr.items : null;

        // 1) 모든 슬롯 비우기 (이미지/데이터/색상)
        ClearAllSlotsVisual();

        if (list == null || list.Count == 0)
            return;

        // 고정 슬롯에 먼저 배치하고,
        // 나머지는 일반 슬롯에 순서대로 채움
        for (int i = 0; i < list.Count; i++)
        {
            ItemData item = list[i];
            if (item == null) continue;

            bool placed = false;

            // ----------------------------
            // (1) 고정 슬롯에 배치 시도
            // ----------------------------
            if (fixedSlotImages != null &&
                fixedSlotItemIds != null &&
                fixedSlotImages.Length > 0 &&
                fixedSlotItemIds.Length == fixedSlotImages.Length)
            {
                for (int f = 0; f < fixedSlotItemIds.Length; f++)
                {
                    // itemId가 설정된 고정 슬롯과 일치하고,
                    // 아직 그 자리에 아무것도 안 들어갔으면
                    if (!string.IsNullOrEmpty(fixedSlotItemIds[f]) &&
                        item.itemId == fixedSlotItemIds[f] &&
                        _fixedSlotData[f] == null)
                    {
                        _fixedSlotData[f] = item;

                        if (fixedSlotImages[f] != null)
                        {
                            fixedSlotImages[f].sprite = item.icon;
                            fixedSlotImages[f].color = Color.white;
                        }

                        placed = true;
                        break;
                    }
                }
            }

            if (placed) continue;

            // ----------------------------
            // (2) 일반 슬롯에 배치
            // ----------------------------
            if (slotImages != null && slotImages.Length > 0)
            {
                for (int s = 0; s < slotImages.Length; s++)
                {
                    if (_normalSlotData[s] == null)
                    {
                        _normalSlotData[s] = item;

                        if (slotImages[s] != null)
                        {
                            slotImages[s].sprite = item.icon;
                            slotImages[s].color = Color.white;
                        }

                        placed = true;
                        break;
                    }
                }
            }

            // 만약 일반 슬롯도 꽉 차서 못 들어가면 그냥 무시 (혹시 인벤토리 크기 넘쳤을 때)
        }
    }

    void ClearAllSlotsVisual()
    {
        // 일반 슬롯
        if (slotImages != null)
        {
            for (int i = 0; i < slotImages.Length; i++)
            {
                if (slotImages[i] != null)
                {
                    slotImages[i].sprite = null;
                    slotImages[i].color = new Color(1f, 1f, 1f, 0f); // 투명
                }
            }
        }
        if (_normalSlotData != null)
        {
            for (int i = 0; i < _normalSlotData.Length; i++)
                _normalSlotData[i] = null;
        }
        if (_slotSelected != null)
        {
            for (int i = 0; i < _slotSelected.Length; i++)
                _slotSelected[i] = false;
        }

        // 고정 슬롯
        if (fixedSlotImages != null)
        {
            for (int i = 0; i < fixedSlotImages.Length; i++)
            {
                if (fixedSlotImages[i] != null)
                {
                    fixedSlotImages[i].sprite = null;
                    fixedSlotImages[i].color = new Color(1f, 1f, 1f, 0f); // 투명
                }
            }
        }
        if (_fixedSlotData != null)
        {
            for (int i = 0; i < _fixedSlotData.Length; i++)
                _fixedSlotData[i] = null;
        }
        if (_fixedSlotSelected != null)
        {
            for (int i = 0; i < _fixedSlotSelected.Length; i++)
                _fixedSlotSelected[i] = false;
        }

        HideTooltip();
    }

    // ============================
    //  슬롯 버튼 세팅
    // ============================
    void SetupSlotButtons()
    {
        if (slotImages == null) return;

        for (int i = 0; i < slotImages.Length; i++)
        {
            Image img = slotImages[i];
            if (img == null) continue;

            int slotIndex = i;

            Button btn = img.GetComponent<Button>();
            if (btn == null)
                btn = img.GetComponentInParent<Button>();

            if (btn == null)
            {
                btn = img.gameObject.AddComponent<Button>();
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnNormalSlotClicked(slotIndex));
        }
    }

    void SetupFixedSlotButtons()
    {
        if (fixedSlotImages == null) return;

        for (int i = 0; i < fixedSlotImages.Length; i++)
        {
            Image img = fixedSlotImages[i];
            if (img == null) continue;

            int slotIndex = i;

            Button btn = img.GetComponent<Button>();
            if (btn == null)
                btn = img.GetComponentInParent<Button>();

            if (btn == null)
            {
                btn = img.gameObject.AddComponent<Button>();
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnFixedSlotClicked(slotIndex));
        }
    }

    // ============================
    //  일반 슬롯 클릭 (빨간색, 믹스 가능)
    // ============================
    void OnNormalSlotClicked(int slotIndex)
    {
        if (slotImages == null || slotIndex < 0 || slotIndex >= slotImages.Length)
            return;

        if (_normalSlotData == null) return;

        ItemData data = _normalSlotData[slotIndex];

        if (data == null)
        {
            HideTooltip();
            return;
        }

        // 선택 상태 토글
        _slotSelected[slotIndex] = !_slotSelected[slotIndex];

        if (_slotSelected[slotIndex])
        {
            // 빨간색 하이라이트 (기존 그대로)
            slotImages[slotIndex].color = new Color(1f, 0.3f, 0.3f, 1f);
            ShowTooltip(data);

            // 믹스용 선택 전달
            if (labSelector != null)
                labSelector.OnItemClicked(data);
        }
        else
        {
            // 선택 해제
            slotImages[slotIndex].color = Color.white;
            HideTooltip();

            // 선택 해제 시에도 labSelector에 다시 보내고 싶다면
            // 여기서 labSelector.ClearSelection() 등을 호출할 수 있지만,
            // 현재 구조를 최대한 그대로 유지하려고 아무 것도 안 함.
        }
    }

    // ============================
    //  고정 슬롯 클릭 (파란색, 믹스 불가)
    // ============================
    void OnFixedSlotClicked(int slotIndex)
    {
        if (fixedSlotImages == null || slotIndex < 0 || slotIndex >= fixedSlotImages.Length)
            return;

        if (_fixedSlotData == null) return;

        ItemData data = _fixedSlotData[slotIndex];

        if (data == null)
        {
            HideTooltip();
            return;
        }

        // 선택 상태 토글
        _fixedSlotSelected[slotIndex] = !_fixedSlotSelected[slotIndex];

        if (_fixedSlotSelected[slotIndex])
        {
            // 파란색 하이라이트
            fixedSlotImages[slotIndex].color = new Color(0.3f, 0.3f, 1f, 1f);
            ShowTooltip(data);
        }
        else
        {
            fixedSlotImages[slotIndex].color = Color.white;
            HideTooltip();
        }

        // ❗ 중요: 고정 슬롯은 믹스와 관계 없음
        // labSelector.OnItemClicked() 절대 호출 안 함
    }

    // ============================
    //  전체 슬롯 색/선택 초기화
    // ============================
    public void ResetAllSlotColors()
    {
        // 일반 슬롯
        if (slotImages != null && _slotSelected != null)
        {
            for (int i = 0; i < slotImages.Length; i++)
            {
                _slotSelected[i] = false;
                if (slotImages[i] != null)
                    slotImages[i].color = Color.white;
            }
        }

        // 고정 슬롯
        if (fixedSlotImages != null && _fixedSlotSelected != null)
        {
            for (int i = 0; i < fixedSlotImages.Length; i++)
            {
                _fixedSlotSelected[i] = false;
                if (fixedSlotImages[i] != null)
                    fixedSlotImages[i].color = Color.white;
            }
        }

        HideTooltip();
    }

    // ============================
    //  Tooltip
    // ============================
    public void ShowTooltip(ItemData data)
    {
        if (data == null) return;

        if (tooltipNameText != null)
            tooltipNameText.text = data.displayName;

        if (tooltipDescriptionText != null)
            tooltipDescriptionText.text = data.description;
    }

    public void HideTooltip()
    {
        if (tooltipNameText != null)
            tooltipNameText.text = string.Empty;

        if (tooltipDescriptionText != null)
            tooltipDescriptionText.text = string.Empty;
    }

    public void CloseInventory()
    {
        if (_isOpen)
        {
            _isOpen = false;

            if (inventoryPanel != null)
                inventoryPanel.SetActive(false);
        }
    }
}
