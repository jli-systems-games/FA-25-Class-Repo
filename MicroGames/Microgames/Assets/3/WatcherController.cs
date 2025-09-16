using UnityEngine;
using TMPro;
using System.Collections;

public class WatcherController : MonoBehaviour
{
    [Header("Refs")]
    public GameObject headObject;     // 头（Sprite），默认隐藏
    public TMP_Text warningText;      // 文案 TMP Text（Canvas 下）

    [Header("Warning FX")]
    public float warnFadeIn = 0.2f;
    public float warnHold = 0.6f;   // 提示总计=warnFadeIn + warnHold + warnFadeOut（≈1秒）
    public float warnFadeOut = 0.2f;
    public float shakeDuration = 0.7f;
    public float shakeMagnitude = 8f; // 文本抖动幅度（像素）

    [TextArea]
    public string[] warningLines = new string[]
 {
    "What was that noise?",
    "Who's there?",
    "Anyone outside?",
    "Huh?",
    "This is private property!"
 };


    [Header("Timing (will ramp faster)")]
    public float idleMin = 2.0f;      // 初始：两次探头之间的最小间隔
    public float idleMax = 5.0f;      // 初始：最大间隔
    public float peekDuration = 3.0f; // 探头时间

    [Header("Difficulty Ramp")]
    public float rampFactor = 0.9f;   // 每轮乘以这个系数（越小越紧凑）
    public float minIdleMin = 0.8f;   // 下限
    public float minIdleMax = 1.6f;   // 下限

    [Header("SFX")]
    public AudioSource sfxSource;     // 2D AudioSource
    public AudioClip peekSfx;         // 探头音效（提示后伸头时播放一次）

    public bool IsPeeking { get; private set; }

    Coroutine loopCo;
    bool stopped;

    void Start()
    {
        if (headObject) headObject.SetActive(false);
        if (warningText) warningText.gameObject.SetActive(false);
        loopCo = StartCoroutine(CoLoop());
    }

    public void StopAll()
    {
        stopped = true;
        if (loopCo != null) StopCoroutine(loopCo);
        if (headObject) headObject.SetActive(false);
        if (warningText) warningText.gameObject.SetActive(false);
        IsPeeking = false;
    }

    IEnumerator CoLoop()
    {
        var rt = warningText ? warningText.rectTransform : null;
        Vector3 origin = rt ? rt.anchoredPosition3D : Vector3.zero;

        while (!stopped)
        {
            // 随机空闲
            float idle = Random.Range(idleMin, idleMax);
            yield return new WaitForSeconds(idle);
            if (stopped) yield break;

            // 文案随机 + 淡入/抖动/淡出
            if (warningText)
            {
                warningText.text = warningLines[Random.Range(0, warningLines.Length)];
                warningText.gameObject.SetActive(true);
                // 初始为红色半透明
                Color c = warningText.color; c.a = 0f; c.r = 1f; c.g = 0.2f; c.b = 0.2f; warningText.color = c;

                // 并行：淡入+抖动
                yield return StartCoroutine(FadeAndShake(warningText, rt, true, origin));
                // 保持
                yield return new WaitForSeconds(warnHold);
                // 淡出
                yield return StartCoroutine(FadeAndShake(warningText, rt, false, origin));
                warningText.gameObject.SetActive(false);
                if (rt) rt.anchoredPosition3D = origin;
            }

            // 探头 + 音效
            IsPeeking = true;
            if (headObject) headObject.SetActive(true);
            if (sfxSource && peekSfx) sfxSource.PlayOneShot(peekSfx);
            yield return new WaitForSeconds(peekDuration);

            // 收头
            IsPeeking = false;
            if (headObject) headObject.SetActive(false);

            // 难度递增：缩短间隔
            idleMin = Mathf.Max(minIdleMin, idleMin * rampFactor);
            idleMax = Mathf.Max(minIdleMax, idleMax * rampFactor);
        }
    }

    IEnumerator FadeAndShake(TMP_Text txt, RectTransform rt, bool fadeIn, Vector3 origin)
    {
        float t = 0f;
        float dur = fadeIn ? warnFadeIn : warnFadeOut;
        float shakeT = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / dur);

            // 透明度
            Color c = txt.color;
            c.a = fadeIn ? k : (1f - k);
            txt.color = c;

            // 抖动（在整个 warnFadeIn/Out 期间持续）
            if (rt && shakeDuration > 0f && shakeMagnitude > 0f)
            {
                shakeT += Time.deltaTime;
                float shakeK = Mathf.Clamp01(shakeT / shakeDuration);
                float mag = shakeMagnitude * (1f - shakeK); // 逐渐减弱
                Vector2 jitter = Random.insideUnitCircle * mag;
                rt.anchoredPosition3D = origin + new Vector3(jitter.x, jitter.y, 0f);
            }

            yield return null;
        }
    }
}
