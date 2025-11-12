using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Splines;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class HandManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text potText;
    public TMP_Text player1ChipText;
    public TMP_Text player2ChipText;
    public TMP_Text roundText;
    public Button revealButton;
    public Button player1BetButton;
    public Button player2BetButton;

    [Header("Card Setup")]
    public List<GameObject> allCardPrefabs;
    public Transform[] communitySlots;
    public GameObject deckArea;
    public CardSpline player1Spline;
    public CardSpline player2Spline;

    private List<GameObject> deck = new();
    private List<GameObject> player1Hand = new();
    private List<GameObject> player2Hand = new();
    private List<GameObject> community = new();

    private int player1Chips = 5;
    private int player2Chips = 5;
    private int pot = 0;
    private int round = 1;
    private int revealedCards = 0;

    private bool canPlayersAct = false;

    private enum PlayerAction { None, Draw, Raise }
    private PlayerAction p1Action = PlayerAction.None;
    private PlayerAction p2Action = PlayerAction.None;

    void Start()
    {
        revealButton.onClick.AddListener(OnRevealButtonClicked);
        player1BetButton.onClick.AddListener(Player1Raise);
        player2BetButton.onClick.AddListener(Player2Raise);

        player1BetButton.gameObject.SetActive(false);
        player2BetButton.gameObject.SetActive(false);
        revealButton.gameObject.SetActive(true);

        StartNewRound();
    }

    void Update()
    {
        if (!canPlayersAct) return;

        if (Input.GetKeyDown(KeyCode.S))
            Player1Draw();

        if (Input.GetKeyDown(KeyCode.K))
            Player2Draw();
    }

    void StartNewRound()
    {
        ClearAllCards();
        BuildDeck();
        Shuffle(deck);

        pot = 0;
        revealedCards = 0;
        round = 1;
        player1Hand.Clear();
        player2Hand.Clear();
        community.Clear();
        p1Action = PlayerAction.None;
        p2Action = PlayerAction.None;

        DealCardToPlayer(1);
        DealCardToPlayer(2);

        canPlayersAct = false;
        revealButton.gameObject.SetActive(true);
        revealButton.GetComponentInChildren<TMP_Text>().text = "Reveal First Card";

        UpdateUI();
    }

    void BuildDeck()
    {
        deck.Clear();
        foreach (GameObject prefab in allCardPrefabs)
        {
            GameObject clone = Instantiate(prefab, deckArea.transform.position, Quaternion.identity);
            clone.transform.SetParent(deckArea.transform, false);
            clone.SetActive(false);
            deck.Add(clone);
        }
    }

    void Shuffle(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(0, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    void ClearAllCards()
    {
        foreach (Transform c in deckArea.transform) Destroy(c.gameObject);
        foreach (Transform t in communitySlots)
            foreach (Transform c in t) Destroy(c.gameObject);
        player1Spline.ClearCards();
        player2Spline.ClearCards();
    }

    GameObject DrawCard()
    {
        if (deck.Count == 0) return null;
        GameObject card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    public void Player1Draw()
    {
        if (!canPlayersAct) return;
        DealCardToPlayer(1);
        p1Action = PlayerAction.Draw;
        CheckRoundProgress();
    }

    public void Player2Draw()
    {
        if (!canPlayersAct) return;
        DealCardToPlayer(2);
        p2Action = PlayerAction.Draw;
        CheckRoundProgress();
    }

    public void Player1Raise()
    {
        if (!canPlayersAct || player1Chips <= 0) return;

        int bet = (round < 6) ? 2 : 1;
        int actualBet = Mathf.Min(bet, player1Chips);
        player1Chips -= actualBet;
        pot += actualBet;

        UpdateUI();
        p1Action = PlayerAction.Raise;
        CheckRoundProgress();
    }

    public void Player2Raise()
    {
        if (!canPlayersAct || player2Chips <= 0) return;

        int bet = (round < 6) ? 2 : 1;
        int actualBet = Mathf.Min(bet, player2Chips);
        player2Chips -= actualBet;
        pot += actualBet;

        UpdateUI();
        p2Action = PlayerAction.Raise;
        CheckRoundProgress();
    }

    void CheckRoundProgress()
    {
        if (p1Action != PlayerAction.None && p2Action != PlayerAction.None)
        {
            canPlayersAct = false;
            player1BetButton.gameObject.SetActive(false);
            player2BetButton.gameObject.SetActive(false);
            StartCoroutine(NextPhase());
        }
    }

    IEnumerator NextPhase()
    {
        yield return new WaitForSeconds(0.8f);

        round++;
        revealButton.gameObject.SetActive(true);

        if (revealedCards < 3)
            revealButton.GetComponentInChildren<TMP_Text>().text = "Reveal Next Card";
        else if (round < 6)
            revealButton.GetComponentInChildren<TMP_Text>().text = "Continue";
        else
            revealButton.GetComponentInChildren<TMP_Text>().text = "Showdown";

        UpdateUI();
    }

    void DealCardToPlayer(int player)
    {
        GameObject card = DrawCard();
        if (card == null) return;
        card.SetActive(true);

        var info = card.GetComponent<CardInfo>();
        Debug.Log($"P{player} drew {info.rank} of {info.suit}");

        if (player == 1)
        {
            player1Hand.Add(card);
            player1Spline.AddCard(card);
        }
        else
        {
            player2Hand.Add(card);
            player2Spline.AddCard(card);
        }
    }

    void OnRevealButtonClicked()
    {
        revealButton.gameObject.SetActive(false);

        if (revealedCards < 3)
        {
            RevealCommunityCard();
        }
        else if (round >= 6)
        {
            EvaluateWinner();
            return;
        }

        canPlayersAct = true;
        player1BetButton.gameObject.SetActive(true);
        player2BetButton.gameObject.SetActive(true);

        p1Action = PlayerAction.None;
        p2Action = PlayerAction.None;
    }

    void RevealCommunityCard()
    {
        GameObject card = DrawCard();
        if (card == null) return;
        card.SetActive(true);

        Transform slot = communitySlots[revealedCards];
        card.transform.position = deckArea.transform.position;
        card.transform.DOMove(slot.position, 0.5f).OnComplete(() =>
        {
            card.transform.SetParent(slot);
        });

        community.Add(card);
        revealedCards++;

        var info = card.GetComponent<CardInfo>();
        Debug.Log($"Community reveals {info.rank} of {info.suit}");
    }

    // --- Poker Evaluation ---
    void EvaluateWinner()
    {
        List<CardInfo> p1Cards = player1Hand.Select(c => c.GetComponent<CardInfo>()).ToList();
        List<CardInfo> p2Cards = player2Hand.Select(c => c.GetComponent<CardInfo>()).ToList();
        List<CardInfo> communityCards = community.Select(c => c.GetComponent<CardInfo>()).ToList();

        var p1Best = EvaluateBestHand(p1Cards, communityCards);
        var p2Best = EvaluateBestHand(p2Cards, communityCards);

        if (p1Best > p2Best)
        {
            player1Chips += pot;
            Debug.Log("Player 1 wins pot!");
        }
        else if (p2Best > p1Best)
        {
            player2Chips += pot;
            Debug.Log("Player 2 wins pot!");
        }
        else
        {
            int split = pot / 2;
            player1Chips += split;
            player2Chips += pot - split;
            Debug.Log("Tie. Pot split.");
        }

        StartNewRound();
    }

    int EvaluateBestHand(List<CardInfo> playerCards, List<CardInfo> community)
    {
        List<CardInfo> all = new(playerCards);
        all.AddRange(community);

        // Simplified rank conversion for J, Q, K, A
        int RankValue(string r) => r switch
        {
            "J" => 11,
            "Q" => 12,
            "K" => 13,
            "A" => 14,
            _ => 0
        };

        var ranks = all.Select(c => RankValue(c.rank)).OrderByDescending(v => v).ToList();
        var suits = all.Select(c => c.suit).ToList();

        bool flush = suits.GroupBy(s => s).Any(g => g.Count() >= 5);
        bool straight = ranks.Distinct().Count() >= 4 &&
                        (ranks.Contains(11) && ranks.Contains(12) && ranks.Contains(13) && ranks.Contains(14));

        var groups = ranks.GroupBy(r => r).OrderByDescending(g => g.Count()).ThenByDescending(g => g.Key).ToList();
        int maxCount = groups.First().Count();

        if (flush && straight) return 900 + groups.First().Key;   // Straight Flush
        if (maxCount == 4) return 800 + groups.First().Key;       // Four of a Kind
        if (maxCount == 3 && groups.Any(g => g.Count() == 2)) return 700 + groups.First().Key; // Full House
        if (flush) return 600 + ranks.Max();                      // Flush
        if (straight) return 500 + ranks.Max();                   // Straight
        if (maxCount == 3) return 400 + groups.First().Key;       // Three of a Kind
        if (groups.Count(g => g.Count() == 2) >= 2) return 300 + groups.First().Key; // Two Pair
        if (maxCount == 2) return 200 + groups.First().Key;       // One Pair
        return 100 + ranks.Max();                                 // High Card
    }

    void UpdateUI()
    {
        potText.text = pot.ToString();
        player1ChipText.text = player1Chips.ToString();
        player2ChipText.text = player2Chips.ToString();
        roundText.text = round.ToString();
    }
}
