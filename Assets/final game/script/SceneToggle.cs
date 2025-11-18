using UnityEngine;

public class SceneToggle : MonoBehaviour
{
    [Header("마을(월드) 모드")]
    public Camera worldCamera;             // 골드플레이어 카메라
    public MonoBehaviour worldController;  // 골드플레이어 움직임 스크립트 (예: GoldPlayerController)

    [Header("랩 모드")]
    public Camera labCamera;               // 랩에서 냄비/국자 보는 카메라
    public GameObject labRoot;             // 랩 UI나 오브젝트 묶음 (없으면 비워도 됨)

    [Header("키 설정")]
    public KeyCode toggleKey = KeyCode.R;  // R 키로 모드 전환

    bool _inLab = false;

    void Start()
    {
        // 시작은 마을 모드 기준이라고 가정
        SwitchToWorld();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (_inLab)
                SwitchToWorld();
            else
                SwitchToLab();
        }
    }

    void SwitchToLab()
    {
        _inLab = true;

        // 월드 모드 비활성화
        if (worldCamera != null) worldCamera.enabled = false;
        if (worldController != null) worldController.enabled = false;

        // 랩 모드 활성화
        if (labCamera != null) labCamera.enabled = true;
        if (labRoot != null) labRoot.SetActive(true);

        // 랩에서는 커서가 꼭 필요하니까
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void SwitchToWorld()
    {
        _inLab = false;

        // 랩 모드 비활성화
        if (labCamera != null) labCamera.enabled = false;
        if (labRoot != null) labRoot.SetActive(false);

        // 월드 모드 활성화
        if (worldCamera != null) worldCamera.enabled = true;
        if (worldController != null) worldController.enabled = true;

        // 이 게임은 마을에서도 커서가 필요한 구조니까
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}