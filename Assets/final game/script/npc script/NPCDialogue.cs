
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

    [Header("E 키 안내 텍스트 (TMP 들어있는 오브젝트)")]
    public GameObject pressEHint;   // ← 여기에 TextMeshPro가 들어있는 오브젝트 연결

    // 💡 아이템 지급 설정 (유형 2: 사라지는 NPC)
    [Header("💡 아이템 지급 & NPC 제거 (선택 사항)")]
    [Tooltip("지급할 아이템 프리팹. 연결하면 대화 종료 후 NPC가 사라지면서 아이템이 스폰됨.")]
    public GameObject rewardItemPrefab;
    [Tooltip("아이템을 스폰할 때 NPC 발밑에서 띄울 높이")]
    public float itemSpawnOffset = 0.2f;

    bool _isTalking = false;
    int _currentGroupIndex = -1;
    int _currentLineIndex = 0;

    void Start()
    {
        // 플레이어 자동 연결 시도
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // DialogueUI 자동 연결 시도 (씬에 하나만 존재해야 함)
        if (dialogueUI == null)
        {
            dialogueUI = FindFirstObjectByType<DialogueUI>();
        }

        // 시작할 때 E 텍스트는 꺼둠 (Hierarchy에서 비활성화하지 않았다면)
        if (pressEHint != null)
            pressEHint.SetActive(false);
    }

    void Update()
    {
        if (player == null || dialogueUI == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool canInteract = dist <= interactDistance;

        if (pressEHint != null)
        {
            if (!_isTalking && canInteract)
                pressEHint.SetActive(true);
            else
                pressEHint.SetActive(false);
        }

        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            if (_isTalking) return;

            if (pressEHint != null)
                pressEHint.SetActive(false);

            StartNewDialogue();
        }
    }

    void StartNewDialogue()
    {
        if (dialogueGroups == null || dialogueGroups.Length == 0)
            return;

        _isTalking = true;

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

        if (_currentLineIndex >= group.lines.Length)
        {
            EndDialogue();
            return;
        }

        string text = group.lines[_currentLineIndex];
        bool hasNext = (_currentLineIndex < group.lines.Length - 1);

        // DialogueUI는 Next/Close 버튼을 hasNext에 따라 알아서 토글합니다.
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
            // 마지막 줄에서 Next를 누르면 대화 종료 (Close 버튼을 누른 것과 동일)
            EndDialogue();
        }
    }

    // Close 버튼이 눌릴 때 호출
    public void ForceEndFromUI()
    {
        EndDialogue();
    }

    void EndDialogue()
    {
        // 1. 대화 종료 상태 초기화
        _isTalking = false;
        _currentGroupIndex = -1;
        _currentLineIndex = 0;

        // 2. UI 숨기기
        if (dialogueUI != null)
        {
            dialogueUI.Hide();
        }

        // 💡 아이템 지급 설정이 되어 있는 경우 (유형 2: 사라지는 NPC)
        if (rewardItemPrefab != null)
        {
            // ⭐️ 디버그 로그! 이 메시지가 콘솔에 찍혀야 합니다.
            Debug.Log($"✅ 아이템 ({rewardItemPrefab.name}) 스폰 및 NPC 제거 시작.");

            // NPC가 서 있던 위치에 아이템 스폰
            Vector3 spawnPos = transform.position + Vector3.up * itemSpawnOffset;
            Instantiate(rewardItemPrefab, spawnPos, Quaternion.identity);

            // ⭐️ NPC 자신을 월드에서 제거
            Destroy(gameObject);
            return;
        }

        // 3. (유형 1: 남아있는 NPC) E 텍스트 다시 보여주기
        if (pressEHint != null && player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            bool canInteract = dist <= interactDistance;
            pressEHint.SetActive(canInteract);
        }
    }
}