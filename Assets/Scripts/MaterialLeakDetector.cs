using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 材质泄漏检测器 - 主动检测和清理泄漏的材质实例
/// 这个脚本专门针对 Unity 中常见的材质泄漏问题
/// </summary>
public class MaterialLeakDetector : MonoBehaviour
{
    [Header("检测设置")]
    [Tooltip("检测间隔（秒）- 推荐 30-60 秒")]
    [SerializeField] private float detectionInterval = 45f;
    
    [Tooltip("是否自动清理检测到的泄漏")]
    [SerializeField] private bool autoCleanup = false;
    
    [Tooltip("材质实例名称包含的标识（表示是副本）")]
    [SerializeField] private string instanceIdentifier = "(Instance)";
    
    [Tooltip("轻量级检测（性能优先）")]
    [SerializeField] private bool lightweightDetection = true;
    
    [Header("阈值设置")]
    [Tooltip("材质实例数量警告阈值")]
    [SerializeField] private int warningThreshold = 100;
    
    [Tooltip("材质实例数量危险阈值")]
    [SerializeField] private int dangerThreshold = 200;
    
    [Header("调试设置")]
    [Tooltip("显示详细日志")]
    [SerializeField] private bool verboseLogging = true;
    
    [Tooltip("显示统计信息")]
    [SerializeField] private bool showStats = true;

    private float lastDetectionTime;
    private int totalInstancesDetected = 0;
    private int totalInstancesCleaned = 0;
    private Dictionary<string, int> materialInstanceCounts = new Dictionary<string, int>();

    private void Start()
    {
        // 只在运行时工作，编辑器模式和构建时不运行
        if (!Application.isPlaying)
        {
            enabled = false;
            return;
        }
        
        lastDetectionTime = Time.realtimeSinceStartup;
        Log("材质泄漏检测器已启动");
    }

    private void Update()
    {
        // 确保在运行时
        if (!Application.isPlaying)
        {
            return;
        }
        
        if (Time.realtimeSinceStartup - lastDetectionTime >= detectionInterval)
        {
            DetectAndCleanupLeaks();
            lastDetectionTime = Time.realtimeSinceStartup;
        }
    }

    /// <summary>
    /// 检测并清理材质泄漏
    /// </summary>
    public void DetectAndCleanupLeaks()
    {
        Log("开始检测材质泄漏...");
        
        // 收集所有材质实例
        Dictionary<string, List<Material>> materialGroups = CollectMaterialInstances();
        
        // 分析和报告
        AnalyzeMaterials(materialGroups);
        
        // 清理泄漏的材质
        if (autoCleanup)
        {
            CleanupLeakedMaterials(materialGroups);
        }
        
        Log($"检测完成。总计: {totalInstancesDetected} 个实例，已清理: {totalInstancesCleaned} 个");
    }

