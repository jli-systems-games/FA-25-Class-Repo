using UnityEngine;
using TMPro;  // 用于 TextMeshPro UI

public class GameManager : MonoBehaviour
{
    public static GameManager I;   // 单例，方便其它脚本调用

    [Header("Round Settings")]
    public float roundTime = 5f;   // 一局时间
    public float winRatio = 0.6f;  // 命中率要求

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI resultText;

    int shots, hits;
    bool ended;

    void Awake()
    {
        I = this;  // 让 PeeProbe 可以用 GameManager.I 来访问
    }

    public void RegisterShot()
    {
        shots++;
        UpdateScoreUI();
    }

    public void RegisterHit()
    {
        hits++;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"{hits}/{shots}";
    }

    void Update()
    {
        if (ended) return;

        roundTime -= Time.deltaTime;
        if (timerText) timerText.text = Mathf.CeilToInt(Mathf.Max(0, roundTime)).ToString();

        if (roundTime <= 0f)
        {
            ended = true;
            float ratio = shots == 0 ? 0f : (float)hits / shots;
            if (resultText)
                resultText.text = ratio >= winRatio ? "WIN!" : "MISS!";
        }
    }
}
