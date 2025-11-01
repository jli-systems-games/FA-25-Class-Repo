using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// 命令行构建脚本
/// </summary>
public class CommandLineBuild
{
    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        PerformBuild();
    }

    public static void PerformBuild()
    {
        Debug.Log("[CommandLineBuild] 开始 WebGL 构建...");
        
        string buildPath = "/Users/muyimoi/Documents/GitHub/Project/WebGLBuild";
        
        // 构建选项
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Flight.unity" },
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };
        
        Debug.Log($"[CommandLineBuild] 构建路径: {buildPath}");
        Debug.Log($"[CommandLineBuild] 场景: Assets/Scenes/Flight.unity");
        
        // 执行构建
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;
        
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[CommandLineBuild] ✅ 构建成功！");
            Debug.Log($"[CommandLineBuild] 总时间: {summary.totalTime}");
            Debug.Log($"[CommandLineBuild] 大小: {summary.totalSize} bytes");
        }
        else
        {
            Debug.LogError($"[CommandLineBuild] ❌ 构建失败: {summary.result}");
            
            // 输出详细错误
            foreach (BuildStep step in report.steps)
            {
                foreach (BuildStepMessage message in step.messages)
                {
                    if (message.type == LogType.Error || message.type == LogType.Exception)
                    {
                        Debug.LogError($"[CommandLineBuild] 错误: {message.content}");
                    }
                }
            }
        }
    }

    [MenuItem("Build/Build macOS")]
    public static void BuildMacOS()
    {
        PerformMacOSBuild();
    }

    public static void PerformMacOSBuild()
    {
        Debug.Log("[CommandLineBuild] 开始 macOS 构建...");
        
        string buildPath = "/Users/muyimoi/Downloads/SpaceGame.app";
        
        // 构建选项
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Flight.unity" },
            locationPathName = buildPath,
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.None
        };
        
        Debug.Log($"[CommandLineBuild] 构建路径: {buildPath}");
        Debug.Log($"[CommandLineBuild] 场景: Assets/Scenes/Flight.unity");
        Debug.Log($"[CommandLineBuild] 平台: macOS");
        
        // 执行构建
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;
        
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[CommandLineBuild] ✅ macOS 构建成功！");
            Debug.Log($"[CommandLineBuild] 总时间: {summary.totalTime}");
            Debug.Log($"[CommandLineBuild] 大小: {summary.totalSize} bytes");
            Debug.Log($"[CommandLineBuild] 输出: {buildPath}");
        }
        else
        {
            Debug.LogError($"[CommandLineBuild] ❌ 构建失败: {summary.result}");
            
            // 输出详细错误
            foreach (BuildStep step in report.steps)
            {
                foreach (BuildStepMessage message in step.messages)
                {
                    if (message.type == LogType.Error || message.type == LogType.Exception)
                    {
                        Debug.LogError($"[CommandLineBuild] 错误: {message.content}");
                    }
                }
            }
        }
    }
}

