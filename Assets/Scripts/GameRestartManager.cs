using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 游戏重启管理器
/// 按 R 键重启游戏，自动清理内存和重置状态
/// 挂载到场景中的空 GameObject 上
/// </summary>
public class GameRestartManager : MonoBehaviour
{
    [Header("重启设置")]
    [Tooltip("重启按键")]
    [SerializeField] private KeyCode restartKey = KeyCode.R;
    
    [Tooltip("是否需要长按确认（防止误触）")]
    [SerializeField] private bool requireHold = false;
    
    [Tooltip("长按确认时间（秒）")]
    [SerializeField] private float holdDuration = 1.5f;
    
    [Header("清理设置")]
    [Tooltip("重启前是否清理内存")]
    [SerializeField] private bool cleanupMemoryBeforeRestart = true;
    
    [Tooltip("是否执行垃圾回收")]
    [SerializeField] private bool performGarbageCollection = true;
    
    [Tooltip("是否卸载未使用资源")]
    [SerializeField] private bool unloadUnusedAssets = true;
    
    [Header("状态重置")]
    [Tooltip("是否重置时间缩放")]
    [SerializeField] private bool resetTimeScale = true;
    
    [Tooltip("是否重置音频监听器")]
    [SerializeField] private bool resetAudioListener = true;
    
    [Tooltip("是否停止所有音频")]
    [SerializeField] private bool stopAllAudio = true;
    
    [Tooltip("是否停止所有协程")]
    [SerializeField] private bool stopAllCoroutines = true;
    
    [Header("UI 反馈")]
    [Tooltip("显示重启提示")]
    [SerializeField] private bool showRestartHint = true;
    
    [Tooltip("显示重启进度")]
    [SerializeField] private bool showRestartProgress = true;
    
    [Header("调试设置")]
    [Tooltip("显示调试日志")]
    [SerializeField] private bool showDebugLogs = true;

    private float holdStartTime;
    private bool isHolding = false;
    private bool isRestarting = false;
    
    // UI 样式
    private GUIStyle hintStyle;
    private GUIStyle progressStyle;
    private bool stylesInitialized = false;

    private void Update()
    {
        // 只在运行时工作，编辑器模式和构建时不运行
        if (!Application.isPlaying)
        {
            return;
        }
        
        if (isRestarting) return;
        
        // 检测按键
        if (Input.GetKeyDown(restartKey))
        {
            if (requireHold)
            {
                // 开始长按计时
                isHolding = true;
                holdStartTime = Time.realtimeSinceStartup;
                Log("开始长按重启...");
            }
            else
            {
                // 直接重启
                StartRestartSequence();
            }
        }
        
        // 长按模式
        if (requireHold && isHolding)
        {
            if (Input.GetKey(restartKey))
            {
                float holdTime = Time.realtimeSinceStartup - holdStartTime;
                
                if (holdTime >= holdDuration)
                {
                    // 长按时间足够，触发重启
                    isHolding = false;
                    StartRestartSequence();
                }
            }
            else
            {
                // 松开按键，取消重启
                isHolding = false;
                Log("取消重启");
            }
        }
    }

    /// <summary>
    /// 开始重启序列
    /// </summary>
    private void StartRestartSequence()
    {
        if (isRestarting) return;
        
        isRestarting = true;
        Log("开始重启游戏...");
        
        StartCoroutine(RestartGameSequence());
    }

