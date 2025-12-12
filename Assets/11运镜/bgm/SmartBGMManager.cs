using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// 智能BGM管理器
/// 支持多场景音乐切换，记住每首音乐的播放位置
/// </summary>
public class SmartBGMManager : MonoBehaviour
{
    #region 单例模式
    private static SmartBGMManager instance;
    public static SmartBGMManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SmartBGMManager");
                instance = go.AddComponent<SmartBGMManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    #endregion

    [Header("音乐库")]
    [Tooltip("所有可用的BGM（给每首音乐起个名字）")]
    public List<BGMData> bgmLibrary = new List<BGMData>();

    [Header("场景音乐配置")]
    [Tooltip("每个场景应该播放哪首BGM")]
    public List<SceneBGMConfig> sceneBGMConfigs = new List<SceneBGMConfig>();

    [Header("淡入淡出设置")]
    [Tooltip("切换音乐时的淡出时间（秒）")]
    public float fadeOutDuration = 1f;

    [Tooltip("切换音乐时的淡入时间（秒）")]
    public float fadeInDuration = 1f;

    [Header("音量设置")]
    [Tooltip("BGM主音量")]
    [Range(0f, 1f)]
    public float masterVolume = 0.5f;

    [Header("调试")]
    public bool showDebugInfo = true;

    private AudioSource audioSource;
    private Dictionary<string, float> bgmPositions = new Dictionary<string, float>(); // 记录每首音乐的播放位置
    private string currentBGMName = "";
    private bool isFading = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
            SceneManager.sceneLoaded += OnSceneLoaded;

            if (showDebugInfo)
            {
                Debug.Log("SmartBGMManager 初始化完成");
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void InitializeAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = masterVolume;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (showDebugInfo)
        {
            Debug.Log($"场景加载: {scene.name}");
        }

        // 查找这个场景应该播放什么音乐
        string targetBGMName = GetBGMForScene(scene.name);

        if (string.IsNullOrEmpty(targetBGMName))
        {
            if (showDebugInfo)
            {
                Debug.Log($"场景 {scene.name} 没有配置BGM");
            }
            return;
        }

        // 如果是同一首音乐，继续播放
        if (targetBGMName == currentBGMName)
        {
            if (showDebugInfo)
            {
                Debug.Log($"继续播放当前BGM: {currentBGMName}");
            }
            return;
        }

        // 切换到新音乐
        SwitchBGM(targetBGMName);
    }

    /// <summary>
    /// 获取指定场景应该播放的BGM名称
    /// </summary>
    string GetBGMForScene(string sceneName)
    {
        foreach (var config in sceneBGMConfigs)
        {
            if (config.sceneName == sceneName)
            {
                return config.bgmName;
            }
        }
        return "";
    }

    /// <summary>
    /// 切换BGM
    /// </summary>
    void SwitchBGM(string newBGMName)
    {
        if (isFading) return;

        if (showDebugInfo)
        {
            Debug.Log($"切换BGM: {currentBGMName} → {newBGMName}");
        }

        // 保存当前音乐的播放位置
        if (!string.IsNullOrEmpty(currentBGMName) && audioSource.isPlaying)
        {
            bgmPositions[currentBGMName] = audioSource.time;
            if (showDebugInfo)
            {
                Debug.Log($"保存 {currentBGMName} 的播放位置: {audioSource.time:F2}秒");
            }
        }

        // 获取新音乐的AudioClip
        AudioClip newClip = GetBGMClip(newBGMName);
        if (newClip == null)
        {
            Debug.LogWarning($"找不到BGM: {newBGMName}");
            return;
        }

        // 获取新音乐之前保存的播放位置
        float startTime = 0f;
        if (bgmPositions.ContainsKey(newBGMName))
        {
            startTime = bgmPositions[newBGMName];
            if (showDebugInfo)
            {
                Debug.Log($"从保存的位置继续播放 {newBGMName}: {startTime:F2}秒");
            }
        }

        // 开始淡入淡出切换
        StartCoroutine(FadeTransition(newClip, startTime, newBGMName));
    }

    /// <summary>
    /// 淡入淡出切换音乐
    /// </summary>
    System.Collections.IEnumerator FadeTransition(AudioClip newClip, float startTime, string newBGMName)
    {
        isFading = true;

        // 淡出当前音乐
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutDuration);
            yield return null;
        }

        // 切换音乐
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.time = startTime;
        audioSource.Play();
        currentBGMName = newBGMName;

        // 淡入新音乐
        timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, masterVolume, timer / fadeInDuration);
            yield return null;
        }

        audioSource.volume = masterVolume;
        isFading = false;
    }

    /// <summary>
    /// 根据名称获取BGM的AudioClip
    /// </summary>
    AudioClip GetBGMClip(string bgmName)
    {
        foreach (var bgm in bgmLibrary)
        {
            if (bgm.bgmName == bgmName)
            {
                return bgm.audioClip;
            }
        }
        return null;
    }

    /// <summary>
    /// 手动播放指定BGM
    /// </summary>
    public void PlayBGM(string bgmName, bool fromStart = false)
    {
        if (fromStart && bgmPositions.ContainsKey(bgmName))
        {
            bgmPositions[bgmName] = 0f;
        }
        SwitchBGM(bgmName);
    }

    /// <summary>
    /// 暂停BGM
    /// </summary>
    public void PauseBGM()
    {
        if (audioSource.isPlaying)
        {
            bgmPositions[currentBGMName] = audioSource.time;
            audioSource.Pause();

            if (showDebugInfo)
            {
                Debug.Log($"暂停BGM: {currentBGMName}");
            }
        }
    }

    /// <summary>
    /// 恢复播放BGM
    /// </summary>
    public void ResumeBGM()
    {
        audioSource.UnPause();

        if (showDebugInfo)
        {
            Debug.Log($"恢复播放BGM: {currentBGMName}");
        }
    }

    /// <summary>
    /// 停止BGM（清除播放位置）
    /// </summary>
    public void StopBGM()
    {
        if (!string.IsNullOrEmpty(currentBGMName))
        {
            bgmPositions.Remove(currentBGMName);
        }
        audioSource.Stop();
        currentBGMName = "";

        if (showDebugInfo)
        {
            Debug.Log("停止BGM");
        }
    }

    /// <summary>
    /// 设置主音量
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (!isFading)
        {
            audioSource.volume = masterVolume;
        }
    }

    /// <summary>
    /// 清除所有保存的播放位置
    /// </summary>
    public void ClearAllPositions()
    {
        bgmPositions.Clear();

        if (showDebugInfo)
        {
            Debug.Log("已清除所有BGM播放位置");
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

/// <summary>
/// BGM数据
/// </summary>
[System.Serializable]
public class BGMData
{
    [Tooltip("BGM的名称（用于识别）")]
    public string bgmName;

    [Tooltip("音乐文件")]
    public AudioClip audioClip;
}

/// <summary>
/// 场景BGM配置
/// </summary>
[System.Serializable]
public class SceneBGMConfig
{
    [Tooltip("场景名称")]
    public string sceneName;

    [Tooltip("这个场景播放哪首BGM")]
    public string bgmName;
}