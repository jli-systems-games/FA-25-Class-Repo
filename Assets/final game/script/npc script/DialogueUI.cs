using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;      // 대화창 전체 패널
    public TMP_Text dialogueText; // 텍스트메시프로 텍스트
    public Button nextButton;     // "다음" 버튼
    public Button closeButton;    // "닫기" 버튼 (이것만 쓰시면 됩니다)

    NPCDialogue _currentNPC;

    void Awake()
    {
        // 시작할 때는 숨겨두기
        if (panel != null)
            panel.SetActive(false);
    }

    /// <summary>
    /// NPC가 대사를 보여달라고 요청할 때 호출
    /// </summary>
    public void ShowDialogue(NPCDialogue npc, string text, bool hasNext)
    {
        _currentNPC = npc;

        if (panel != null)
            panel.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = text;

        // hasNext가 true면 Next 버튼, false면 Close 버튼을 보여줍니다.
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

    // === 버튼에서 연결해서 쓸 메서드들 ===

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
            // ✅ NPC한테도 "이제 끝이야" 알려주기 -> EndDialogue() 호출 -> 아이템 스폰/NPC 제거 로직 실행
            _currentNPC.ForceEndFromUI();
        }
        else
        {
            Hide();
        }
    }
}