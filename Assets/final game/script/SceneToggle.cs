using UnityEngine;
using TMPro;

public class SceneToggle : MonoBehaviour
{
    public TMP_Text hintText;

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
        SwitchToWorld();
        UpdateHint();
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

    void UpdateHint()
    {
        if (hintText == null) return;

        if (_inLab)
            hintText.text = "Press \"R\" to return to the World.";
        else
            hintText.text = "Press \"R\" to enter the Lab.";
    }


    void SwitchToLab()
    {
        _inLab = true;

        if (worldCamera != null) worldCamera.enabled = false;
        if (worldController != null) worldController.enabled = false;

        if (labCamera != null) labCamera.enabled = true;
        if (labRoot != null) labRoot.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateHint();
    }

    void SwitchToWorld()
    {
        _inLab = false;

        if (labCamera != null) labCamera.enabled = false;
        if (labRoot != null) labRoot.SetActive(false);

        if (worldCamera != null) worldCamera.enabled = true;
        if (worldController != null) worldController.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateHint();
    }

}