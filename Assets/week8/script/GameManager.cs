using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GrandpaController grandpa;
    [SerializeField] private Slider progressBar;  // 0~1 (Life/100 표시)
    [SerializeField] private Text timerText;      // 옵션
    [SerializeField] private Text debugText;      // 옵션

    [Header("Scenes")]
    [SerializeField] private string successSceneName = "Success";
    [SerializeField] private string gameOverSceneName = "GameOver";

    [Header("Difficulty Curve")]
    [SerializeField] private bool enableDifficultyCurve = true;
    [SerializeField] private float rampEverySec = 30f;          // 30초마다 램프
    [SerializeField] private float minNeglectLimitScale = 0.5f; // 방치 한계 최소 50%
    [SerializeField] private float minSpawnIntervalScale = 0.4f;// 스폰 간격 최소 40%
    [SerializeField] private int maxMaxConcurrentNeeds = 3;     // 최종 동시 3개

    private float elapsed = 0f;
    private float nextRampTime = 0f;

    private void OnEnable()
    {
        if (grandpa != null)
        {
            grandpa.OnNeglectExceeded += HandleNeglectExceeded; // (옵션) 방치 실패 유지
            grandpa.OnLifeZero += HandleLifeZero;
            grandpa.OnLifeFull += HandleLifeFull;
        }
    }

    private void OnDisable()
    {
        if (grandpa != null)
        {
            grandpa.OnNeglectExceeded -= HandleNeglectExceeded;
            grandpa.OnLifeZero -= HandleLifeZero;
            grandpa.OnLifeFull -= HandleLifeFull;
        }
    }

    private void Start()
    {
        if (grandpa != null)
        {
            // 초기 난이도
            grandpa.neglectLimitScale = 1f;
            grandpa.spawnIntervalScale = 1f;
            grandpa.maxConcurrentNeeds = 1; // 초반엔 단일 요구
        }
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        if (progressBar && grandpa != null)
            progressBar.value = Mathf.Clamp01(grandpa.GetLife() / 100f);

        if (timerText) timerText.text = $"{Mathf.FloorToInt(elapsed)}s";
        if (debugText && grandpa != null)
        {
            debugText.text =
                $"Life:{grandpa.GetLife():0} | Fatigue:{grandpa.GetFatigue():0} | NegScale:{grandpa.neglectLimitScale:0.00} | SpawnScale:{grandpa.spawnIntervalScale:0.00} | MaxNeeds:{grandpa.maxConcurrentNeeds}";
        }

        if (enableDifficultyCurve && elapsed >= nextRampTime && grandpa != null)
        {
            nextRampTime += rampEverySec;

            // 방치 한계 축소
            grandpa.neglectLimitScale = Mathf.Max(minNeglectLimitScale, grandpa.neglectLimitScale - 0.1f);

            // 스폰 간격 단축
            grandpa.spawnIntervalScale = Mathf.Max(minSpawnIntervalScale, grandpa.spawnIntervalScale - 0.1f);

            // 동시 요구 증가
            if (grandpa.maxConcurrentNeeds < maxMaxConcurrentNeeds)
                grandpa.maxConcurrentNeeds += 1;
        }
    }

    private void HandleNeglectExceeded()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    private void HandleLifeZero()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    private void HandleLifeFull()
    {
        SceneManager.LoadScene(successSceneName);
    }
}