    /// <summary>
    /// 收集所有材质实例（优化版）
    /// </summary>
    private Dictionary<string, List<Material>> CollectMaterialInstances()
    {
        Dictionary<string, List<Material>> groups = new Dictionary<string, List<Material>>();
        
        // 轻量级模式：只检查激活的 Renderer
        Renderer[] renderers = lightweightDetection 
            ? FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None) 
            : FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null) continue;
            
            // 检查 sharedMaterials（不会创建副本）
            Material[] materials = renderer.sharedMaterials;
            
            foreach (Material mat in materials)
            {
                if (mat == null) continue;
                
                // 只关注实例化的材质
                if (mat.name.Contains(instanceIdentifier))
                {
                    string baseName = GetBaseMaterialName(mat.name);
                    
                    if (!groups.ContainsKey(baseName))
                    {
                        groups[baseName] = new List<Material>();
                    }
                    
                    groups[baseName].Add(mat);
                }
            }
        }
        
        // 轻量级模式下跳过其他检查（LineRenderer, TrailRenderer 等）
        if (!lightweightDetection)
        {
            // 从 LineRenderer 收集
            LineRenderer[] lineRenderers = FindObjectsByType<LineRenderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (LineRenderer lr in lineRenderers)
            {
                if (lr == null) continue;
                Material mat = lr.sharedMaterial;
                if (mat != null && mat.name.Contains(instanceIdentifier))
                {
                    string baseName = GetBaseMaterialName(mat.name);
                    if (!groups.ContainsKey(baseName))
                    {
                        groups[baseName] = new List<Material>();
                    }
                    groups[baseName].Add(mat);
                }
            }
            
            // 从 TrailRenderer 收集
            TrailRenderer[] trailRenderers = FindObjectsByType<TrailRenderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (TrailRenderer tr in trailRenderers)
            {
                if (tr == null) continue;
                Material mat = tr.sharedMaterial;
                if (mat != null && mat.name.Contains(instanceIdentifier))
                {
                    string baseName = GetBaseMaterialName(mat.name);
                    if (!groups.ContainsKey(baseName))
                    {
                        groups[baseName] = new List<Material>();
                    }
                    groups[baseName].Add(mat);
                }
            }
        }
        
        return groups;
    }

    /// <summary>
    /// 分析材质使用情况
    /// </summary>
    private void AnalyzeMaterials(Dictionary<string, List<Material>> materialGroups)
    {
        totalInstancesDetected = 0;
        materialInstanceCounts.Clear();
        
        foreach (var group in materialGroups)
        {
            int count = group.Value.Count;
            totalInstancesDetected += count;
            materialInstanceCounts[group.Key] = count;
            
            // 警告过多的实例
            if (count >= dangerThreshold)
            {
                LogWarning($"⚠️ 危险！材质 '{group.Key}' 有 {count} 个实例（可能泄漏）");
            }
            else if (count >= warningThreshold)
            {
                LogWarning($"⚠️ 警告：材质 '{group.Key}' 有 {count} 个实例");
            }
            else if (verboseLogging)
            {
                Log($"材质 '{group.Key}': {count} 个实例");
            }
        }
    }

    /// <summary>
    /// 清理泄漏的材质
    /// </summary>
    private void CleanupLeakedMaterials(Dictionary<string, List<Material>> materialGroups)
    {
        int cleaned = 0;
        
        foreach (var group in materialGroups)
        {
            // 如果某个材质有过多实例，可能存在泄漏
            if (group.Value.Count >= warningThreshold)
            {
                // 查找哪些材质实例没有被引用
                List<Material> unusedMaterials = FindUnusedMaterials(group.Value);
                
                foreach (Material mat in unusedMaterials)
                {
                    if (mat != null)
                    {
                        Destroy(mat);
                        cleaned++;
                    }
                }
                
                if (unusedMaterials.Count > 0)
                {
                    Log($"清理了 '{group.Key}' 的 {unusedMaterials.Count} 个未使用实例");
                }
            }
        }
        
        totalInstancesCleaned += cleaned;
        
        if (cleaned > 0)
        {
            // 清理后执行垃圾回收
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }

    /// <summary>
    /// 查找未使用的材质（没有被任何激活的 Renderer 引用）
    /// </summary>
    private List<Material> FindUnusedMaterials(List<Material> materials)
    {
        List<Material> unused = new List<Material>();
        
        // 获取所有激活的 Renderer 的材质
        HashSet<Material> usedMaterials = new HashSet<Material>();
        
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        foreach (Renderer r in renderers)
        {
            if (r != null && r.gameObject.activeInHierarchy)
            {
                foreach (Material mat in r.sharedMaterials)
                {
                    if (mat != null)
                    {
                        usedMaterials.Add(mat);
                    }
                }
            }
        }
        
        // 检查哪些材质没有被使用
        foreach (Material mat in materials)
        {
            if (mat != null && !usedMaterials.Contains(mat))
            {
                unused.Add(mat);
            }
        }
        
        return unused;
    }

    /// <summary>
    /// 获取材质的基础名称（去掉实例标识）
    /// </summary>
    private string GetBaseMaterialName(string materialName)
    {
        int index = materialName.IndexOf(instanceIdentifier);
        if (index > 0)
        {
            return materialName.Substring(0, index).Trim();
        }
        return materialName;
    }

    /// <summary>
    /// 强制清理所有材质实例
    /// </summary>
    public void ForceCleanupAllInstances()
    {
        LogWarning("强制清理所有材质实例...");
        
        int cleaned = 0;
        
        // 查找所有材质资源
        Material[] allMaterials = Resources.FindObjectsOfTypeAll<Material>();
        
        foreach (Material mat in allMaterials)
        {
            if (mat != null && mat.name.Contains(instanceIdentifier))
            {
                // 检查是否有引用
                Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
                bool inUse = false;
                
                foreach (Renderer r in renderers)
                {
                    if (r != null && r.gameObject.activeInHierarchy)
                    {
                        if (System.Array.Exists(r.sharedMaterials, m => m == mat))
                        {
                            inUse = true;
                            break;
                        }
                    }
                }
                
                if (!inUse)
                {
                    Destroy(mat);
                    cleaned++;
                }
            }
        }
        
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        
        Log($"强制清理完成，清理了 {cleaned} 个材质实例");
    }

    private void Log(string message)
    {
        if (verboseLogging)
        {
            Debug.Log($"[材质泄漏检测器] {message}");
        }
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"[材质泄漏检测器] {message}");
    }

    private void OnGUI()
    {
        if (!showStats || !Application.isPlaying) return;
        
        GUIStyle style = new GUIStyle();
        style.fontSize = 11;
        style.normal.textColor = Color.yellow;
        style.padding = new RectOffset(10, 10, 10, 10);
        
        string statsText = $"=== 材质泄漏统计 ===\n" +
                          $"检测到实例: {totalInstancesDetected}\n" +
                          $"已清理: {totalInstancesCleaned}\n" +
                          $"下次检测: {Mathf.Max(0, detectionInterval - (Time.realtimeSinceStartup - lastDetectionTime)):F0}秒\n\n";
        
        // 显示前 5 个材质的实例数
        var topMaterials = materialInstanceCounts.OrderByDescending(x => x.Value).Take(5);
        statsText += "实例最多的材质:\n";
        foreach (var mat in topMaterials)
        {
            statsText += $"  {mat.Key}: {mat.Value}\n";
        }
        
        GUI.Label(new Rect(10, 90, 400, 200), statsText, style);
    }
}

