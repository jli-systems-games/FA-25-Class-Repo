using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectSceneCarousel : MonoBehaviour
{
    // ===== OnClick에서 직접 부를 public 메서드들 =====
    public void P1Up() { MoveIndex(ref p1Index, +1, forP1: true, forP2: false); }
    public void P1Down() { MoveIndex(ref p1Index, -1, forP1: true, forP2: false); }
    public void P2Up() { MoveIndex(ref p2Index, +1, forP1: false, forP2: true); }
    public void P2Down() { MoveIndex(ref p2Index, -1, forP1: false, forP2: true); }
    public void SubmitP1() { DoSubmitP1(); }
    public void SubmitP2() { DoSubmitP2(); }
    public void StartBattleBtn() { StartBattle(); }

    // ===== 데이터 & UI =====
    [Header("Roster (drag SOs in desired order)")]
    public AnimalStats[] roster;                // 동물 SO들을 원하는 순서대로

    [Header("P1 UI")]
    public Image p1Portrait;                 // 초상화 이미지
    public TMP_Text p1Name;                     // 이름(선택)
    public Image p1Frame;                    // 잠금 시 색 변경용 테두리/배경

    [Header("P2 UI")]
    public Image p2Portrait;
    public TMP_Text p2Name;
    public Image p2Frame;

    [Header("Helper & Start")]
    public TMP_Text helperText;                 // 안내 문구(선택)
    public string battleSceneName = "game";   // 다음 씬 이름

    [Header("Colors")]
    public Color p1Color = new Color(1f, 0.3f, 0.3f, 1f); // 잠금 시 P1 색
    public Color p2Color = new Color(0.3f, 0.5f, 1f, 1f); // 잠금 시 P2 색
    public Color idleColor = Color.white;                   // 잠금 전 색

    // ===== 내부 상태 =====
    int p1Index = 0;
    int p2Index = 1;
    bool p1Locked = false;
    bool p2Locked = false;

    void Start()
    {
        SelectionData.Clear();

        if (roster == null || roster.Length == 0)
        {
            Debug.LogError("Roster is empty. Drag AnimalStats SOs into 'roster'.");
            return;
        }

        RefreshPortraits();
        UpdateFrames();
        UpdateHelper("(1) P1이 캐릭터를 고르고 Submit P1을 누르세요.");
    }

    // ===== 내부 로직 =====
    void MoveIndex(ref int idx, int delta, bool forP1, bool forP2)
    {
        if (forP1 && p1Locked) return;
        if (forP2 && p2Locked) return;

        int len = roster.Length;
        idx = (idx + (delta % len) + len) % len; // 순환 인덱스
        RefreshPortraits();
    }

    void RefreshPortraits()
    {
        if (p1Portrait) p1Portrait.sprite = roster[p1Index]?.sprite;
        if (p2Portrait) p2Portrait.sprite = roster[p2Index]?.sprite;
        if (p1Name) p1Name.text = roster[p1Index]?.displayName ?? "";
        if (p2Name) p2Name.text = roster[p2Index]?.displayName ?? "";
    }

    void DoSubmitP1()
    {
        if (p1Locked) return;
        SelectionData.P1 = roster[p1Index];
        p1Locked = true;
        UpdateFrames();
        UpdateHelper(SelectionData.P2 == null ? "(2) 이제 P2가 고르고 Submit P2를 누르세요."
                                              : "두 플레이어 완료! Start를 누르세요.");
    }

    void DoSubmitP2()
    {
        if (p2Locked) return;
        SelectionData.P2 = roster[p2Index];
        p2Locked = true;
        UpdateFrames();
        UpdateHelper(SelectionData.P1 == null ? "(1) P1이 고르고 Submit P1을 누르세요."
                                              : "두 플레이어 완료! Start를 누르세요.");
    }

    void UpdateFrames()
    {
        // 테두리/배경 색으로 잠금 상태 표현
        if (p1Frame) p1Frame.color = p1Locked ? p1Color : idleColor;
        if (p2Frame) p2Frame.color = p2Locked ? p2Color : idleColor;
    }

    void StartBattle()
    {
        if (SelectionData.P1 == null || SelectionData.P2 == null)
        {
            UpdateHelper("양쪽 캐릭터를 모두 Submit 해주세요.");
            return;
        }
        SceneManager.LoadScene(battleSceneName);
    }

    void UpdateHelper(string msg)
    {
        if (helperText) helperText.text = msg;
    }
}
