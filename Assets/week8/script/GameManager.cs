// GameManager.cs  (씬 전환 전용, UI/속성 어트리뷰트 없음)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GrandpaController grandpa;
    public Slider lifeBar; // 0~1 표시만

    public string successSceneName = "Success";
    public string gameOverSceneName = "GameOver";

    public bool enableDifficultyCurve = true;
    public float rampEverySec = 30f;
    public float minNeglectLimitScale = 0.5f;
    public float minSpawnIntervalScale = 0.4f;
    public int maxMaxConcurrentNeeds = 3;

    float elapsed = 0f;
    float nextRamp = 0f;
    bool ended = false;

    void Update()
    {
        if (grandpa == null || ended) return;

        elapsed += Time.deltaTime;

        // Life UI
        if (lifeBar) lifeBar.value = Mathf.Clamp01(grandpa.GetLife() / 100f);

        // 방치 실패 우선
        if (grandpa.neglectFailed)
        {
            ended = true;
            SceneManager.LoadScene(gameOverSceneName);
            return;
        }

        // Life 승패
        float life = grandpa.GetLife();
        if (life <= 0f)
        {
            ended = true;
            SceneManager.LoadScene(gameOverSceneName);
            return;
        }
        if (life >= 100f)
        {
            ended = true;
            SceneManager.LoadScene(successSceneName);
            return;
        }

        // 난이도 곡선(선택)
        if (enableDifficultyCurve && elapsed >= nextRamp)
        {
            nextRamp += rampEverySec;
            grandpa.neglectLimitScale = Mathf.Max(minNeglectLimitScale, grandpa.neglectLimitScale - 0.1f);
            grandpa.spawnIntervalScale = Mathf.Max(minSpawnIntervalScale, grandpa.spawnIntervalScale - 0.1f);
            if (grandpa.maxConcurrentNeeds < maxMaxConcurrentNeeds) grandpa.maxConcurrentNeeds += 1;
        }
    }
}
