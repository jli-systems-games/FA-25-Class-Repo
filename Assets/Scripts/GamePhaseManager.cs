using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GamePhase
{
    PreGame,      // 游戏开始前，玩家部署武器
    Playing,      // 游戏进行中（5波）
    RestPhase     // 中场休息
}

public class GamePhaseManager : MonoBehaviour {

    public static GamePhaseManager Instance;

    public GamePhase currentPhase = GamePhase.PreGame;
    
    // 是否是困难模式
    public bool isHardMode = false;
    
    // 当前阶段编号（每5波为一个阶段）
    public int currentStage = 0;
    
    // 记录在困难模式下每个阶段部署的武器（用于锁定）
    private Dictionary<int, List<Node>> stageDeployedTurrets = new Dictionary<int, List<Node>>();
    
    // UI引用
    public GameObject startGameButton;
    public GameObject restPhaseUI;
    public GameObject shopUI;  // 底部Shop UI（只在部署阶段显示）
    
    private GameManager gameManager;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        SetPhase(GamePhase.PreGame);
    }

    // 设置游戏阶段
    public void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        
        switch (newPhase)
        {
            case GamePhase.PreGame:
                ShowPreGameUI();
                break;
                
            case GamePhase.Playing:
                HideAllPhaseUI();
                // 更新阶段信息显示（Phase X of Y）
                if (WaveSpawner.Instance != null)
                {
                    WaveSpawner.Instance.UpdatePhaseInfo();
                }
                break;
                
            case GamePhase.RestPhase:
                ShowRestPhaseUI();
                break;
        }
    }

    // 显示游戏开始前UI
    void ShowPreGameUI()
    {
        if (startGameButton != null)
            startGameButton.SetActive(true);
        
        // 显示Shop UI（可以部署武器）
        if (shopUI != null)
            shopUI.SetActive(true);
        
        // PreGame阶段不显示Rest Phase UI
        if (restPhaseUI != null)
            restPhaseUI.SetActive(false);
        
        // 显示初始提示信息
        if (gameManager != null)
            gameManager.UpdatePhaseInfo("Place Weapon to Protect HQ");
        
        // 播放PreGame音乐
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayPreGameMusic();
    }

    // 显示休息阶段UI
    void ShowRestPhaseUI()
    {
        if (restPhaseUI != null)
            restPhaseUI.SetActive(true);
            
        if (startGameButton != null)
            startGameButton.SetActive(true);
        
        // 显示Shop UI（可以部署/拆除武器）
        if (shopUI != null)
            shopUI.SetActive(true);
        
        // 更新休息时间文本
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.UpdateRestInfo();
        
        // 播放休息音乐
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayRestPhaseMusic();
    }

    // 隐藏所有阶段UI（游戏进行中）
    void HideAllPhaseUI()
    {
        if (startGameButton != null)
            startGameButton.SetActive(false);
        
        // Playing阶段隐藏Rest Phase UI
        if (restPhaseUI != null)
            restPhaseUI.SetActive(false);
        
        // 隐藏Shop UI（游戏进行中不能操作）
        if (shopUI != null)
            shopUI.SetActive(false);
    }

    // 检查是否可以开始游戏
    public bool CanStartGame()
    {
        // 检查场上至少有一个炮塔
        Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        foreach (Node node in nodes)
        {
            if (node.turret != null)
                return true;
        }
        return false;
    }

    // 玩家点击开始按钮
    public void OnStartButtonClicked()
    {
        if (currentPhase == GamePhase.PreGame)
        {
            // 游戏开始前检查是否至少有一个炮塔
            if (!CanStartGame())
            {
                return;
            }
            
            // 记录初始部署的武器（困难模式用）
            if (isHardMode)
            {
                RecordCurrentStageTurrets();
            }
            
            SetPhase(GamePhase.Playing);
            WaveSpawner.Instance?.StartNextStage();
        }
        else if (currentPhase == GamePhase.RestPhase)
        {
            // 休息阶段：进入下一阶段
            // 不需要检查是否有炮塔，因为之前肯定已经有了
            currentStage++;
            
            // 记录本阶段新部署的武器（困难模式用）
            if (isHardMode)
            {
                RecordCurrentStageTurrets();
            }
            
            SetPhase(GamePhase.Playing);
            WaveSpawner.Instance?.StartNextStage();
        }
    }

    // 记录当前阶段部署的所有武器（困难模式用）
    void RecordCurrentStageTurrets()
    {
        List<Node> currentTurrets = new List<Node>();
        Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        
        foreach (Node node in nodes)
        {
            if (node.turret != null)
            {
                currentTurrets.Add(node);
            }
        }
        
        stageDeployedTurrets[currentStage] = currentTurrets;
    }

    // 检查一个节点上的武器是否可以被删除（困难模式检查）
    public bool CanRemoveTurret(Node node)
    {
        if (!isHardMode)
            return true;  // 普通模式总是可以删除
        
        if (currentPhase == GamePhase.PreGame)
            return true;  // 游戏开始前总是可以删除
        
        // 检查这个武器是否是在本阶段部署的
        if (stageDeployedTurrets.ContainsKey(currentStage))
        {
            return stageDeployedTurrets[currentStage].Contains(node);
        }
        
        return false;
    }

    // 完成一个阶段（5波完成）
    public void OnStageCompleted()
    {
        // 给予技能点（每完成一个阶段奖励2点）
        PlayerStats.AddSkillPoints(2);
        
        SetPhase(GamePhase.RestPhase);
    }

    // 游戏是否正在进行中
    public bool IsPlaying()
    {
        return currentPhase == GamePhase.Playing;
    }

    // 是否可以建造/删除武器
    public bool CanModifyTurrets()
    {
        return currentPhase != GamePhase.Playing;
    }
}

