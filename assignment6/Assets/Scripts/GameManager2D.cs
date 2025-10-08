using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager2D : MonoBehaviour
{
    public static GameManager2D I;

    [Header("HUD (TMP)")]
    public TMP_Text scoreText;      // Canvas/TopBar/ScoreText
    public TMP_Text speedText;      // Canvas/TopBar/SpeedText
    public TMP_Text tipText;        // Canvas/Tips

    [Header("End Panel")]
    public GameObject endPanel;     // Canvas/EndPanel
    public TMP_Text titleText;      // Canvas/EndPanel/TitleText
    public TMP_Text finalScoreText; // Canvas/EndPanel/FinalScoreText

    [Header("Refs")]
    public Train2DController_Pos train;

    [Header("Options")]
    [SerializeField] bool alwaysSyncHUD = true;   // 每帧同步 HUD
    [SerializeField] bool autoWireByPath = true;  // 按层级路径自动连线（当 Inspector 未手动绑定时生效）

    // -------- 分数脉冲（不叠加 + 上限）--------
    [Header("Score pulse (UI feedback)")]
    [SerializeField] float pulseScale = 1.06f;     // 每次脉冲目标倍数（1.03~1.10）
    [SerializeField] float pulseDuration = 0.10f;  // 动画时长（秒）
    [SerializeField] float pulseMax = 1.20f;       // 绝对上限
    [SerializeField] Color pulseColor = Color.white;
    Coroutine _pulseCo;
    Vector3 _scoreBaseScale = Vector3.one;

    int _score = 0;
    int _lastShownScore = int.MinValue;

    void Awake()
    {
        // 单例
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        // 自动连线（仅在对应字段为空时）
        if (autoWireByPath)
        {
            TryWireIfNull(ref scoreText, "Canvas/TopBar/ScoreText");
            TryWireIfNull(ref speedText, "Canvas/TopBar/SpeedText");
            TryWireIfNull(ref tipText, "Canvas/Tips");
            TryWireIfNull(ref titleText, "Canvas/EndPanel/TitleText");
            TryWireIfNull(ref finalScoreText, "Canvas/EndPanel/FinalScoreText");
            if (!endPanel)
            {
                var go = GameObject.Find("Canvas/EndPanel");
                if (go) endPanel = go;
            }
            if (!train) train = FindObjectOfType<Train2DController_Pos>();
        }

        if (endPanel) endPanel.SetActive(false);

        // 记住分数字体初始缩放
        if (scoreText) _scoreBaseScale = scoreText.rectTransform.localScale;

        // 初始 HUD
        SyncScoreUI(force: true);
        if (tipText && string.IsNullOrEmpty(tipText.text))
            tipText.text = "Tap / Space to switch";

        Debug.Log($"[GM] scoreText bound to: {PathOf(scoreText)}");
    }

    void Update()
    {
        if (speedText && train)
            speedText.text = $"Speed: {train.speed:0.0}";

        if (alwaysSyncHUD) SyncScoreUI();
    }

    // ======= 外部接口 =======

    public void AddScore(int s)
    {
        _score += s;
        Debug.Log($"[GM] score = {_score}");
        SyncScoreUI(force: true);
        StartScorePulse();
    }

    public void GameOver()
    {
        if (endPanel) endPanel.SetActive(true);
        if (titleText) titleText.text = "Game Over";
        if (finalScoreText) finalScoreText.text = $"Score: {_score}";
        // 可选：Time.timeScale = 0f;
    }

    public void Win()
    {
        if (endPanel) endPanel.SetActive(true);
        if (titleText) titleText.text = "You Win!";
        if (finalScoreText) finalScoreText.text = $"Score: {_score}";
        // 可选：Time.timeScale = 0f;
    }

    public void Restart()
    {
        //Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ======= 内部实现 =======

    void SyncScoreUI(bool force = false)
    {
        if (!scoreText) return;
        if (force || _lastShownScore != _score)
        {
            scoreText.SetText($"Score: {_score}");
            _lastShownScore = _score;
            Canvas.ForceUpdateCanvases(); // 保险刷新
        }
    }

    void StartScorePulse()
    {
        if (!scoreText) return;

        // 若上一次还在执行，先停止并恢复到基准，避免叠加
        if (_pulseCo != null)
        {
            StopCoroutine(_pulseCo);
            scoreText.rectTransform.localScale = _scoreBaseScale;
        }
        _pulseCo = StartCoroutine(PulseTMP_Clamped(scoreText, pulseScale, pulseDuration, pulseMax, pulseColor));
    }

    System.Collections.IEnumerator PulseTMP_Clamped(TMP_Text t, float scale, float dur, float maxScale, Color flash)
    {
        var rt = t.rectTransform;

        // 基准缩放：以记录的 _scoreBaseScale 为准
        Vector3 from = _scoreBaseScale;
        Vector3 to = _scoreBaseScale * Mathf.Min(scale, maxScale);

        Color orig = t.color;
        t.color = flash;

        float t0 = 0f;
        // ease-out（前快后慢）
        while (t0 < dur)
        {
            t0 += Time.unscaledDeltaTime; // 不受暂停影响
            float k = Mathf.Clamp01(t0 / dur);
            float e = 1f - (1f - k) * (1f - k); // easeOutQuad
            rt.localScale = Vector3.LerpUnclamped(from, to, e);
            yield return null;
        }

        // 回到基准
        rt.localScale = _scoreBaseScale;
        t.color = orig;
        _pulseCo = null;
    }

    void TryWireIfNull(ref TMP_Text target, string path)
    {
        if (target) return;
        var go = GameObject.Find(path);
        if (go) target = go.GetComponent<TMP_Text>();
    }

    static string PathOf(Component c)
    {
        if (!c) return "null";
        var t = c.transform;
        System.Text.StringBuilder sb = new();
        while (t != null) { sb.Insert(0, "/" + t.name); t = t.parent; }
        return sb.ToString();
    }
}