    /// <summary>
    /// 重启游戏序列
    /// </summary>
    private IEnumerator RestartGameSequence()
    {
        // 1. 暂停游戏
        if (resetTimeScale)
        {
            Time.timeScale = 0f;
        }
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        // 2. 停止所有音频
        if (stopAllAudio)
        {
            StopAllAudioSources();
        }
        
        // 3. 停止所有协程
        if (stopAllCoroutines)
        {
            StopAllActiveCoroutines();
        }
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        // 4. 重置状态
        ResetGameState();
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        // 5. 清理内存
        if (cleanupMemoryBeforeRestart)
        {
            yield return StartCoroutine(CleanupMemory());
        }
        
        yield return new WaitForSecondsRealtime(0.2f);
        
        // 6. 恢复时间缩放
        if (resetTimeScale)
        {
            Time.timeScale = 1f;
        }
        
        // 7. 重新加载场景
        Log("重新加载场景...");
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    /// <summary>
    /// 停止所有音频源
    /// </summary>
    private void StopAllAudioSources()
    {
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        int stopped = 0;
        
        foreach (AudioSource source in audioSources)
        {
            if (source != null && source.isPlaying)
            {
                source.Stop();
                stopped++;
            }
        }
        
        // 重置音频监听器
        if (resetAudioListener)
        {
            AudioListener listener = FindAnyObjectByType<AudioListener>();
            if (listener != null)
            {
                listener.enabled = false;
                listener.enabled = true;
            }
        }
        
        Log($"停止了 {stopped} 个音频源");
    }

    /// <summary>
    /// 停止所有活动的协程
    /// </summary>
    private void StopAllActiveCoroutines()
    {
        MonoBehaviour[] allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        
        foreach (MonoBehaviour behaviour in allBehaviours)
        {
            if (behaviour != null && behaviour != this)
            {
                behaviour.StopAllCoroutines();
            }
        }
        
        Log("停止了所有协程");
    }

    /// <summary>
    /// 重置游戏状态
    /// </summary>
    private void ResetGameState()
    {
        // 重置时间缩放
        if (resetTimeScale)
        {
            Time.timeScale = 1f;
        }
        
        // 重置物理状态
        Physics.SyncTransforms();
        if (Physics2D.simulationMode == SimulationMode2D.FixedUpdate)
        {
            Physics2D.SyncTransforms();
        }
        
        // 清除输入缓冲
        Input.ResetInputAxes();
        
        Log("游戏状态已重置");
    }

    /// <summary>
    /// 清理内存
    /// </summary>
    private IEnumerator CleanupMemory()
    {
        Log("开始清理内存...");
        
        // 清理材质实例
        if (MemoryOptimizer.Instance != null)
        {
            MemoryOptimizer.Instance.PerformMemoryCleanup();
        }
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        // 清理 Job System
        JobSystemMemoryFix.ForceCleanup();
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        // 卸载未使用资源
        if (unloadUnusedAssets)
        {
            AsyncOperation unloadOp = Resources.UnloadUnusedAssets();
            while (!unloadOp.isDone)
            {
                yield return null;
            }
            Log("未使用资源已卸载");
        }
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        // 执行垃圾回收
        if (performGarbageCollection)
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            Log("垃圾回收已完成");
        }
        
        Log("内存清理完成");
    }

    /// <summary>
    /// 手动触发重启（可从其他脚本调用）
    /// </summary>
    public void ManualRestart()
    {
        if (!isRestarting)
        {
            StartRestartSequence();
        }
    }

    /// <summary>
    /// 快速重启（跳过清理）
    /// </summary>
    public void QuickRestart()
    {
        Log("快速重启游戏...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        InitializeStyles();
        
        // 显示重启提示
        if (showRestartHint && !isRestarting)
        {
            string hintText = requireHold 
                ? $"长按 [{restartKey}] {holdDuration:F1}秒 重启游戏"
                : $"按 [{restartKey}] 重启游戏";
            
            GUI.Label(new Rect(Screen.width - 250, Screen.height - 40, 240, 30), 
                hintText, hintStyle);
        }
        
        // 显示长按进度
        if (showRestartProgress && requireHold && isHolding)
        {
            float progress = (Time.realtimeSinceStartup - holdStartTime) / holdDuration;
            progress = Mathf.Clamp01(progress);
            
            // 绘制进度条
            float barWidth = 200f;
            float barHeight = 30f;
            float barX = (Screen.width - barWidth) / 2f;
            float barY = Screen.height / 2f - 50f;
            
            // 背景
            GUI.Box(new Rect(barX - 5, barY - 5, barWidth + 10, barHeight + 10), "");
            
            // 进度
            GUI.Box(new Rect(barX, barY, barWidth * progress, barHeight), "");
            
            // 文字
            GUI.Label(new Rect(barX, barY, barWidth, barHeight), 
                $"重启进度: {progress * 100f:F0}%", progressStyle);
        }
        
        // 显示重启中提示
        if (isRestarting)
        {
            GUI.Label(new Rect(Screen.width / 2f - 100f, Screen.height / 2f, 200f, 40f), 
                "正在重启游戏...", progressStyle);
        }
    }

    private void InitializeStyles()
    {
        if (stylesInitialized) return;
        
        hintStyle = new GUIStyle();
        hintStyle.fontSize = 14;
        hintStyle.normal.textColor = new Color(1f, 1f, 1f, 0.7f);
        hintStyle.alignment = TextAnchor.MiddleRight;
        hintStyle.padding = new RectOffset(5, 5, 5, 5);
        
        progressStyle = new GUIStyle();
        progressStyle.fontSize = 16;
        progressStyle.normal.textColor = Color.white;
        progressStyle.alignment = TextAnchor.MiddleCenter;
        progressStyle.fontStyle = FontStyle.Bold;
        
        stylesInitialized = true;
    }

    private void Log(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[重启管理器] {message}");
        }
    }

    private void OnDestroy()
    {
        // 确保时间缩放恢复正常
        Time.timeScale = 1f;
    }
}

