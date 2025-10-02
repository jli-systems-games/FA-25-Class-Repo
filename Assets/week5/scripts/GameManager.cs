using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public float totalPlayTime = 180f;
    public string winScene = "WinScene";
    public string loseScene = "GameOverScene";   // 시간 초과 시 이동
    public string timeoutScene = "TimeOutScene"; // 카메라/콜라이더에 걸려 조기 실패 시 이동

    public HeartGaugeController heartGauge;
    public UIController ui;

    float timeLeft;
    bool playing;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

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

        if (heartGauge != null && heartGauge.IsFull())
        {
            Win();
        }
        else if (timeLeft <= 0f)
        {
            // 시간 초과로 실패
            GameOver();
        }
    }

    /// <summary>
    /// 실패 처리. 호출 시점의 남은 시간을 보고 원인을 구분합니다.
    /// - timeLeft <= 0 : 시간 초과 → loseScene 로드
    /// - timeLeft  > 0 : 조기 실패(카메라/콜라이더 등) → timeoutScene 로드
    /// </summary>
    public void GameOver()
    {
        if (!playing) return;
        playing = false;

        string sceneToLoad = (timeLeft <= 0f) ? loseScene : timeoutScene;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void Win()
    {
        if (!playing) return;
        playing = false;
        SceneManager.LoadScene(winScene);
    }
}
