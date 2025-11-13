using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Splines;
using TMPro;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// referencing: https://www.youtube.com/watch?v=hmIS2iBe-iQ
/// </summary>

public class HandManager : MonoBehaviour
{
    public TMP_Text potText;
    public TMP_Text player1ChipText;
    public TMP_Text player2ChipText;
    public TMP_Text roundText;
    public Button revealButton;
    public Button player1BetButton;
    public Button player2BetButton;
    public Button player1WinButton;
    public Button player2WinButton;
    public Button continueButton;
    public Button resetButton;
    public GameObject WinnerRing;
    public GameObject TruckAvatar;
    public GameObject MagicianAvatar;

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
        continueButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);

        continueButton.onClick.AddListener(OnContinueGame);
        resetButton.onClick.AddListener(OnResetGame);

        player1WinButton.gameObject.SetActive(false);
        player2WinButton.gameObject.SetActive(false);

        player1WinButton.onClick.AddListener(() => ManualWin(1));
        player2WinButton.onClick.AddListener(() => ManualWin(2));

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
        if (!canPlayersAct || p1Action != PlayerAction.None) return;
        DealCardToPlayer(1);
        p1Action = PlayerAction.Draw;
        CheckRoundProgress();
    }

    public void Player2Draw()
    {
        if (!canPlayersAct || p2Action != PlayerAction.None) return;
        DealCardToPlayer(2);
        p2Action = PlayerAction.Draw;
        CheckRoundProgress();
    }

    public void Player1Raise()
    {
        if (!canPlayersAct || p1Action != PlayerAction.None || player1Chips <= 0) return;

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
        if (!canPlayersAct || p2Action != PlayerAction.None || player2Chips <= 0) return;

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
            player1WinButton.gameObject.SetActive(true);
            player2WinButton.gameObject.SetActive(true);
            return;
        }

        canPlayersAct = true;
        player1BetButton.gameObject.SetActive(true);
        player2BetButton.gameObject.SetActive(true);

        p1Action = PlayerAction.None;
        p2Action = PlayerAction.None;
    }

    void ManualWin(int winner)
    {
        player1WinButton.gameObject.SetActive(false);
        player2WinButton.gameObject.SetActive(false);
        WinnerRing.SetActive(true);

        if (winner == 1)
        {
            player1Chips += pot;
            TruckAvatar.SetActive(true);
        }
        else
        {
            player2Chips += pot;
            MagicianAvatar.SetActive(true);
        }

        if ((player1Chips <= 0 && winner != 1) ||
            (player2Chips <= 0 && winner != 2))
        {
            ShowResetOnly();
            return;
        }

        ShowContinueAndResetButtons();
    }
    void DeactivateWinVisuals()
    {
        WinnerRing.SetActive(false);
        TruckAvatar.SetActive(false);
        MagicianAvatar.SetActive(false);
    }

    void ShowResetOnly()
    {
        continueButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(true);
    }

    void ShowContinueAndResetButtons()
    {
        continueButton.gameObject.SetActive(true);
        resetButton.gameObject.SetActive(true);
    }

    void OnContinueGame()
    {
        continueButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
        DeactivateWinVisuals();

        StartNewRound();
    }

    void OnResetGame()
    {
        continueButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
        DeactivateWinVisuals();

        player1Chips = 5;
        player2Chips = 5;

        StartNewRound();
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
    }

    void UpdateUI()
    {
        potText.text = pot.ToString();
        player1ChipText.text = player1Chips.ToString();
        player2ChipText.text = player2Chips.ToString();
        roundText.text = round.ToString();
    }
}
