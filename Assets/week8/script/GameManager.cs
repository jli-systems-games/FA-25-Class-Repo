using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GrandpaController grandpa;
    [SerializeField] private Slider progressBar;  // 0~1
    [SerializeField] private Text timerText;      // 옵션
    [SerializeField] private Text debugText;      // 옵션

    [Header("Win/Lose")]
    [SerializeField] private float targetGoodCareSeconds = 150f; // 2~3분
    [SerializeField] private string successSceneName = "Success";
    [SerializeField] private string gameOverSceneName = "GameOver";

    [Header("Difficulty Curve")]
    [SerializeField] private bool enableDifficultyCurve = true;
    [SerializeField] private float rampEverySec = 30f;          // 30초마다 램프
    [SerializeField] private float minNeglectLimitScale = 0.5f; // 방치 한계 최소 50%
    [SerializeField] private float minSpawnIntervalScale = 0.4f;// 스폰 간격 최소 40%
    [SerializeField] private int maxMaxConcurrentNeeds = 3;   // 최종 동시 3개

    private float goodCareTimer = 0f;
    private float elapsed = 0f;
    private float nextRampTime = 0f;

    private void OnEnable()
    {
        if (grandpa != null)
        {
            grandpa.OnNeglectExceeded += HandleNeglectExceeded;
            grandpa.OnSleeping += HandleSleeping;
        }
    }

    private void OnDisable()
    {
        if (grandpa != null)
        {
            grandpa.OnNeglectExceeded -= HandleNeglectExceeded;
            grandpa.OnSleeping -= HandleSleeping;
        }
    }

    private void Start()
    {
        // 초기 난이도
        if (grandpa != null)
        {
            grandpa.neglectLimitScale = 1f;
            grandpa.spawnIntervalScale = 1f;
            grandpa.maxConcurrentNeeds = 1; // 초반엔 단일 요구
        }
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        goodCareTimer += Time.deltaTime;

        float p = Mathf.Clamp01(goodCareTimer / targetGoodCareSeconds);
        if (progressBar) progressBar.value = p;

        if (timerText) timerText.text = $"{Mathf.FloorToInt(elapsed)}s";
        if (debugText && grandpa != null)
        {
            debugText.text =
                $"Fatigue:{grandpa.GetFatigue():0} | NegScale:{grandpa.neglectLimitScale:0.00} | SpawnScale:{grandpa.spawnIntervalScale:0.00} | MaxNeeds:{grandpa.maxConcurrentNeeds}";
        }

        if (p >= 1f)
        {
            SceneManager.LoadScene(successSceneName);
            return;
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

    private void HandleSleeping(bool sleeping)
    {
        // 필요 시 수면 중 진행도 증가율 조정 가능
        // (현재는 단순화를 위해 변화 없음)
    }

    // 잘못된 입력 등 패널티를 줄 때 호출(옵션)
    public void Penalty(float seconds = 5f)
    {
        goodCareTimer = Mathf.Max(0f, goodCareTimer - seconds);
    }
}
