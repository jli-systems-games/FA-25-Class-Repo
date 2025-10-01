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

    [Header("紧张音乐（可选）")]
    public AudioSource tensionMusic;
    public float musicFadePerSec = 0.6f;

    bool ended = false;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[GameFlow] Awake on " + name);
    }

    void OnDestroy()
    {
        if (I == this) I = null;
        StopAllCoroutines();
    }

    void Update()
    {
        if (tensionMusic && NoiseSystem.I)
        {
            float target = (NoiseSystem.I.IsWarning() || NoiseSystem.I.IsCritical()) ? 0.6f : 0f;
            tensionMusic.volume = Mathf.MoveTowards(
                tensionMusic.volume,
                target,
                musicFadePerSec * Time.deltaTime
            );
        }

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

        StartCoroutine(PauseLateRealtime(0.35f));
    }

    public void Win()
    {
        if (ended) return;
        ended = true;

        Show(winUI, true);

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
}
