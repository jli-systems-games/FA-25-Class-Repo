using UnityEngine;
// InventoryManager를 사용하기 위해 필요
using System.Linq;

[System.Serializable]
public class DialogueGroup
{
    public string groupName;
    [TextArea] public string[] lines;
}

public class NPCDialogue : MonoBehaviour
{
    // ⭐️ 퀘스트 마커 필드 추가
    public GameObject questMarker;

    public Transform player;
    public float interactDistance = 2f;

    // 도구 지급 설정
    public GameObject toolPrefabToSpawn;
    public Transform spawnPoint;
    public bool isToolExchangeNPC = false;
    public string toolNameFlag = "";

    public DialogueGroup[] dialogueGroups;
    public DialogueUI dialogueUI;
    public GameObject pressEHint;

    // 아이템 지급 & NPC 제거 (기존 필드 유지)
    public GameObject rewardItemPrefab;
    public float itemSpawnOffset = 0.2f;

    bool _isTalking = false;
    // -1로 초기화하여 StartNewDialogue()에서 그룹이 랜덤으로 선택됨을 명시
    int _currentGroupIndex = -1;
    // 인덱스는 0으로 유지. ShowCurrentLine()이 인덱스 0을 보여주므로 이 방식이 맞습니다.
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
            // FindFirstObjectByType는 씬에서 해당 타입의 첫 번째 오브젝트를 찾습니다.
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
            // 대화 중이 아니고 상호작용 가능할 때만 E 힌트 표시
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

        // 대화 중에는 Enter 키로 다음 대화/종료 처리
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

        // ⭐️ [퀘스트 마커 숨기기 로직 추가]
        if (questMarker != null)
        {
            questMarker.SetActive(false);
        }

        // 현재는 랜덤 그룹 선택으로 되어 있으므로 유지
        _currentGroupIndex = Random.Range(0, dialogueGroups.Length);
        _currentLineIndex = 0; // 첫 번째 대사를 보여주기 위해 0으로 시작

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (_currentGroupIndex < 0 || _currentGroupIndex >= dialogueGroups.Length)
            return;

        DialogueGroup group = dialogueGroups[_currentGroupIndex];
        // 🚨 중요: 여기서 lines가 비어있으면 바로 EndDialogue() 호출됩니다.
        if (group.lines == null || group.lines.Length == 0)
        {
            EndDialogue();
            return;
        }

        if (_currentLineIndex >= group.lines.Length)
        {
            EndDialogue();
            return;
        }

        string text = group.lines[_currentLineIndex];
        // 다음 대사가 남아 있는지 확인 (다음 버튼을 보여줄지 결정)
        bool hasNext = (_currentLineIndex < group.lines.Length - 1);

        dialogueUI.ShowDialogue(this, text, hasNext);
    }

    public void OnClickNextFromUI()
    {
        // _currentGroupIndex가 유효한지 확인
        if (_currentGroupIndex < 0 || _currentGroupIndex >= dialogueGroups.Length)
        {
            EndDialogue();
            return;
        }

        DialogueGroup group = dialogueGroups[_currentGroupIndex];

        _currentLineIndex++; // 인덱스 증가 (다음 대사로)

        if (_currentLineIndex < group.lines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            EndDialogue(); // 모든 대사 종료
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
        _currentLineIndex = 0; // 다음 대화 시작을 위해 0으로 초기화 유지

        if (dialogueUI != null)
        {
            dialogueUI.Hide();
        }

        if (isToolExchangeNPC)
        {
            // 도구 스폰 및 획득 로직
            if (toolPrefabToSpawn != null && spawnPoint != null)
            {
                Instantiate(toolPrefabToSpawn, spawnPoint.position, Quaternion.identity);
                Debug.Log("NPC가 도구를 스폰했습니다.");
            }

            if (InventoryManager.Instance != null && !string.IsNullOrEmpty(toolNameFlag))
            {
                // InventoryManager의 AcquireTool 함수 호출
                InventoryManager.Instance.AcquireTool(toolNameFlag);
                Debug.Log($"플레이어가 도구 '{toolNameFlag}' 권한을 획득했습니다!");
            }

            Destroy(gameObject);
            return;
        }

        // 대화 종료 후 E 힌트 재활성화 로직
        if (pressEHint != null && player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            bool canInteract = dist <= interactDistance;
            pressEHint.SetActive(canInteract);
        }
    }
}