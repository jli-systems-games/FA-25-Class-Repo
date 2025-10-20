using System;

public enum MatchOutcome { Draw, P1Win, P2Win }

[Serializable]
public class MatchResult
{
    public int p1Score;
    public int p2Score;
    public int roundsPlayed;
    public MatchOutcome outcome;
    public DateTime time;
}

[Serializable]
public class PlayerDeckData
{
    public int rock;
    public int paper;
    public int scissor;

    public void Set(int r, int p, int s)
    {
        rock = r; paper = p; scissor = s;
    }
}

[Serializable]
public class RoundRevealData
{
    public HandType p1Play;
    public HandType p2Play;
    public int result;
    public bool continued;
}

public sealed class CentralData
{
    private static CentralData _i;
    public static CentralData I => _i ?? (_i = new CentralData());

    private CentralData() { ResetAll(); }

    public string p1Name;
    public string p2Name;

    public PlayerDeckData p1Deck;
    public PlayerDeckData p2Deck;

    public MatchResult lastResult;
    public RoundRevealData reveal;

    public void SetNames(string n1, string n2)
    {
        p1Name = string.IsNullOrWhiteSpace(n1) ? "Player 1" : n1.Trim();
        p2Name = string.IsNullOrWhiteSpace(n2) ? "Player 2" : n2.Trim();
    }

    public void ResetAll(bool keepNames = false)
    {
        if (!keepNames)
        {
            p1Name = "";
            p2Name = "";
        }
        p1Deck = new PlayerDeckData();
        p2Deck = new PlayerDeckData();
        lastResult = null;
        reveal = new RoundRevealData();
    }
}