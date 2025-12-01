// === NPCDialogue.cs ===

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

        // ⚠️ 필수 연결 필드가 누락되었는지 확인 (디버그용)
        if (player == null) Debug.LogError(gameObject.name + ": 'Player' 오브젝트를 찾을 수 없습니다. 태그를 확인하세요.");
        if (dialogueUI == null) Debug.LogError(gameObject.name + ": 'DialogueUI' 오브젝트를 찾을 수 없습니다. 씬에 있는지 확인하세요.");
    }

    void Update()
    {
        // 필수 요소 중 하나라도 없으면 여기서 즉시 종료
        if (player == null || dialogueUI == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool canInteract = dist <= interactDistance;

        // 대화 중이 아니면, 거리 안에 있을 때만 E 텍스트 보이기
        if (pressEHint != null)
        {
            if (!_isTalking && canInteract)
                pressEHint.SetActive(true);
            else
                pressEHint.SetActive(false);
        }

        // 플레이어가 가까이 있고, E를 눌렀을 때
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            // 이미 대화 중이면 무시
            if (_isTalking) return;

            // 대화를 시작하니까 E 텍스트는 숨기기
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

        // dialogueUI의 ShowDialogue 함수가 DialogueUI 컴포넌트에서 구현되어 있어야 합니다.
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

    // Close 버튼이 눌릴 때 호출
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

        // 대화가 끝났을 때, 아직 근처에 있으면 E 텍스트 다시 보여주기
        if (pressEHint != null && player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            bool canInteract = dist <= interactDistance;
            pressEHint.SetActive(canInteract);
        }
    }
}