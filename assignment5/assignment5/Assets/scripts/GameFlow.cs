using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-1000)]
public class GameFlow : MonoBehaviour
{
    public static GameFlow I;

    [Header("UI")]
    public CanvasGroup gameOverUI;
    public CanvasGroup winUI;

    [Header("失败血红遮罩")]
    public CanvasGroup bloodOverlay;
    public float bloodFadeSpeed = 2.5f;

    [Header("音乐（可选）")]
    public AudioSource tensionMusic;
    public float musicFadePerSec = 0.6f;

    [Header("调试(出包可见)")]
    public bool showHud = true;

    bool ended = false;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // 紧张音乐淡入/出
        if (tensionMusic && NoiseSystem.I)
        {
            float target = (NoiseSystem.I.IsWarning() || NoiseSystem.I.IsCritical()) ? 0.6f : 0f;
            tensionMusic.volume = Mathf.MoveTowards(tensionMusic.volume, target, musicFadePerSec * Time.deltaTime);
        }

        // 热键自检（Build 里也可用）
        if (Input.GetKeyDown(KeyCode.K)) GameOver();
        if (Input.GetKeyDown(KeyCode.L)) Win();

        // 结束后允许 R 重开
        if (ended && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void GameOver()
    {
        if (ended) return;
        ended = true;

        var camShake = Camera.main ? Camera.main.GetComponent<CameraShake>() : null;
        if (camShake) camShake.Shake(0.5f, 0.4f);

        Show(gameOverUI, true);
        if (bloodOverlay) StartCoroutine(FadeCanvas(bloodOverlay, 1f, bloodFadeSpeed));

        // 解锁鼠标，确保可点 UI（出包重要）
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(PauseLateRealtime(0.35f));
    }

    public void Win()
    {
        if (ended) return;
        ended = true;

        Show(winUI, true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(PauseLateRealtime(0.15f));
    }

    void Show(CanvasGroup cg, bool v)
    {
        if (!cg) return;
        cg.alpha = v ? 1f : 0f;
        cg.blocksRaycasts = v;
        cg.interactable = v;
    }

    IEnumerator PauseLateRealtime(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 0f;
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float targetAlpha, float speed)
    {
        if (!cg) yield break;
        while (!Mathf.Approximately(cg.alpha, targetAlpha))
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, speed * Time.unscaledDeltaTime);
            yield return null;
        }
    }

    // 屏幕 HUD（Build 中观测关键状态）
    void OnGUI()
    {
        if (!showHud) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 14;
        style.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(10, 10, 520, 160), GUI.skin.box);

        GUILayout.Label("GF Alive: " + (I == this), style);
        GUILayout.Label("Ended: " + ended + "  |  TimeScale: " + Time.timeScale.ToString("F2"), style);

        // WinUI 行
        string winStr =
            "WinUI: " +
            (winUI ? winUI.alpha.ToString("F2") : "null") +
            ", blocks=" + (winUI ? winUI.blocksRaycasts.ToString() : "false") +
            ", inter=" + (winUI ? winUI.interactable.ToString() : "false");
        GUILayout.Label(winStr, style);

        // GameOver 行
        string overStr =
            "OverUI: " +
            (gameOverUI ? gameOverUI.alpha.ToString("F2") : "null") +
            ", blocks=" + (gameOverUI ? gameOverUI.blocksRaycasts.ToString() : "false") +
            ", inter=" + (gameOverUI ? gameOverUI.interactable.ToString() : "false");
        GUILayout.Label(overStr, style);

        // 噪音
        string noiseStr = "Noise: " + (NoiseSystem.I ? NoiseSystem.I.noiseLevel.ToString("F2") : "-");
        GUILayout.Label(noiseStr, style);

        GUILayout.Label("Hotkeys: K=GameOver, L=Win, R=Reload", style);

        GUILayout.EndArea();
    }

}
