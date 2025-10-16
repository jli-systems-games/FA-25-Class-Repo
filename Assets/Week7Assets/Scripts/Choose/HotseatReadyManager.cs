using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class HotseatReadyManager : MonoBehaviour
{
    public CanvasGroup p1Area;
    public CanvasGroup p2Area;

    public CanvasGroup p1Cover;
    public CanvasGroup p2Cover;

    public CardPick p1Picker;
    public CardPick p2Picker;

    public Button p1Ready;
    public Button p2Ready;

    public TMP_Text promptText;
    public Button promptButton;

    void Start()
    {
        p1Picker.OnReady += OnP1Ready;
        p2Picker.OnReady += OnP2Ready;

        StartCoroutine(Intermission(nextPlayer: 1));
    }

    void OnP1Ready(int r, int p, int s)
    {
        CentralData.I.p1Deck.Set(r, p, s);
        StartCoroutine(Intermission(nextPlayer: 2));
    }

    void OnP2Ready(int r, int p, int s)
    {
        CentralData.I.p2Deck.Set(r, p, s);
        SceneManager.LoadScene("Battle");
    }

    IEnumerator Intermission(int nextPlayer)
    {
        SetArea(p1Area, false);
        SetArea(p2Area, false);
        SetCover(p1Cover, true);
        SetCover(p2Cover, true);

        if (promptText) promptText.gameObject.SetActive(true);
        if (promptButton) promptButton.gameObject.SetActive(true);

        if (promptText)
        {
            string n1 = CentralData.I.p1Name;
            string n2 = CentralData.I.p2Name;
            promptText.text = (nextPlayer == 1)
                ? $"{n1}'s turn, {n2} close your eyes"
                : $"{n2}'s turn, {n1} close your eyes";
        }

        bool clicked = false;
        if (promptButton)
        {
            promptButton.onClick.RemoveAllListeners();
            promptButton.onClick.AddListener(() => clicked = true);
        }
        while (!clicked) yield return null;

        if (promptText) promptText.gameObject.SetActive(false);
        if (promptButton) promptButton.gameObject.SetActive(false);

        bool p1Turn = (nextPlayer == 1);
        SetArea(p1Area, p1Turn);
        SetArea(p2Area, !p1Turn);
        SetCover(p1Cover, !p1Turn);
        SetCover(p2Cover, p1Turn);
    }

    void SetTurn(int who)
    {
        bool p1Active = (who == 1);
        SetArea(p1Area, p1Active);
        SetArea(p2Area, !p1Active);
        SetCover(p1Cover, !p1Active);
        SetCover(p2Cover, p1Active);
    }

    void SetArea(CanvasGroup cg, bool on)
    {
        if (!cg) return;
        cg.alpha = 1f;
        cg.interactable = on;
        cg.blocksRaycasts = on;
    }
    void SetCover(CanvasGroup cg, bool show)
    {
        if (!cg) return;
        cg.alpha = show ? 1f : 0f;
        cg.blocksRaycasts = show;
        cg.interactable = false;
    }
}