using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 内存优化管理器 - 主动清理和优化游戏内存，防止内存泄漏
/// 将此脚本挂载到场景中的空 GameObject 上
/// </summary>
public class MemoryOptimizer : MonoBehaviour
{
    [Header("自动清理设置")]
    [Tooltip("是否启用自动内存清理")]
    [SerializeField] private bool enableAutoCleanup = true;
    
    [Tooltip("自动清理间隔（秒）- 推荐 60-120 秒")]
    [SerializeField] private float cleanupInterval = 90f;
    
    [Tooltip("是否在场景切换时强制清理")]
    [SerializeField] private bool cleanupOnSceneChange = true;
    
    [Tooltip("智能清理：避免在战斗/移动时清理")]
    [SerializeField] private bool smartCleanup = true;
    
    [Header("垃圾回收设置")]
    [Tooltip("是否定期强制垃圾回收")]
    [SerializeField] private bool enableGarbageCollection = false;
    
    [Tooltip("垃圾回收间隔（秒）")]
    [SerializeField] private float gcInterval = 180f;
    
    [Header("资源卸载设置")]
    [Tooltip("是否卸载未使用的资源（可能导致卡顿）")]
    [SerializeField] private bool unloadUnusedAssets = false;
    
    [Tooltip("资源卸载间隔（秒）")]
    [SerializeField] private float unloadInterval = 300f;
    
    [Header("调试设置")]
    [Tooltip("是否显示调试日志")]
    [SerializeField] private bool showDebugLogs = true;
    
    [Tooltip("是否显示内存使用信息")]
    [SerializeField] private bool showMemoryStats = true;
    
    [Tooltip("内存统计更新间隔（秒）")]
    [SerializeField] private float memoryStatsInterval = 10f;

    private float lastCleanupTime;
    private float lastGCTime;
    private float lastUnloadTime;
    private float lastMemoryStatsTime;
    
    // 单例模式
    private static MemoryOptimizer instance;
    public static MemoryOptimizer Instance => instance;

    private void Awake()
    {
        // 只在运行时工作，编辑器模式和构建时不运行
        if (!Application.isPlaying)
        {
            enabled = false;
            return;
        }
        
        // 确保只有一个实例
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        Log("内存优化器已启动");
        
        // 初始化时间
        lastCleanupTime = Time.realtimeSinceStartup;
        lastGCTime = Time.realtimeSinceStartup;
        lastUnloadTime = Time.realtimeSinceStartup;
        lastMemoryStatsTime = Time.realtimeSinceStartup;
        
        // 注册场景切换事件
        if (cleanupOnSceneChange)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
        
        if (cleanupOnSceneChange)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Update()
    {
        float currentTime = Time.realtimeSinceStartup;
        
        // 智能清理：检查是否适合清理
        bool canCleanup = !smartCleanup || IsGoodTimeToCleanup();
        
        // 自动清理
        if (enableAutoCleanup && canCleanup && currentTime - lastCleanupTime >= cleanupInterval)
        {
            PerformMemoryCleanup();
            lastCleanupTime = currentTime;
        }
        
        // 垃圾回收（降低频率）
        if (enableGarbageCollection && canCleanup && currentTime - lastGCTime >= gcInterval)
        {
            PerformGarbageCollection();
            lastGCTime = currentTime;
        }
        
        // 卸载未使用资源（降低频率）
        if (unloadUnusedAssets && canCleanup && currentTime - lastUnloadTime >= unloadInterval)
        {
            StartCoroutine(UnloadUnusedAssetsAsync());
            lastUnloadTime = currentTime;
        }
        
        // 内存统计
        if (showMemoryStats && currentTime - lastMemoryStatsTime >= memoryStatsInterval)
        {
            LogMemoryStats();
            lastMemoryStatsTime = currentTime;
        }
    }
    
    /// <summary>
    /// 检查是否适合执行清理（避免在关键时刻清理）
    /// </summary>
    private bool IsGoodTimeToCleanup()
    {
        // 检查玩家是否在快速移动（可能在战斗中）
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null && rb.linearVelocity.magnitude > 50f) // 高速移动时不清理
            {
                return false;
            }
        }
        
        // 检查帧率（帧率低时不清理，避免雪上加霜）
        if (Time.deltaTime > 0.033f) // 低于 30 FPS
        {
            return false;
        }
        
        // 检查是否在加载
        if (Time.timeScale < 0.1f) // 游戏暂停时可以清理
        {
            return true;
        }
        
