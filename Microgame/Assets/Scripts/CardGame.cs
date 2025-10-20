using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CardGame : MonoBehaviour
{
    [Header("References")]
    public Button[] cardButtons;       
    public TMP_Text feedbackText;       
    public TMP_Text timerText;         

    [Header("Settings")]
    public float flashTime = 0.5f;      
    public float popScale = 1.2f;       
    public float timeLimit = 3f;        

    private int luckyCardIndex;
    private float timer;
    private bool gameEnded = false;

    void Start()
    {
        PickLuckyCard();
        AssignButtonListeners();
        feedbackText.text = "";
        timer = timeLimit;
    }

    void Update()
    {
        if (gameEnded) return;

        
        timer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = "Time: " + timer.ToString("F1");

        if (timer <= 0)
        {
            EndGame(false);
        }
    }

    void PickLuckyCard()
    {
        luckyCardIndex = Random.Range(0, cardButtons.Length);
        StartCoroutine(FlashLuckyCard());
    }

    IEnumerator FlashLuckyCard()
    {
        Transform cardTransform = cardButtons[luckyCardIndex].transform;
        Vector3 originalScale = cardTransform.localScale;

        
        cardTransform.localScale = originalScale * popScale;
        yield return new WaitForSeconds(flashTime);
        cardTransform.localScale = originalScale;
    }

    void AssignButtonListeners()
    {
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int index = i; 
            cardButtons[i].onClick.AddListener(() => CardClicked(index));
        }
    }

    void CardClicked(int index)
    {
        if (gameEnded) return;

        if (index == luckyCardIndex)
            EndGame(true);
        else
            EndGame(false);
    }

    void EndGame(bool success)
    {
        if (gameEnded) return;
        gameEnded = true;

        feedbackText.text = success ? "Lucky!" : "Wrong!";

        if (success)
            Invoke(nameof(GoVictory), 1.5f); 
        else
            Invoke(nameof(GoFail), 1.5f);
    }

    void GoVictory()
    {
        SceneManager.LoadScene("VictoryScreen");
    }

    void GoFail()
    {
        GameManager.Instance.GameOver();
    }
}
