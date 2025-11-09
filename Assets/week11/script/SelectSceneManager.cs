using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectSceneManager : MonoBehaviour
{
    [Header("Roster (SO들)")]
    public AnimalStats panda, chick, rabbit, turtle, cat;

    [Header("UI - Portrait (선택 미리보기)")]
    public Image p1Portrait;
    public Image p2Portrait;

    [Header("다음 씬 이름")]
    public string battleSceneName = "game"; // 전투 씬 이름

    void Start()
    {
        SelectionData.Clear();
        if (p1Portrait) p1Portrait.sprite = null;
        if (p2Portrait) p2Portrait.sprite = null;
    }

    // --- 버튼에서 호출 (P1) ---
    public void PickP1(string name)
    {
        SelectionData.P1 = NameToStats(name);
        if (p1Portrait && SelectionData.P1) p1Portrait.sprite = SelectionData.P1.sprite;
    }

    // --- 버튼에서 호출 (P2) ---
    public void PickP2(string name)
    {
        SelectionData.P2 = NameToStats(name);
        if (p2Portrait && SelectionData.P2) p2Portrait.sprite = SelectionData.P2.sprite;
    }

    public void StartBattle()
    {
        if (SelectionData.P1 == null || SelectionData.P2 == null)
        {
            Debug.LogWarning("양쪽 캐릭터를 먼저 선택하세요.");
            return;
        }
        SceneManager.LoadScene(battleSceneName);
    }

    AnimalStats NameToStats(string n)
    {
        switch (n)
        {
            case "Panda": return panda;
            case "Chick": return chick;
            case "Rabbit": return rabbit;
            case "Turtle": return turtle;
            case "Cat": return cat;
        }
        return null;
    }
}
