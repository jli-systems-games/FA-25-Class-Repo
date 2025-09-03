using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerPong : MonoBehaviour
{
    [Header("数据来源")]
    public PaintManager3D paint;

    [Header("比赛时长（秒）")]
    public float matchDuration = 90f;

    [Header("UI 显示（可选）")]
    public TextMeshProUGUI dayText;     // 实时 Day: 123
    public TextMeshProUGUI nightText;   // 实时 Night: 98
    public TextMeshProUGUI timeText;    // 实时 Time: 01:28
    public TextMeshProUGUI resultText;  // 结束瞬间 "Day Wins/Night Wins/Draw"

    [Header("结束行为")]
    public UnityEvent onMatchEnd;       // 停输入/停球等（Inspector 里挂）
    [Tooltip("日方胜利时跳转的场景名（优先）")]
    public string dayWinSceneName = "";
    [Tooltip("月方胜利时跳转的场景名（优先）")]
    public string nightWinSceneName = "";
    [Tooltip("平局时跳转的场景名（留空则不跳）")]
    public string drawSceneName = "";

    [Tooltip("若未填场景名，则使用以下 BuildIndex")]
    public int dayWinBuildIndex = -1;
    public int nightWinBuildIndex = -1;
    public int drawBuildIndex = -1;

    [Tooltip("结束后延迟多少秒再切场（给结果文本留展示时间）")]
    public float loadSceneDelay = 0.5f;

    float timeLeft;
    bool ended;

    void Start()
    {
        timeLeft = matchDuration;
        UpdateHUD(); // 先刷一遍
    }

    void Update()
    {
        if (ended) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            EndMatchAndRoute();
        }
        else
        {
            UpdateHUD();
        }
    }

    void UpdateHUD()
    {
        if (paint)
        {
            if (dayText) dayText.text = $"Day: {paint.dayCount}";
            if (nightText) nightText.text = $"Night: {paint.nightCount}";
        }

        if (timeText)
        {
            // 显示用 FloorToInt，避免卡在 1；结束时会再刷成 00:00
            int t = Mathf.Max(0, Mathf.FloorToInt(timeLeft));
            int mm = t / 60, ss = t % 60;
            timeText.text = $"Time: {mm:00}:{ss:00}";
        }
    }

    void EndMatchAndRoute()
    {
        ended = true;
        timeLeft = 0f;

        // 最后再刷一次 UI，确保时间显示成 00:00
        if (timeText) timeText.text = "Time: 00:00";

        int day = paint ? paint.dayCount : 0;
        int night = paint ? paint.nightCount : 0;

        string result =
            day > night ? "Day Wins" :
            day < night ? "Night Wins" :
                          "Draw";

        if (resultText) resultText.text = result;

        // 触发外部事件（停输入/停球/音效…）
        onMatchEnd?.Invoke();

        // 按结果决定要去的场景
        if (day > night)
        {
            StartCoroutine(LoadSceneAfterDelay(dayWinSceneName, dayWinBuildIndex, loadSceneDelay));
        }
        else if (day < night)
        {
            StartCoroutine(LoadSceneAfterDelay(nightWinSceneName, nightWinBuildIndex, loadSceneDelay));
        }
        else // 平局
        {
            // 你可以选择：重开、进某个平局场景、或留在当前场景
            if (!string.IsNullOrEmpty(drawSceneName) || drawBuildIndex >= 0)
                StartCoroutine(LoadSceneAfterDelay(drawSceneName, drawBuildIndex, loadSceneDelay));
            // 否则什么也不做，只显示结果
        }
    }

    System.Collections.IEnumerator LoadSceneAfterDelay(string sceneName, int buildIndex, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else if (buildIndex >= 0)
        {
            SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("[GameManagerPong] 未配置要跳转的场景（名字或 BuildIndex）。");
        }
    }

    public float GetTimeLeft() => Mathf.Max(0f, timeLeft);
}