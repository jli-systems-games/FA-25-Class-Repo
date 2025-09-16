using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    [Header("UI")]
    public Text urinePercentageText; // 尿量百分比
    public Text missPercentageText; // miss 百分比
    public Text statusText; // 失败/胜利文本
    public Text timerText; // 10 秒倒计时
    [Header("Settings")]
    public float urineCapacity = 100f;
    [Range(0.05f, 1f)]
    public float missRatioMax = 0.2f;
    public float winThreshold = 0.1f;
    [Header("Fade Out")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 2f;
    private float urine;
    private float misses;
    public bool failed { get; private set; }
    public bool won { get; private set; }
    private float timer = 10f; // 10 秒倒计时

    public float Urine => urine;
    public float UrineCapacity => urineCapacity;

    void Awake()
    {
        I = this;
        urine = urineCapacity;
        UpdateUI();
        if (fadeCanvas) fadeCanvas.alpha = 0f;
    }

    void Update()
    {
        if (!failed && !won)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f) Fail();
            UpdateUI();
            if (urine <= urineCapacity * winThreshold && misses < urineCapacity * missRatioMax)
                Win();
        }
    }

    public bool CanShoot() => !failed && !won && urine > 0;

    public void ConsumeUrine(float amount)
    {
        if (failed || won) return;
        urine = Mathf.Max(0f, urine - amount);
        UpdateUI();
    }

    public void RegisterShot() { }
    public void RegisterHit() { }
    public void RegisterMiss()
    {
        if (failed || won) return;
        misses += 1f;
        UpdateUI();
        if (misses >= urineCapacity * missRatioMax)
            Fail();
    }

    void UpdateUI()
    {
        if (urinePercentageText) urinePercentageText.text = $"Urine: {(urine / urineCapacity * 100):F0}%";
        if (missPercentageText) missPercentageText.text = $"Miss: {(misses / urineCapacity * 100):F0}%";
        if (timerText) timerText.text = $"Time: {(int)timer} s";
    }

    void Fail()
    {
        failed = true;
        if (statusText) statusText.text = "Lose: Time Up or Too Many Misses";
        StartCoroutine(FadeAndRestart());
    }

    void Win()
    {
        won = true;
        if (statusText) statusText.text = "Win! Next Level!";
        StartCoroutine(FadeAndNextScene());
    }

    System.Collections.IEnumerator FadeAndRestart()
    {
        if (fadeCanvas)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    System.Collections.IEnumerator FadeAndNextScene()
    {
        if (fadeCanvas)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(nextSceneIndex);
            else
                SceneManager.LoadScene(0);
        }
    }
}