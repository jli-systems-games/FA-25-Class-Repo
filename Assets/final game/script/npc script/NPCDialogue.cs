using UnityEngine;

[System.Serializable]
public class DialogueGroup
{
    [Tooltip("그룹 이름(예: A, B, C...) 그냥 구분용")]
    public string groupName;

    [Tooltip("이 그룹이 말하는 실제 대사들 (a-1, a-2 같은 것)")]
    [TextArea] public string[] lines;
}

public class NPCDialogue : MonoBehaviour
{
    [Header("플레이어 인식")]
    public Transform player;
    public float interactDistance = 2f;

    [Header("대화 데이터 (A~E 그룹들)")]
    public DialogueGroup[] dialogueGroups;

    [Header("UI 매니저")]
    public DialogueUI dialogueUI;

    bool _isTalking = false;
    int _currentGroupIndex = -1;
    int _currentLineIndex = 0;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (dialogueUI == null)
        {
            dialogueUI = FindFirstObjectByType<DialogueUI>();
        }
    }

    void Update()
    {
        if (player == null || dialogueUI == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool canInteract = dist <= interactDistance;

        // 플레이어가 가까이 있고, E를 눌렀을 때
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            // 이미 대화 중이면 그냥 무시 (원하면 여기서 다음으로 넘기게 해도 됨)
            if (_isTalking) return;

            StartNewDialogue();
        }
    }

    void StartNewDialogue()
    {
        if (dialogueGroups == null || dialogueGroups.Length == 0)
            return;

        _isTalking = true;

        // 매번 새로 대화를 시작할 때마다 A~E 중 하나 랜덤 뽑기
        _currentGroupIndex = Random.Range(0, dialogueGroups.Length);
        _currentLineIndex = 0;

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (_currentGroupIndex < 0 || _currentGroupIndex >= dialogueGroups.Length)
            return;

        DialogueGroup group = dialogueGroups[_currentGroupIndex];
        if (group.lines == null || group.lines.Length == 0) return;

        string text = group.lines[_currentLineIndex];
        bool hasNext = (_currentLineIndex < group.lines.Length - 1);

        dialogueUI.ShowDialogue(this, text, hasNext);
    }

    // === UI에서 호출 ===
    public void OnClickNextFromUI()
    {
        DialogueGroup group = dialogueGroups[_currentGroupIndex];

        _currentLineIndex++;

        if (_currentLineIndex < group.lines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    // Close 버튼이 누를 때 호출
    public void ForceEndFromUI()
    {
        EndDialogue();
    }

    void EndDialogue()
    {
        _isTalking = false;
        _currentGroupIndex = -1;
        _currentLineIndex = 0;

        if (dialogueUI != null)
        {
            dialogueUI.Hide();
        }
    }
}
