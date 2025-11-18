using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;      // 대화창 전체 패널
    public TMP_Text dialogueText; // 텍스트메시프로
    public Button nextButton;     // "다음" 버튼
    public Button closeButton;    // "닫기" 버튼

    NPCDialogue _currentNPC;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowDialogue(NPCDialogue npc, string text, bool hasNext)
    {
        _currentNPC = npc;

        if (panel != null)
            panel.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = text;

        if (nextButton != null)
            nextButton.gameObject.SetActive(hasNext);

        if (closeButton != null)
            closeButton.gameObject.SetActive(!hasNext);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);

        _currentNPC = null;
    }

    // === 버튼 연결용 ===
    public void OnClickNext()
    {
        if (_currentNPC != null)
        {
            _currentNPC.OnClickNextFromUI();
        }
    }

    public void OnClickClose()
    {
        if (_currentNPC != null)
        {
            // ✅ NPC한테도 "이제 끝이야" 알려주기
            _currentNPC.ForceEndFromUI();
        }
        else
        {
            Hide();
        }
    }
}
