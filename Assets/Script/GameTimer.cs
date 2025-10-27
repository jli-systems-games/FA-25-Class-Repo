using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float duration = 60f;
    public bool useUnscaledTime = false;
    public int targetSceneBuildIndex = 0;
    public TMP_Text timerText;

    float remain;
    bool ended;

    void Start()
    {
        remain = Mathf.Max(0f, duration);
        UpdateUI();
    }

    void Update()
    {
        if (ended) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        remain -= dt;
        if (remain <= 0f)
        {
            EndNow();
            return;
        }
        UpdateUI();
    }

    void EndNow()
    {
        ended = true;
        SceneManager.LoadScene(targetSceneBuildIndex);
    }

    void UpdateUI()
    {
        if (!timerText) return;

        float t = Mathf.Max(0f, remain);
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        timerText.text = $"{s:00}";
    }
}