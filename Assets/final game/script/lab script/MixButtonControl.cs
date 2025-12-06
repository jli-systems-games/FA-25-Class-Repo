using UnityEngine;

public class MixButtonControl : MonoBehaviour
{
    // ⭐️ Mix 버튼 UI GameObject 자체를 연결합니다. (Inspector에서 연결 필요)
    [Header("믹스 버튼 UI (GameObject)")]
    public GameObject mixButtonUI;

    // 선택 초기화 기능을 위해 LabSelector 컴포넌트는 유지합니다. (Inspector에서 연결 필요)
    [Header("랩 인벤토리 선택기 (ClearSelection 용)")]
    public LabInventorySelector labSelector;

    void Start()
    {
        if (mixButtonUI != null)
        {
            mixButtonUI.SetActive(false);
        }
    }

    // 큐브가 콜라이더에 들어왔을 때
    void OnTriggerEnter(Collider other)
    {
        if (mixButtonUI != null)
        {
            mixButtonUI.SetActive(true);
        }
    }

    // ⭐️ OnTriggerExit은 비활성화 시 작동하지 않으므로, 이 코드는 백업용입니다.
    void OnTriggerExit(Collider other)
    {
        // 큐브가 자연스럽게 나갔을 때만 작동
        if (mixButtonUI != null)
        {
            mixButtonUI.SetActive(false);
        }

        if (labSelector != null)
        {
            labSelector.ClearSelection();
        }
    }

    // ⭐️ [새로운 핵심 함수]: 큐브가 사라질 때 외부에서 호출하여 버튼을 강제 종료합니다.
    public void ForceButtonOff()
    {
        if (mixButtonUI != null)
        {
            mixButtonUI.SetActive(false);
        }

        if (labSelector != null)
        {
            labSelector.ClearSelection();
        }
    }
}