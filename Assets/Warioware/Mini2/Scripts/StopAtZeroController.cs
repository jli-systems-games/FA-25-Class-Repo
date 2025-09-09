using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StopAtZeroController : MonoBehaviour
{
    [Header("Timer")]
    public float duration = 3f;          // 전체 제한 시간(초)
    public float successWindow = 0.15f;  // 이 이하로 남았을 때 누르면 성공
    float remaining;
    bool ended;

    [Header("UI")]
    public Image barFill;       // Image(type=Filled, Fill Method=Horizontal)
    public TMP_Text timeText;   // 선택(없으면 비워도 됨)
    public Button stopButton;   // STOP 버튼
    public GameObject successUI; // 성공 팝업(비활성 시작 권장)
    public float successShowTime = 0.6f;

    [Header("Scenes")]
    public string nextScene = "Mini02";  // 성공 시 이동할 씬
    public string failScene = "Fail";    // 실패 씬

    void Start()
    {
        remaining = duration;
        if (successUI) successUI.SetActive(false);
        if (stopButton)
        {
            stopButton.onClick.RemoveAllListeners();
            stopButton.onClick.AddListener(OnStopClicked);
        }
        UpdateUI();
    }

    void Update()
    {
        if (ended) return;

        remaining -= Time.deltaTime;
        if (remaining <= 0f)
        {
            remaining = 0f;
            UpdateUI();
            Fail();
            return;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        if (barFill) barFill.fillAmount = Mathf.InverseLerp(0f, duration, remaining);
        if (timeText) timeText.text = $"{remaining:0.00}s"; // 선택
    }

    void OnStopClicked()
    {
        if (ended) return;

      
        if (remaining > 0f && remaining <= successWindow)
        {
            Success();
        }
        else
        {
            Fail();
        }
    }

    void Success()
    {
        ended = true;
        if (successUI)
        {
            successUI.SetActive(true);
            Invoke(nameof(GoNext), successShowTime);
        }
        else
        {
            GoNext();
        }
        if (stopButton) stopButton.interactable = false;
    }

    void Fail()
    {
        ended = true;
        if (stopButton) stopButton.interactable = false;
        SceneManager.LoadScene(failScene);
    }

    void GoNext()
    {
        SceneManager.LoadScene(nextScene);
    }
}
