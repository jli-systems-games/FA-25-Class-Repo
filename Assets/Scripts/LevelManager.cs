using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 关卡目标类型
/// </summary>
public enum LevelGoalType
{
    SurviveDuration,        // 存活指定时间
    MaintainPlantCount,     // 维持植物数量
    ReachFertilityLevel,    // 达到肥力水平
    GrowSpecificPlant       // 种植特定数量的某种植物
}

/// <summary>
/// 关卡目标数据
/// </summary>
[System.Serializable]
public class LevelGoal
{
    public LevelGoalType goalType;
    public float targetValue;           // 目标数值（时间/数量/肥力）
    public PlantData specificPlant;     // 特定植物（仅用于GrowSpecificPlant类型）
    public string description;          // 目标描述
    
    [HideInInspector]
    public float currentProgress;       // 当前进度
    [HideInInspector]
    public bool isCompleted;            // 是否完成
}

/// <summary>
/// 关卡管理器
/// 处理关卡目标、胜利/失败条件
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("关卡目标")]
    [SerializeField] private List<LevelGoal> levelGoals = new List<LevelGoal>();
    
    [Header("UI引用")]
    [SerializeField] private Text goalDisplayText;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    
    [Header("关卡状态")]
    private float levelStartTime;
    private bool isLevelActive = true;
    private bool isVictorious = false;
    
    private void Start()
    {
        levelStartTime = Time.time;
        
        // 初始化目标
        foreach (var goal in levelGoals)
        {
            goal.currentProgress = 0f;
            goal.isCompleted = false;
        }
        
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
    }
    
    private void Update()
    {
        if (!isLevelActive) return;
        
        // 更新所有目标进度
        UpdateGoalProgress();
        
        // 检查是否完成所有目标
        CheckVictoryCondition();
        
        // 更新UI显示
        UpdateGoalUI();
    }
    
    /// <summary>
    /// 更新目标进度
    /// </summary>
    private void UpdateGoalProgress()
    {
        foreach (var goal in levelGoals)
        {
            if (goal.isCompleted) continue;
            
            switch (goal.goalType)
            {
                case LevelGoalType.SurviveDuration:
                    // 存活时间
                    goal.currentProgress = Time.time - levelStartTime;
                    if (goal.currentProgress >= goal.targetValue)
                    {
                        goal.isCompleted = true;
                    }
                    break;
                    
                case LevelGoalType.MaintainPlantCount:
                    // 维持植物数量
                    List<Plant> allPlants = EcosystemManager.Instance.GetAllPlants();
                    goal.currentProgress = allPlants.Count;
                    // 这个目标需要持续维持，不设置isCompleted
                    break;
                    
                case LevelGoalType.ReachFertilityLevel:
                    // 达到肥力水平
                    goal.currentProgress = EcosystemManager.Instance.CurrentFertility;
                    if (goal.currentProgress >= goal.targetValue)
                    {
                        goal.isCompleted = true;
                    }
                    break;
                    
                case LevelGoalType.GrowSpecificPlant:
                    // 种植特定植物（需要统计）
                    int count = CountSpecificPlant(goal.specificPlant);
                    goal.currentProgress = count;
                    if (goal.currentProgress >= goal.targetValue)
                    {
                        goal.isCompleted = true;
                    }
                    break;
            }
        }
    }
    
    /// <summary>
    /// 统计特定植物数量
    /// </summary>
    private int CountSpecificPlant(PlantData targetPlant)
    {
        if (targetPlant == null) return 0;
        
        int count = 0;
        List<Plant> allPlants = EcosystemManager.Instance.GetAllPlants();
        
        foreach (Plant plant in allPlants)
        {
            // 通过反射获取植物的PlantData（因为是私有字段）
            var field = typeof(Plant).GetField("plantData", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            PlantData plantData = field?.GetValue(plant) as PlantData;
            
            if (plantData == targetPlant)
            {
                count++;
            }
        }
        
        return count;
    }
    
    /// <summary>
    /// 检查胜利条件
    /// </summary>
    private void CheckVictoryCondition()
    {
        // 所有目标都完成
        bool allCompleted = true;
        foreach (var goal in levelGoals)
        {
            if (!goal.isCompleted)
            {
                allCompleted = false;
                break;
            }
        }
        
        if (allCompleted && !isVictorious)
        {
            OnVictory();
        }
        
        // 检查失败条件（例如肥力归零且无植物）
        if (EcosystemManager.Instance.CurrentFertility <= 0 &&
            EcosystemManager.Instance.GetAllPlants().Count == 0)
        {
            OnDefeat();
        }
    }
    
    /// <summary>
    /// 胜利
    /// </summary>
    private void OnVictory()
    {
        isLevelActive = false;
        isVictorious = true;
        
        Debug.Log("[LevelManager] 关卡胜利！");
        
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }
    
    /// <summary>
    /// 失败
    /// </summary>
    private void OnDefeat()
    {
        isLevelActive = false;
        
        Debug.Log("[LevelManager] 关卡失败！");
        
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }
    }
    
    /// <summary>
    /// 更新目标UI显示
    /// </summary>
    private void UpdateGoalUI()
    {
        if (goalDisplayText == null) return;
        
        string displayText = "关卡目标:\n";
        
        foreach (var goal in levelGoals)
        {
            string status = goal.isCompleted ? "[✓]" : "[ ]";
            string progress = "";
            
            switch (goal.goalType)
            {
                case LevelGoalType.SurviveDuration:
                    progress = $"{goal.currentProgress:F0}s / {goal.targetValue:F0}s";
                    break;
                case LevelGoalType.MaintainPlantCount:
                    progress = $"{goal.currentProgress:F0} / {goal.targetValue:F0}";
                    break;
                case LevelGoalType.ReachFertilityLevel:
                    progress = $"{goal.currentProgress:F0} / {goal.targetValue:F0}";
                    break;
                case LevelGoalType.GrowSpecificPlant:
                    progress = $"{goal.currentProgress:F0} / {goal.targetValue:F0}";
                    break;
            }
            
            displayText += $"{status} {goal.description} ({progress})\n";
        }
        
        goalDisplayText.text = displayText;
    }
    
    /// <summary>
    /// 重新开始关卡
    /// </summary>
    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
