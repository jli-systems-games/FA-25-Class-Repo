// GameTimer.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Timer")]
    public float duration = 65f;          // 총 제한시간(초)
    public TMP_Text timerText;            // 타이머 표시용 TMP 텍스트

    [Header("End UI")]
    public GameObject endPanel;           // 종료 UI 패널 (처음엔 비활성화)
    public TMP_Text endMessage;           // "성공/실패" 안내 문구(옵션)
    public Button replayButton;           // 리플레이 버튼

    bool ended;
    float remain;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 초기 상태 세팅
        remain = duration;
        if (endPanel) endPanel.SetActive(false);

        // 리플레이 버튼 연결
        if (replayButton) replayButton.onClick.AddListener(Reload);

        // 카운트다운 시작
        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while (!ended && remain > 0f)
        {
            remain -= Time.deltaTime;
            UpdateTimerUI(Mathf.Max(remain, 0f));
            yield return null;
        }

        if (!ended)
        {
            // 시간초과 → 패배 처리
            EndGame(success: false);
        }
    }

    void UpdateTimerUI(float t)
    {
        if (!timerText) return;
        int sec = Mathf.CeilToInt(t);
        int m = sec / 60;
        int s = sec % 60;
        timerText.text = $"{m:00}:{s:00}";
    }

    // 외부에서 목표 트리거에 닿았을 때 호출
    public void HitGoal()
    {
        if (ended) return;
        EndGame(success: true);
    }

    void EndGame(bool success)
    {
        ended = true;

        // 게임 일시정지
        Time.timeScale = 0f;

        // 종료 UI 오픈
        if (endPanel) endPanel.SetActive(true);
        if (endMessage)
            endMessage.text = success ? "성공!" : "시간 초과!";

        // 필요시 커서 표시
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Reload()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
