using UnityEngine;
using System.Collections;

/// <summary>
/// 音乐淡入淡出控制器
/// 可以通过按钮或代码触发音乐的淡入淡出效果
/// </summary>
public class MusicFadeController : MonoBehaviour
{
    [Header("音频设置")]
    [Tooltip("要控制的AudioSource（如果为空则自动查找）")]
    public AudioSource audioSource;

    [Header("淡出设置")]
    [Tooltip("淡出时间（秒）")]
    public float fadeOutDuration = 2f;

    [Tooltip("淡出到的最小音量")]
    public float fadeOutTargetVolume = 0f;

    [Header("淡入设置")]
    [Tooltip("淡入时间（秒）")]
    public float fadeInDuration = 2f;

    [Tooltip("淡入到的最大音量")]
    public float fadeInTargetVolume = 1f;

    [Header("淡出后行为")]
    [Tooltip("淡出完成后是否停止音乐")]
    public bool stopAfterFadeOut = true;

    [Tooltip("淡出完成后是否暂停音乐（不停止）")]
    public bool pauseAfterFadeOut = false;

    private float originalVolume;
    private Coroutine currentFadeCoroutine;

    void Awake()
    {
        // 如果没有指定AudioSource，尝试自动获取
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
            if (audioSource != null)
            {
                Debug.Log($"[MusicFadeController] 自动找到AudioSource: {audioSource.gameObject.name}");
            }
        }

        if (audioSource != null)
        {
            originalVolume = audioSource.volume;
        }
    }

    /// <summary>
    /// 淡出音乐（按钮调用这个）
    /// </summary>
    public void FadeOut()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("[MusicFadeController] AudioSource未设置！");
            return;
        }

        // 如果正在执行淡入淡出，先停止
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeOutCoroutine());
    }

    /// <summary>
    /// 淡入音乐（按钮调用这个）
    /// </summary>
    public void FadeIn()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("[MusicFadeController] AudioSource未设置！");
            return;
        }

        // 如果正在执行淡入淡出，先停止
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        // 如果音乐没在播放，先播放
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        currentFadeCoroutine = StartCoroutine(FadeInCoroutine());
    }

    /// <summary>
    /// 淡出协程
    /// </summary>
    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        Debug.Log($"[MusicFadeController] 开始淡出音乐 从 {startVolume} 到 {fadeOutTargetVolume}，耗时 {fadeOutDuration}秒");

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeOutDuration;

            // 使用平滑插值
            audioSource.volume = Mathf.Lerp(startVolume, fadeOutTargetVolume, progress);

            yield return null;
        }

        // 确保最终音量精确
        audioSource.volume = fadeOutTargetVolume;

        Debug.Log("[MusicFadeController] 淡出完成");

        // 淡出完成后的行为
        if (stopAfterFadeOut)
        {
            audioSource.Stop();
            Debug.Log("[MusicFadeController] 音乐已停止");
        }
        else if (pauseAfterFadeOut)
        {
            audioSource.Pause();
            Debug.Log("[MusicFadeController] 音乐已暂停");
        }

        currentFadeCoroutine = null;
    }

    /// <summary>
    /// 淡入协程
    /// </summary>
    private IEnumerator FadeInCoroutine()
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        Debug.Log($"[MusicFadeController] 开始淡入音乐 从 {startVolume} 到 {fadeInTargetVolume}，耗时 {fadeInDuration}秒");

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeInDuration;

            // 使用平滑插值
            audioSource.volume = Mathf.Lerp(startVolume, fadeInTargetVolume, progress);

            yield return null;
        }

        // 确保最终音量精确
        audioSource.volume = fadeInTargetVolume;

        Debug.Log("[MusicFadeController] 淡入完成");

        currentFadeCoroutine = null;
    }

    /// <summary>
    /// 自定义淡出（指定时间和目标音量）
    /// </summary>
    public void FadeOutCustom(float duration, float targetVolume)
    {
        if (audioSource == null) return;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeOutCustomCoroutine(duration, targetVolume));
    }

    private IEnumerator FadeOutCustomCoroutine(float duration, float targetVolume)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        if (stopAfterFadeOut && targetVolume <= 0f)
        {
            audioSource.Stop();
        }

        currentFadeCoroutine = null;
    }

    /// <summary>
    /// 立即停止音乐（不淡出）
    /// </summary>
    public void StopImmediate()
    {
        if (audioSource == null) return;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }

        audioSource.Stop();
        audioSource.volume = originalVolume;
    }

    /// <summary>
    /// 恢复原始音量
    /// </summary>
    public void ResetVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = originalVolume;
        }
    }

    /// <summary>
    /// 检查是否正在淡出/淡入
    /// </summary>
    public bool IsFading()
    {
        return currentFadeCoroutine != null;
    }
}