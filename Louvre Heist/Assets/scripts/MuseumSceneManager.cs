using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MuseumSceneManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float totalTime = 480f; // 8 minutes (480 secs)
    public TMP_Text timerText;

    [Header("Game State")]
    public bool playerHasJewels = false;
    public bool gameActive = true;

    void Update()
    {
        if (!gameActive) return;

        // TIMER COUNTDOWN
        totalTime -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";

        // TIME UP → LOSE
        if (totalTime <= 0)
        {
            gameActive = false;
            SceneManager.LoadScene("LoseScreen");
        }
    }

    public void PickupJewels()
    {
        playerHasJewels = true;
    }

    public void EscapeHeist()
    {
        if (playerHasJewels)
        {
            SceneManager.LoadScene("WinScreen");
        }
    }

    public void TriggerAlarmFail()
    {
        gameActive = false;
        SceneManager.LoadScene("LoseScreen");
    }
}
