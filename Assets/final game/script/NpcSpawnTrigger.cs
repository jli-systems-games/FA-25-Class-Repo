using UnityEngine;

public class NpcSpawnTrigger : MonoBehaviour
{
    [Header("활성화할 NPC 오브젝트 (비활성화 상태로 시작해야 함)")]
    public GameObject targetNpc;

    [Header("NPC를 등장시킬 필요한 아이템 개수")]
    public int requiredItemCount = 4;

    private bool _isTriggered = false; // 한 번만 활성화되도록 제어하는 플래그

    private void Start()
    {
        // 씬 시작 시 타겟 NPC는 반드시 비활성화 상태여야 합니다.
        if (targetNpc != null)
        {
            // 스크립트 시작 시 Inspector의 설정에 따라 비활성화합니다.
            targetNpc.SetActive(false);
        }

        // InventoryManager의 이벤트를 구독하여 아이템이 추가될 때마다 체크합니다.
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += CheckInventoryCount;
        }

        // 씬 로딩 시 인벤토리가 이미 채워져 있을 경우를 대비하여 한 번 체크
        CheckInventoryCount();
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 구독을 해제합니다.
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= CheckInventoryCount;
        }
    }

    private void CheckInventoryCount()
    {
        if (_isTriggered) return; // 이미 활성화되었으면 다시 체크하지 않습니다.
        if (InventoryManager.Instance == null) return;

        // InventoryManager에서 현재 아이템 개수를 가져옵니다.
        int currentCount = InventoryManager.Instance.GetTotalItemCount();

        Debug.Log($"현재 아이템 개수: {currentCount}/{requiredItemCount}");

        if (currentCount >= requiredItemCount)
        {
            // ⭐️ 조건 충족 시 NPC 활성화
            if (targetNpc != null)
            {
                targetNpc.SetActive(true);
                Debug.Log($"NPC '{targetNpc.name}'가 활성화되었습니다!");

                _isTriggered = true;
                // 목표 달성 후 더 이상 체크할 필요 없으면 구독 해제
                InventoryManager.Instance.OnInventoryChanged -= CheckInventoryCount;
            }
        }
    }
}