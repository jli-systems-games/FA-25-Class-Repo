using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DDOLMusicManager : MonoBehaviour
{
    private static DDOLMusicManager instance;  // 单例
    private AudioSource audioSource;

    [Header("音乐是否在启动时自动播放")]
    public bool playOnStart = true;

    void Awake()
    {
        // 单例模式，避免重复
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (playOnStart && audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    // ✅ 对外提供的静态访问接口
    public static DDOLMusicManager Instance => instance;

    // ✅ 外部可调用：停止播放
    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    // ✅ 外部可调用：播放
    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    // ✅ 外部可调用：淡出停止
    public void FadeOut(float duration = 1f)
    {
        if (audioSource != null)
            StartCoroutine(FadeOutCoroutine(duration));
    }

    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // 还原音量
    }
}