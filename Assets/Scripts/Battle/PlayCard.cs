using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayCard : MonoBehaviour
{
    public Button[] handButtons = new Button[9];
    public Image[] handImages = new Image[9];

    public Sprite rockSprite;
    public Sprite paperSprite;
    public Sprite scissorSprite;
    public Sprite usedSprite;

    public TMP_Text selectedText;
    public Button readyBtn;

    HandType[] hand;
    bool[] used;
    int selectedIndex = -1;

    public System.Action OnReady;

    void Awake()
    {
        for (int i = 0; i < handButtons.Length; i++)
        {
            int idx = i;
            if (handButtons[i])
                handButtons[i].onClick.AddListener(() => SelectIndex(idx));
        }
        if (readyBtn)
            readyBtn.onClick.AddListener(() => { if (selectedIndex >= 0) OnReady?.Invoke(); });
    }

    public void SetHand(HandType[] cards)
    {
        if (cards == null || cards.Length != 9)
        {
            return;
        }

        hand = (HandType[])cards.Clone();

        used = new bool[hand.Length];
        selectedIndex = -1;

        UpdateUI();
    }

    public void SelectIndex(int idx)
    {
        if (hand == null) return;
        if (idx < 0 || idx >= hand.Length) return;
        if (used[idx]) return;
        selectedIndex = idx;
        UpdateUI();
    }

    public bool HasSelection => selectedIndex >= 0 && hand != null;
    public HandType GetSelectedType() => hand[selectedIndex];

    public void ConsumeSelected()
    {
        if (!HasSelection) return;
        used[selectedIndex] = true;
        selectedIndex = -1;
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < handImages.Length; i++)
        {
            var img = handImages[i];
            var btn = handButtons[i];
            if (!img || !btn) continue;

            if (hand == null || i >= hand.Length)
            {
                btn.interactable = false;
                img.color = new Color(1, 1, 1, 0.25f);
                continue;
            }

            img.sprite = ToSprite(hand[i]);
            img.color = Color.white;

            if (used[i])
            {
                btn.interactable = false;
                if (usedSprite) img.sprite = usedSprite;
                img.color = new Color(1, 1, 1, 0.25f);
            }
            else
            {
                btn.interactable = true;
                if (i == selectedIndex) img.color = Color.white;
            }
        }

        if (selectedText)
            selectedText.text = "Selected: " + (HasSelection ? hand[selectedIndex].ToString() : " ");

        if (readyBtn)
            readyBtn.interactable = HasSelection;
    }

    Sprite ToSprite(HandType h)
    {
        switch (h)
        {
            case HandType.Rock: return rockSprite;
            case HandType.Paper: return paperSprite;
            case HandType.Scissor: return scissorSprite;
        }
        return rockSprite;
    }
}