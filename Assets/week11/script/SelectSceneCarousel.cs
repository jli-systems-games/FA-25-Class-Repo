using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectSceneCarousel : MonoBehaviour
{

    public void P1Up() { MoveIndex(ref p1Index, +1, forP1: true, forP2: false); }
    public void P1Down() { MoveIndex(ref p1Index, -1, forP1: true, forP2: false); }
    public void P2Up() { MoveIndex(ref p2Index, +1, forP1: false, forP2: true); }
    public void P2Down() { MoveIndex(ref p2Index, -1, forP1: false, forP2: true); }
    public void SubmitP1() { DoSubmitP1(); }
    public void SubmitP2() { DoSubmitP2(); }
    public void StartBattleBtn() { StartBattle(); }

    [Header("Roster (drag SOs in desired order)")]
    public AnimalStats[] roster;

    [Header("P1 UI")]
    public Image p1Portrait;         // 선택 아이콘(색을 바꿀 대상)
    public TMP_Text p1Name;

    [Header("P2 UI")]
    public Image p2Portrait;         // 선택 아이콘(색을 바꿀 대상)
    public TMP_Text p2Name;

    [Header("Helper & Start")]
    public TMP_Text helperText;
    public string battleSceneName = "game";

    [Header("Tint Colors")]
    public Color p1Tint = new Color(1f, 0.45f, 0.45f, 1f); // P1 잠금 색
    public Color p2Tint = new Color(0.45f, 0.65f, 1f, 1f); // P2 잠금 색
    public Color idleTint = Color.white;                   // 잠금 전(기본) 색

    [Header("Rules")]
    public bool disallowDuplicate = true;   // 중복 선택 금지

    // 내부 상태
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

        if (disallowDuplicate && p1Index == p2Index) p2Index = (p2Index + 1) % roster.Length;

        RefreshPortraits();
        UpdateTintVisuals();   // 초기 색상 적용
        UpdateHelper("(1) P1이 캐릭터를 고르고 Submit P1을 누르세요.");
    }

    void MoveIndex(ref int idx, int delta, bool forP1, bool forP2)
    {
        if (forP1 && p1Locked) return;
        if (forP2 && p2Locked) return;

        int len = roster.Length;
        idx = (idx + (delta % len) + len) % len; // 순환 인덱스

        // 상대가 잠갔고 중복 금지면 그 인덱스 건너뛰기
        if (disallowDuplicate)
        {
            if (forP1 && p2Locked && idx == p2Index)
                idx = (idx + (delta >= 0 ? +1 : -1) + len) % len;

            if (forP2 && p1Locked && idx == p1Index)
                idx = (idx + (delta >= 0 ? +1 : -1) + len) % len;
        }

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

        if (disallowDuplicate && p2Locked && roster[p1Index] == roster[p2Index])
        { UpdateHelper("P2가 이미 그 캐릭터를 선택했어요. 다른 캐릭터를 골라주세요."); return; }

        SelectionData.P1 = roster[p1Index];
        p1Locked = true;
        UpdateTintVisuals();  
        UpdateHelper(SelectionData.P2 == null ? "(2) 이제 P2가 고르고 Submit P2를 누르세요."
                                              : "두 플레이어 완료! Start를 누르세요.");
    }

    void DoSubmitP2()
    {
        if (p2Locked) return;

        if (disallowDuplicate && p1Locked && roster[p2Index] == roster[p1Index])
        { UpdateHelper("P1이 이미 그 캐릭터를 선택했어요. 다른 캐릭터를 골라주세요."); return; }

        SelectionData.P2 = roster[p2Index];
        p2Locked = true;
        UpdateTintVisuals();  
        UpdateHelper(SelectionData.P1 == null ? "(1) P1이 고르고 Submit P1을 누르세요."
                                              : "두 플레이어 완료! Start를 누르세요.");
    }

    void UpdateTintVisuals()
    {
        if (p1Portrait) p1Portrait.color = p1Locked ? p1Tint : idleTint;
        if (p2Portrait) p2Portrait.color = p2Locked ? p2Tint : idleTint;
    }

    void StartBattle()
    {
        if (SelectionData.P1 == null || SelectionData.P2 == null)
        { UpdateHelper("양쪽 캐릭터를 모두 Submit 해주세요."); return; }

        if (disallowDuplicate && SelectionData.P1 == SelectionData.P2)
        { UpdateHelper("두 플레이어가 서로 다른 캐릭터여야 합니다."); return; }

        SceneManager.LoadScene(battleSceneName);
    }

    void UpdateHelper(string msg)
    {
        if (helperText) helperText.text = msg;
    }
}
