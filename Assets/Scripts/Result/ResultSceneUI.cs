using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultSceneUI : MonoBehaviour
{
    public TMP_Text winnerText;
    public TMP_Text scoreText;
    public Button mainMenuBtn;

    void Start()
    {
        if (CentralData.I == null || CentralData.I.lastResult == null)
        {
            if (winnerText) winnerText.text = "No Result";
            if (scoreText) scoreText.text = "";
            if (mainMenuBtn) mainMenuBtn.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
            return;
        }

        var r = CentralData.I.lastResult;

        string n1 = CentralData.I.p1Name ?? "Player 1";
        string n2 = CentralData.I.p2Name ?? "Player 2";

        if (winnerText)
        {
            winnerText.text = r.outcome == MatchOutcome.Draw ? "Draw!"
                             : (r.outcome == MatchOutcome.P1Win ? $"{n1} Wins!" : $"{n2} Wins!");
        }

        if (scoreText) scoreText.text = $"Final Score\n{n1} {r.p1Score} : {r.p2Score} {n2}";

        if (mainMenuBtn)
        {
            mainMenuBtn.onClick.AddListener(() =>
            {
                CentralData.I.ResetAll(keepNames: false);
                SceneManager.LoadScene("MainMenu");
            });
        }
    }
}