        return true;
    }

    /// <summary>
    /// 执行内存清理
    /// </summary>
    public void PerformMemoryCleanup()
    {
        Log("开始执行内存清理...");
        
        // 清理废弃的材质实例
        CleanupOrphanedMaterials();
        
        // 清理 RenderTexture
        CleanupRenderTextures();
        
        // 清理音频资源
        CleanupAudioResources();
        
        Log("内存清理完成");
    }

    /// <summary>
    /// 清理孤立的材质（轻量级版本）
    /// </summary>
    private void CleanupOrphanedMaterials()
    {
        // 轻量级清理：不使用 Resources.UnloadUnusedAssets（会卡顿）
        // 只清理明确废弃的材质
        
        // 触发 GC 来清理引用计数为 0 的对象
        System.GC.Collect(0, System.GCCollectionMode.Optimized);
        
        Log("已执行轻量级材质清理");
    }

    /// <summary>
    /// 清理 RenderTexture
    /// </summary>
    private void CleanupRenderTextures()
    {
        // 查找所有激活的 RenderTexture 并释放未使用的
        RenderTexture[] renderTextures = FindObjectsByType<RenderTexture>(FindObjectsSortMode.None);
        int released = 0;
        
        foreach (RenderTexture rt in renderTextures)
        {
            if (rt != null && !rt.IsCreated())
            {
                rt.Release();
                released++;
            }
        }
        
        if (released > 0)
        {
            Log($"已释放 {released} 个 RenderTexture");
        }
    }

    /// <summary>
    /// 清理音频资源（改进版：不影响音乐）
    /// </summary>
    private void CleanupAudioResources()
    {
        // 只清理已停止且不是音乐的音频源
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        int cleaned = 0;
        
        foreach (AudioSource source in audioSources)
        {
            if (source != null && 
                !source.isPlaying && 
                source.clip != null && 
                !source.loop && // 不清理循环音频（通常是音乐）
                !source.gameObject.activeInHierarchy) // 只清理未激活对象的音频
            {
                // 仅清理明确未使用的音频
                source.clip = null;
                cleaned++;
            }
        }
        
        if (cleaned > 0)
        {
            Log($"已清理 {cleaned} 个未使用的音频资源");
        }
    }

    /// <summary>
    /// 执行垃圾回收
    /// </summary>
    public void PerformGarbageCollection()
    {
        Log("执行垃圾回收...");
        
        // 执行两次 GC 以确保彻底清理
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
        
        Log("垃圾回收完成");
    }

    /// <summary>
    /// 异步卸载未使用的资源
    /// </summary>
    private IEnumerator UnloadUnusedAssetsAsync()
    {
        Log("开始卸载未使用的资源...");
        
        AsyncOperation operation = Resources.UnloadUnusedAssets();
        
        while (!operation.isDone)
        {
            yield return null;
        }
        
        // 卸载后执行垃圾回收
        System.GC.Collect();
        
        Log("未使用资源卸载完成");
    }

    /// <summary>
    /// 记录内存统计信息
    /// </summary>
    private void LogMemoryStats()
    {
        if (!showMemoryStats) return;
        
        float totalMemoryMB = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1048576f;
        float allocatedMemoryMB = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
        float unusedMemoryMB = UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong() / 1048576f;
        float monoUsedMB = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / 1048576f;
        float monoHeapMB = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong() / 1048576f;
        
        Log($"=== 内存统计 ===\n" +
            $"总预留: {totalMemoryMB:F2} MB\n" +
            $"已分配: {allocatedMemoryMB:F2} MB\n" +
            $"未使用: {unusedMemoryMB:F2} MB\n" +
            $"Mono 使用: {monoUsedMB:F2} MB\n" +
            $"Mono 堆: {monoHeapMB:F2} MB");
    }

    /// <summary>
    /// 场景加载时的清理
    /// </summary>
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Log($"场景已加载: {scene.name}，执行清理...");
        
        // 延迟执行，等待场景完全加载
        StartCoroutine(CleanupAfterSceneLoad());
    }

    private IEnumerator CleanupAfterSceneLoad()
    {
        // 等待一帧，让场景完全加载
        yield return new WaitForEndOfFrame();
        
        // 执行完整清理
        PerformMemoryCleanup();
        yield return UnloadUnusedAssetsAsync();
        PerformGarbageCollection();
        
        Log("场景切换清理完成");
    }

    /// <summary>
    /// 手动触发完整清理（可从其他脚本调用）
    /// </summary>
    public void ForceFullCleanup()
    {
        Log("强制执行完整清理...");
        StartCoroutine(PerformFullCleanup());
    }

    private IEnumerator PerformFullCleanup()
    {
        PerformMemoryCleanup();
        yield return UnloadUnusedAssetsAsync();
        PerformGarbageCollection();
        LogMemoryStats();
        Log("完整清理已完成");
    }

    /// <summary>
    /// 日志输出
    /// </summary>
    private void Log(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[内存优化器] {message}");
        }
    }

    /// <summary>
    /// 在编辑器中显示内存信息
    /// </summary>
    private void OnGUI()
    {
        if (!showMemoryStats || !Application.isPlaying) return;
        
        GUIStyle style = new GUIStyle();
        style.fontSize = 12;
        style.normal.textColor = Color.green;
        style.padding = new RectOffset(10, 10, 10, 10);
        
        float allocatedMB = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
        float monoMB = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / 1048576f;
        
        GUI.Label(new Rect(10, 10, 300, 60), 
            $"内存使用: {allocatedMB:F1} MB\n" +
            $"Mono 内存: {monoMB:F1} MB\n" +
            $"下次清理: {Mathf.Max(0, cleanupInterval - (Time.realtimeSinceStartup - lastCleanupTime)):F0}秒", 
            style);
    }
}

