using UnityEngine;

[System.Serializable]
public class DialogueGroup
{
    public string groupName;
    [TextArea] public string[] lines;
}

public class NPCDialogue : MonoBehaviour
{
    public Transform player;
    public float interactDistance = 2f;

    // 도구 지급 설정 (수정된 부분)
    public GameObject toolPrefabToSpawn;
    public Transform spawnPoint;
    public bool isToolExchangeNPC = false;
    public string toolNameFlag = ""; // ⭐️ 획득할 도구 이름 (예: "Sickle", "Ladder")

    public DialogueGroup[] dialogueGroups;
    public DialogueUI dialogueUI;
    public GameObject pressEHint;

    // 아이템 지급 & NPC 제거 (기존 필드 유지)
    public GameObject rewardItemPrefab;
    public float itemSpawnOffset = 0.2f;

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

        // ⭐️ [추가] 대화 중에는 Enter 키로 다음 대화/종료 처리
        if (_isTalking && Input.GetKeyDown(KeyCode.Return))
        {
            OnClickNextFromUI();
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

        dialogueUI.ShowDialogue(this, text, hasNext);
    }

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

        if (isToolExchangeNPC)
        {
            if (toolPrefabToSpawn != null && spawnPoint != null)
            {
                Instantiate(toolPrefabToSpawn, spawnPoint.position, Quaternion.identity);
                Debug.Log("고양이 NPC가 도구를 스폰했습니다.");
            }

            // ⭐️ [수정된 핵심 로직]: 다중 도구 획득 플래그 활성화
            if (InventoryManager.Instance != null && !string.IsNullOrEmpty(toolNameFlag))
            {
                // InventoryManager의 AcquireTool 함수 호출
                InventoryManager.Instance.AcquireTool(toolNameFlag);
                Debug.Log($"플레이어가 도구 '{toolNameFlag}' 권한을 획득했습니다!");
            }

            Destroy(gameObject);
            return;
        }

        if (pressEHint != null && player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            bool canInteract = dist <= interactDistance;
            pressEHint.SetActive(canInteract);
        }
    }
}