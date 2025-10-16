using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RoundBattleManager : MonoBehaviour
{
    public PlayCard p1Picker;
    public PlayCard p2Picker;

    public CanvasGroup p1Area, p2Area;
    public CanvasGroup p1Cover, p2Cover;

    public TMP_Text roundText;
    public TMP_Text scoreText;
    public TMP_Text centerRemainText;

    public TMP_Text promptText;
    public UnityEngine.UI.Button promptButton;

    int roundIndex = 0;
    int p1Score = 0, p2Score = 0;

    int r1, p1c, s1;
    int r2, p2c, s2;

    bool p1Locked, p2Locked;

    const int TotalRounds = 9;
    const int TargetWins = 5;

    void Start()
    {
        var d1 = CentralData.I.p1Deck;
        var d2 = CentralData.I.p2Deck;

        var hand1 = BuildHand(d1.rock, d1.paper, d1.scissor);
        var hand2 = BuildHand(d2.rock, d2.paper, d2.scissor);

        p1Picker.SetHand(hand1);
        p2Picker.SetHand(hand2);

        r1 = d1.rock; p1c = d1.paper; s1 = d1.scissor;
        r2 = d2.rock; p2c = d2.paper; s2 = d2.scissor;

        p1Picker.OnReady += OnP1Ready;
        p2Picker.OnReady += OnP2Ready;

        BeginRound();
    }

    HandType[] BuildHand(int r, int p, int s)
    {
        var list = new List<HandType>(9);
        for (int i = 0; i < r; i++) list.Add(HandType.Rock);
        for (int i = 0; i < p; i++) list.Add(HandType.Paper);
        for (int i = 0; i < s; i++) list.Add(HandType.Scissor);
        return list.ToArray();
    }

    void BeginRound()
    {
        if (IsMatchOver()) { EndMatch(); return; }

        roundIndex++;
        p1Locked = p2Locked = false;
        UpdateHUD();

        StartCoroutine(Intermission(nextPlayer: 1));
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

    void UpdateHUD()
    {
        if (roundText) roundText.text = $"Round {roundIndex} / {TotalRounds}";
        if (scoreText)
        {
            string n1 = CentralData.I.p1Name;
            string n2 = CentralData.I.p2Name;
            scoreText.text = $"P1: {p1Score}  VS  {p2Score} :P2";
        }

        if (centerRemainText)
        {
            int R = r1 + r2;
            int P = p1c + p2c;
            int S = s1 + s2;
            centerRemainText.text = $"Rock: {R}    Paper: {P}    Scissor: {S}";
        }
    }

    void SetTurn(int who)
    {
        if (who == 1)
        {
            SetArea(p1Area, true); SetCover(p2Cover, true);
            SetArea(p2Area, false); SetCover(p1Cover, false);
        }
        else if (who == 2)
        {
            SetArea(p1Area, false); SetCover(p2Cover, false);
            SetArea(p2Area, true); SetCover(p1Cover, true);
        }
        else
        {
            SetArea(p1Area, false); SetCover(p1Cover, false);
            SetArea(p2Area, false); SetCover(p2Cover, false);
        }
    }

    void SetArea(CanvasGroup cg, bool on) { if (!cg) return; cg.alpha = 1f; cg.interactable = on; cg.blocksRaycasts = on; }
    void SetCover(CanvasGroup cg, bool show) { if (!cg) return; cg.alpha = show ? 1f : 0f; cg.blocksRaycasts = show; cg.interactable = false; }

    void OnP1Ready()
    {
        if (!p1Picker.HasSelection) return;
        p1Locked = true;
        StartCoroutine(Intermission(nextPlayer: 2));
    }

    void OnP2Ready()
    {
        if (!p2Picker.HasSelection) return;
        p2Locked = true;
        if (p1Locked && p2Locked) StartCoroutine(RevealAndScoreSequence());
    }

    IEnumerator RevealAndScoreSequence()
    {
        SetTurn(3);

        var a = p1Picker.GetSelectedType();
        var b = p2Picker.GetSelectedType();
        int r = Resolve(a, b); // 1/-1/0

        CentralData.I.reveal.p1Play = a;
        CentralData.I.reveal.p2Play = b;
        CentralData.I.reveal.result = r;
        CentralData.I.reveal.continued = false;

        var loadOp = SceneManager.LoadSceneAsync("Reveal", LoadSceneMode.Additive);
        while (!loadOp.isDone) yield return null;

        while (!CentralData.I.reveal.continued) yield return null;

        var unloadOp = SceneManager.UnloadSceneAsync("Reveal");
        while (unloadOp != null && !unloadOp.isDone) yield return null;

        if (r > 0) p1Score++;
        else if (r < 0) p2Score++;

        p1Picker.ConsumeSelected();
        p2Picker.ConsumeSelected();
        DecRemain(ref r1, ref p1c, ref s1, a);
        DecRemain(ref r2, ref p2c, ref s2, b);

        UpdateHUD();

        if (IsMatchOver()) EndMatch();
        else BeginRound();
    }

    void DecRemain(ref int rr, ref int pp, ref int ss, HandType h)
    {
        if (h == HandType.Rock) rr--;
        else if (h == HandType.Paper) pp--;
        else ss--;
    }

    int Resolve(HandType a, HandType b)
    {
        if (a == b) return 0;
        if ((a == HandType.Rock && b == HandType.Scissor) ||
            (a == HandType.Scissor && b == HandType.Paper) ||
            (a == HandType.Paper && b == HandType.Rock)) return 1;
        return -1;
    }

    bool IsMatchOver()
    {
        return p1Score >= TargetWins || p2Score >= TargetWins || roundIndex >= TotalRounds;
    }

    void EndMatch()
    {
        MatchOutcome outcome =
            (p1Score == p2Score) ? MatchOutcome.Draw :
            (p1Score > p2Score) ? MatchOutcome.P1Win : MatchOutcome.P2Win;

        CentralData.I.lastResult = new MatchResult
        {
            p1Score = p1Score,
            p2Score = p2Score,
            roundsPlayed = roundIndex,
            outcome = outcome,
            time = System.DateTime.Now
        };

        SceneManager.LoadScene("Result");
    }
}