using System;
using UnityEngine;

/// <summary>
/// 小猫的数据类，存储所有属性
/// </summary>
[System.Serializable]
public class CatData
{
    [Header("基础属性")]
    [Range(0, 100)]
    public float hunger = 100f;         // 饱食度
    
    [Range(0, 100)]
    public float happiness = 100f;      // 心情
    
    [Range(0, 100)]
    public float hygiene = 100f;        // 清洁度
    
    [Header("游戏数据")]
    public int age = 0;                 // 年龄（天数）
    public float playTime = 0f;         // 游戏时长（秒）
    public float survivalTime = 0f;     // 当前生命存活时间（秒）
    
    [Header("衰减速率 - 每秒减少的量")]
    public float hungerDecayRate = 1.5f;         // 饱食度衰减速率（最快）- 约1.1分钟降到0
    public float happinessDecayRate = 0.75f;     // 心情衰减速率（中等）- 约2.2分钟降到0
    public float hygieneDecayRate = 1.0f;        // 清洁度衰减速率（中等偏快）- 约1.7分钟降到0
    
    [Header("危险阈值")]
    public float criticalThreshold = 10f;       // 危险阈值
    public float criticalDuration = 30f;        // 危险状态持续多久会死亡（秒）
    
    // 当前危险状态持续时间
    [NonSerialized]
    public float currentCriticalTime = 0f;
    
    /// <summary>
    /// 检查是否有任何属性处于危险状态
    /// </summary>
    public bool IsInCriticalState()
    {
        return hunger < criticalThreshold || 
               happiness < criticalThreshold || 
               hygiene < criticalThreshold;
    }
    
    /// <summary>
    /// 检查是否应该触发死亡（任何一个值到0就死亡）
    /// </summary>
    public bool ShouldDie()
    {
        return hunger <= 0f || happiness <= 0f || hygiene <= 0f;
    }
    
    /// <summary>
    /// 重置数据（新游戏）
    /// </summary>
    public void Reset()
    {
        hunger = 100f;
        happiness = 100f;
        hygiene = 100f;
        age = 0;
        survivalTime = 0f;
        currentCriticalTime = 0f;
        // playTime不重置，累计游戏总时长
    }
    
    /// <summary>
    /// 获取格式化的游戏时长
    /// </summary>
    public string GetFormattedPlayTime()
    {
        int hours = Mathf.FloorToInt(playTime / 3600f);
        int minutes = Mathf.FloorToInt((playTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        
        if (hours > 0)
        {
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }
        else
        {
            return $"{minutes:00}:{seconds:00}";
        }
    }
    
    /// <summary>
    /// 获取年龄（天数）
    /// </summary>
    public int GetAgeDays()
    {
        return Mathf.FloorToInt(survivalTime / 86400f); // 86400秒 = 1天
    }
}

