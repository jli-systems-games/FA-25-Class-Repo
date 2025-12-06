using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class InventoryUI : MonoBehaviour
{
    [Header("인벤토리 전체 패널(켜지고 꺼지는 박스)")]
    public GameObject inventoryPanel;

    [Header("아이콘을 보여줄 슬롯 이미지들")]
    public Image[] slotImages;
    bool[] _slotSelected;

    [Header("랩 선택 (랩에서만 쓰고, 아니면 비워둬도 됨)")]
    public LabInventorySelector labSelector;   

    [Header("Tooltip Text Fields")]
    public TMP_Text tooltipNameText;              
    public TMP_Text tooltipDescriptionText;



    bool _isOpen = false;

    void Awake()
    {
        if (slotImages != null)
            _slotSelected = new bool[slotImages.Length];
    }

    void Start()
    {

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);


        HideTooltip();

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += Refresh;

        SetupSlotButtons();

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
                img.sprite = list[i].icon;
                img.color = Color.white;   
            }
            else
            {
                img.sprite = null;
                img.color = new Color(1f, 1f, 1f, 0f); 
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

            Button btn = img.GetComponent<Button>();
            if (btn == null)
                
                btn = img.GetComponentInParent<Button>();

            if (btn == null)
            {
      
                btn = img.gameObject.AddComponent<Button>();
            }


            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnSlotClicked(slotIndex));
        }
    }

    void OnSlotClicked(int slotIndex)
    {
        if (slotImages == null || slotIndex < 0 || slotIndex >= slotImages.Length)
            return;

        var mgr = InventoryManager.Instance;
        if (mgr == null || mgr.items == null) return;

        var list = mgr.items;
        if (slotIndex < 0 || slotIndex >= list.Count) return;

        ItemData data = list[slotIndex];

        if (data != null)
        {
            if (_slotSelected[slotIndex])
            {
                _slotSelected[slotIndex] = false;
                slotImages[slotIndex].color = Color.white;

                HideTooltip();
            }
            else
            {
                _slotSelected[slotIndex] = true;
                slotImages[slotIndex].color = new Color(1f, 0.3f, 0.3f, 1f);

                ShowTooltip(data);
            }
        }
        else
        {

            HideTooltip();
            return;
        }


        if (labSelector == null)
            return;

        if (data == null) return;

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

        HideTooltip();
    }

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
        {
            tooltipNameText.text = string.Empty;
        }
        if (tooltipDescriptionText != null)
        {
            tooltipDescriptionText.text = string.Empty;
        }
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