using UnityEngine;

/// <summary>
/// 音乐系统管理器 - 简化重构版
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("游戏音乐配置")]
    public AudioClip preGameMusic;
    public bool preGameLoop = true;
    
    public AudioClip firstWaveMusic;
    public bool firstWaveLoop = true;
    
    public AudioClip restPhaseMusic;
    public bool restPhaseLoop = true;
    
    public AudioClip mainBattleMusic;
    public bool mainBattleLoop = true;

    [Header("结束音乐配置")]
    public AudioClip victoryMusic;
    public bool victoryLoop = false;
    
    public AudioClip defeatMusic;
    public bool defeatLoop = false;

    [Header("播放设置")]
    [Range(0f, 1f)]
    public float volume = 0.7f;

    private AudioSource audioSource;
    private bool hasPlayedFirstWave = false;

    void Awake()
    {
        // 单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化AudioSource
        InitAudioSource();
    }

    void InitAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 最简单的配置
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
        audioSource.pitch = 1f;
        audioSource.spatialBlend = 0f; // 2D
    }

    void Start()
    {
        PlayPreGameMusic();
    }

    void Update()
    {
        // 强制锁定pitch
        if (audioSource != null && audioSource.pitch != 1f)
        {
            audioSource.pitch = 1f;
        }
    }

    public void PlayPreGameMusic()
    {
        if (preGameMusic != null)
        {
            PlayClip(preGameMusic, preGameLoop);
        }
    }

    public void PlayFirstWaveMusic()
    {
        if (firstWaveMusic != null && !hasPlayedFirstWave)
        {
            hasPlayedFirstWave = true;
            PlayClip(firstWaveMusic, firstWaveLoop);
        }
    }

    public void PlayRestPhaseMusic()
    {
        if (restPhaseMusic != null)
        {
            PlayClip(restPhaseMusic, restPhaseLoop);
        }
    }

    public void PlayMainBattleMusic()
    {
        if (mainBattleMusic != null && hasPlayedFirstWave)
        {
            PlayClip(mainBattleMusic, mainBattleLoop);
        }
    }

    public void PlayVictoryMusic()
    {
        if (victoryMusic != null)
        {
            PlayClip(victoryMusic, victoryLoop);
        }
    }

    public void PlayDefeatMusic()
    {
        if (defeatMusic != null)
        {
            PlayClip(defeatMusic, defeatLoop);
        }
    }

    private void PlayClip(AudioClip clip, bool loop)
    {
        if (audioSource == null || clip == null) return;

        // 如果已经在播放相同音乐，不重复播放
        if (audioSource.isPlaying && audioSource.clip == clip)
        {
            return;
        }

        // 停止当前音乐
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 播放新音乐
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.pitch = 1f;
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    public void ResetMusicSystem()
    {
        hasPlayedFirstWave = false;
        StopMusic();
    }
}
