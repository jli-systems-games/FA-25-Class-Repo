using UnityEngine;
using System.Collections.Generic;

public class QuestSequencer : MonoBehaviour
{
    // ⭐️ Inspector에서 순서대로 추적할 NPC들을 연결합니다.
    [Header("순서대로 추적할 NPC 목록 (NPC GameObjects)")]
    public List<GameObject> sequentialNpcs;

    // ⭐️ NPC 머리 위에 붙일 마커 프리팹 (BillboardMarker 스크립트가 붙어있어야 함)
    [Header("NPC 마커 프리팹 (World Space)")]
    public GameObject markerPrefab;

    private int _currentIndex = 0;
    private GameObject _currentMarkerInstance = null;

    public static QuestSequencer Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 씬 시작 시 첫 번째 NPC를 목표로 설정
        StartQuest(0);
    }

    // ⭐️ 특정 인덱스의 NPC를 현재 목표로 설정
    private void StartQuest(int index)
    {
        if (index < 0 || index >= sequentialNpcs.Count)
        {
            Debug.Log("🎉 모든 퀘스트 NPC와의 상호작용이 완료되었습니다!");
            ClearMarker();
            return;
        }

        _currentIndex = index;

        // 이전 마커를 제거하고 새 마커를 부착합니다.
        ClearMarker();

        GameObject nextNpc = sequentialNpcs[_currentIndex];

        if (nextNpc != null)
        {
            // 마커를 NPC 머리 위에 부착
            _currentMarkerInstance = Instantiate(markerPrefab, nextNpc.transform);
            // 마커의 로컬 위치를 NPC 머리 위로 조정 (여기서는 0, 2, 0 예시)
            _currentMarkerInstance.transform.localPosition = new Vector3(0, 2, 0);
            Debug.Log($"➡️ 다음 퀘스트 대상: {nextNpc.name}");
        }
        else
        {
            Debug.LogWarning($"인덱스 {_currentIndex}의 NPC가 Null입니다. 다음 NPC로 넘어갑니다.");
            GoToNextNpc(); // Null이면 바로 다음 NPC를 찾습니다.
        }
    }

    private void ClearMarker()
    {
        if (_currentMarkerInstance != null)
        {
            Destroy(_currentMarkerInstance);
            _currentMarkerInstance = null;
        }
    }

    // ⭐️ 외부에서 호출하여 다음 퀘스트 대상으로 넘어가는 함수
    public void GoToNextNpc()
    {
        // NPC를 비활성화하는 역할은 NPCDialogue가 담당합니다.
        StartQuest(_currentIndex + 1);
    }
}