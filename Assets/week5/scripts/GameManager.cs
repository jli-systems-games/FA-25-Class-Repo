using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public float totalPlayTime = 180f;
    public string winScene = "WinScene";
    public string loseScene = "GameOverScene";

    public HeartGaugeController heartGauge;
    public UIController ui;

    float timeLeft;
    bool playing;

    void Awake() { if (I == null) I = this; else Destroy(gameObject); }

    void Start()
    {
        timeLeft = totalPlayTime;
        playing = true;
        ui?.SetTimer(timeLeft);
    }

    void Update()
    {
        if (!playing) return;

        timeLeft -= Time.deltaTime;
        ui?.SetTimer(timeLeft);

        if (heartGauge != null && heartGauge.IsFull()) Win();
        else if (timeLeft <= 0f)
        {
            if (heartGauge != null && heartGauge.IsFull()) Win();
            else GameOver();
        }
    }

    public void GameOver()
    {
        if (!playing) return;
        playing = false;
        SceneManager.LoadScene(loseScene);
    }

    public void Win()
    {
        if (!playing) return;
        playing = false;
        SceneManager.LoadScene(winScene);
    }
}
