using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BetUI : MonoBehaviour
{
    public MazeFloodGame_Overlay game;
    public Button betRedButton;
    public Button betBlueButton;
    public Button nextButton;
    public TMP_Text correctText;
    public TMP_Text wrongText;

    enum Choice { None, RedA, BlueB }
    Choice _choice = Choice.None;
    bool _locked = false;
    bool _needNext = false;

    void Awake()
    {
        if (BetCounter.I == null)
        {
            var go = new GameObject("BetCounter");
            go.AddComponent<BetCounter>();
        }

        betRedButton.onClick.AddListener(() => PlaceBet(Choice.RedA));
        betBlueButton.onClick.AddListener(() => PlaceBet(Choice.BlueB));
        nextButton.onClick.AddListener(NextRound);

        nextButton.interactable = false;
        RefreshInfo("");
    }

    void OnEnable()
    {
        if (!game) game = FindFirstObjectByType<MazeFloodGame_Overlay>();
        if (game) game.OnMatchFinished += OnMatchFinished;
    }

    void OnDisable()
    {
        if (game) game.OnMatchFinished -= OnMatchFinished;
    }

    void PlaceBet(Choice c)
    {
        if (_locked) return;
        if (_needNext) return;
        if (BetCounter.I != null && (BetCounter.I.IsGameOver() || BetCounter.I.IsWin())) return;

        _choice = c;
        _locked = true;

        if (game) game.RequestMatch();

        betRedButton.interactable = false;
        betBlueButton.interactable = false;

        RefreshInfo("");
    }

    void OnMatchFinished(MatchResult r)
    {
        bool correct =
            (_choice == Choice.RedA  && r.winner == Winner.A) ||
            (_choice == Choice.BlueB && r.winner == Winner.B);

        if (r.winner == Winner.Draw)
        {
            RefreshInfo("Draw");
        }
        else if (correct)
        {
            BetCounter.I.AddCorrect();
            RefreshInfo("Correct!");
            
            if (BetCounter.I.IsWin())
            {
                return;
            }
        }
        else
        {
            BetCounter.I.AddWrong();
            RefreshInfo("Wrong");
            
            if (BetCounter.I.IsGameOver())
            {
                return;
            }
        }

        _choice = Choice.None;
        _locked = false;
        _needNext = true;
        betRedButton.interactable = false;
        betBlueButton.interactable = false;
        nextButton.interactable = true;
    }

    void NextRound()
    {
        if (BetCounter.I != null && (BetCounter.I.IsGameOver() || BetCounter.I.IsWin()))
        {
            return;
        }

        _needNext = false;
        betRedButton.interactable = true;
        betBlueButton.interactable = true;
        nextButton.interactable = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void RefreshInfo(string line)
    {
        if (correctText)
            correctText.text = $"Correct: {BetCounter.I.correctGuesses}";
        if (wrongText)
            wrongText.text = $"Wrong: {BetCounter.I.wrongGuesses}";
    }
}