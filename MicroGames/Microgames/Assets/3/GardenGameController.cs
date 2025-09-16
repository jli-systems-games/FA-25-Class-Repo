using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GardenGameController : MonoBehaviour
{
    public static GardenGameController I;

    [Header("Refs")]
    public PoopManController poopMan;
    public WatcherController watcher;
    public TMP_Text hudTimer;            // 可选 HUD

    [Header("Win/Fail")]
    public float targetCrouchSeconds = 15f;
    public string winScene = "Scene3";
    public string failScene = "Scene1";

    [Header("Screen Fade")]
    public CanvasGroup blackFade;        // 全屏 UI（黑），Alpha=1 开始
    public float fadeDuration = 0.8f;

    [Header("Audio")]
    public AudioSource bgmSource;        // 循环 BGM（Loop=✓）
    public AudioClip bgmClip;
    public AudioSource uiSfxSource;      // 2D SFX
    public AudioClip winSfx;
    public AudioClip failSfx;

    float crouchAccum = 0f;
    bool gameOver = false;

    void Awake() { I = this; }

    void Start()
    {
        // 黑屏淡入
        if (blackFade)
        {
            blackFade.alpha = 1f;
            StartCoroutine(FadeCanvas(blackFade, 1f, 0f, fadeDuration));
        }

        // 播 BGM
        if (bgmSource && bgmClip)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    void Update()
    {
        if (gameOver) return;
        if (!poopMan || !watcher) return;

        // 失败判定：探头 & 蹲着 同时
        if (watcher.IsPeeking && poopMan.IsCrouching)
        {
            Fail();
            return;
        }

        // 胜利计时：只在蹲着时累计
        if (poopMan.IsCrouching) crouchAccum += Time.deltaTime;

        if (hudTimer) hudTimer.text = $"Already Poop:{crouchAccum:0.0}s / {targetCrouchSeconds:0}s";

        if (crouchAccum >= targetCrouchSeconds)
        {
            Win();
        }
    }

    public void Win()
    {
        if (gameOver) return;
        gameOver = true;
        watcher?.StopAll();
        if (uiSfxSource && winSfx) uiSfxSource.PlayOneShot(winSfx);
        StartCoroutine(FadeOutAndLoad(winScene));
    }

    public void Fail()
    {
        if (gameOver) return;
        gameOver = true;
        watcher?.StopAll();
        if (uiSfxSource && failSfx) uiSfxSource.PlayOneShot(failSfx);
        StartCoroutine(FadeOutAndLoad(failScene));
    }

    IEnumerator FadeOutAndLoad(string scene)
    {
        if (blackFade)
            yield return FadeCanvas(blackFade, blackFade.alpha, 1f, fadeDuration);
        SceneManager.LoadScene(scene);
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float dur)
    {
        float t = 0f;
        cg.blocksRaycasts = true;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / dur);
            cg.alpha = Mathf.Lerp(from, to, k);
            yield return null;
        }
        cg.alpha = to;
        if (to <= 0.001f) cg.blocksRaycasts = false;
    }
}
