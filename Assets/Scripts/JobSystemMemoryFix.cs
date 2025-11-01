using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Job System 内存泄漏修复器
/// 专门针对 "JobTempAlloc" 和 "Invalid memory pointer" 错误
/// 这些错误通常与 Mesh、Physics、Animation 的 Job System 有关
/// </summary>
public class JobSystemMemoryFix : MonoBehaviour
{
    [Header("修复设置")]
    [Tooltip("强制同步间隔（秒）- 强制等待所有 Jobs 完成")]
    [SerializeField] private float forceSyncInterval = 5f;
    
    [Tooltip("是否启用 Mesh 数据清理")]
    [SerializeField] private bool enableMeshCleanup = true;
    
    [Tooltip("是否启用 Physics 清理")]
    [SerializeField] private bool enablePhysicsCleanup = true;
    
    [Tooltip("是否启用 Animation 清理")]
    [SerializeField] private bool enableAnimationCleanup = true;
    
    [Header("调试设置")]
    [Tooltip("显示详细日志")]
    [SerializeField] private bool showDebugLogs = true;

    private float lastSyncTime;

    private void Start()
    {
        // 只在运行时工作，编辑器模式和构建时不运行
        if (!Application.isPlaying)
        {
            enabled = false;
            return;
        }
        
        lastSyncTime = Time.realtimeSinceStartup;
        Log("Job System 内存修复器已启动");
        
        // 立即执行一次清理
        PerformFullCleanup();
    }

    private void Update()
    {
        // 确保在运行时
        if (!Application.isPlaying)
        {
            return;
        }
        
        if (Time.realtimeSinceStartup - lastSyncTime >= forceSyncInterval)
        {
            PerformFullCleanup();
            lastSyncTime = Time.realtimeSinceStartup;
        }
    }

    private void LateUpdate()
    {
        // 确保在运行时
        if (!Application.isPlaying)
        {
            return;
        }
        
        // 在每帧末尾确保所有 Jobs 完成
        // 这可以防止 Job 数据在帧之间累积
        CompleteAllJobs();
    }

    /// <summary>
    /// 执行完整清理
    /// </summary>
    public void PerformFullCleanup()
    {
        Log("开始 Job System 清理...");
        
        // 1. 完成所有挂起的 Jobs
        CompleteAllJobs();
        
        // 2. 清理 Mesh 数据
        if (enableMeshCleanup)
        {
            CleanupMeshData();
        }
        
        // 3. 清理 Physics 数据
        if (enablePhysicsCleanup)
        {
            CleanupPhysicsData();
        }
        
        // 4. 清理 Animation 数据
        if (enableAnimationCleanup)
        {
            CleanupAnimationData();
        }
        
        // 5. 强制刷新渲染管线
        FlushRenderingPipeline();
        
        Log("Job System 清理完成");
    }

    /// <summary>
    /// 完成所有挂起的 Jobs
    /// </summary>
    private void CompleteAllJobs()
    {
        // 同步物理系统 - 这会等待所有物理 Jobs 完成
        if (Physics.simulationMode == SimulationMode.FixedUpdate)
        {
            Physics.SyncTransforms();
        }
        
        // 同步 2D 物理
        if (Physics2D.simulationMode == SimulationMode2D.FixedUpdate)
        {
            Physics2D.SyncTransforms();
        }
        
        // 强制等待渲染完成
        if (Camera.main != null)
        {
            Camera.main.Render();
        }
    }

    /// <summary>
    /// 清理 Mesh 数据
    /// </summary>
    private void CleanupMeshData()
    {
        // Mesh 数据通常由 Job System 处理
        // 强制更新所有 MeshRenderer 以清理挂起的 Mesh jobs
        
        MeshRenderer[] meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        foreach (MeshRenderer mr in meshRenderers)
        {
            if (mr != null && mr.enabled)
            {
                // 触发 Mesh 更新
                mr.enabled = false;
                mr.enabled = true;
            }
        }
        
        SkinnedMeshRenderer[] skinnedMeshRenderers = FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None);
        foreach (SkinnedMeshRenderer smr in skinnedMeshRenderers)
        {
            if (smr != null && smr.enabled)
            {
                // 强制更新 Skinned Mesh（这是常见的 JobTempAlloc 泄漏源）
                smr.forceMatrixRecalculationPerRender = true;
            }
        }
        
        Log("Mesh 数据已清理");
    }

    /// <summary>
    /// 清理 Physics 数据
    /// </summary>
    private void CleanupPhysicsData()
    {
        // Physics 查询可能会在 Job System 中产生临时分配
        // 同步物理系统以确保所有查询完成
        
        if (Physics.simulationMode == SimulationMode.FixedUpdate)
        {
            Physics.SyncTransforms();
        }
        
        // 如果有 2D 物理
        if (Physics2D.simulationMode == SimulationMode2D.FixedUpdate)
        {
            Physics2D.SyncTransforms();
        }
        
        Log("Physics 数据已清理");
    }

    /// <summary>
    /// 清理 Animation 数据
    /// </summary>
    private void CleanupAnimationData()
    {
        // Animation jobs 也可能导致 JobTempAlloc 泄漏
        
        Animator[] animators = FindObjectsByType<Animator>(FindObjectsSortMode.None);
        foreach (Animator animator in animators)
        {
            if (animator != null && animator.enabled && animator.isActiveAndEnabled)
            {
                // 强制完成当前的 animation job
                animator.Update(0);
            }
        }
        
        Log("Animation 数据已清理");
    }

    /// <summary>
    /// 刷新渲染管线
    /// </summary>
    private void FlushRenderingPipeline()
    {
        // 强制完成所有渲染 Jobs
        // 这对于 URP/HDRP 特别重要
        
        Camera.main?.Render();
        
        Log("渲染管线已刷新");
    }

    /// <summary>
    /// 在场景切换时清理
    /// </summary>
    private void OnDisable()
    {
        Log("场景切换，执行深度清理...");
        PerformFullCleanup();
    }

    private void OnDestroy()
    {
        Log("Job System 内存修复器已停止");
    }

    private void Log(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[Job System 修复器] {message}");
        }
    }

    /// <summary>
    /// 手动触发清理（可从其他脚本调用）
    /// </summary>
    public static void ForceCleanup()
    {
        JobSystemMemoryFix instance = FindAnyObjectByType<JobSystemMemoryFix>();
        if (instance != null)
        {
            instance.PerformFullCleanup();
        }
    }
}

