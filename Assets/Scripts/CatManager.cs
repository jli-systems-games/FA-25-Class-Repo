using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 管理小猫的各项属性、状态和行为
/// </summary>
public class CatManager : MonoBehaviour
{
    public static CatManager Instance { get; private set; }
    
    [Header("小猫数据")]
    public CatData catData;
    
    [Header("小猫动画")]
    public Animator catAnimator;
    
    [Header("当前状态")]
    public CatState currentState = CatState.Idle;
    
    [Header("游戏控制")]
    public bool gameStarted = false;            // 游戏是否已开始
    public float accelerationRate = 10f;        // 衰减加速率（每15秒翻10倍）
    private float gameStartTime = 0f;           // 游戏开始时间
    
    // 事件：当属性改变时触发
    public event Action OnStatsChanged;
    public event Action OnCatDied;
    
    private bool isDead = false;
    private Coroutine currentActionCoroutine;
    
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
        if (catData == null)
        {
            catData = new CatData();
        }
        
        UpdateAnimationState();
    }
    
    void Update()
    {
        if (isDead)
        {
            // 每10秒提示一次猫咪已死亡
            if (Time.frameCount % 600 == 0)
            {
                Debug.Log("CatManager.Update: 猫咪已死亡 (isDead=true)，不再更新");
            }
            return;
        }
        
        if (!gameStarted)
        {
            // 每5秒输出一次提示，避免日志刷屏
            if (Time.frameCount % 300 == 0)
            {
                Debug.Log($"CatManager.Update: 游戏未开始 (gameStarted={gameStarted})，等待StartGame()调用");
            }
            return;
        }
        
        float deltaTime = Time.deltaTime;
        
        // 更新时间
        catData.survivalTime += deltaTime;
        catData.playTime += deltaTime;
        
        // 更新年龄（每86400秒=1天）
        catData.age = catData.GetAgeDays();
        
        // 属性随时间衰减（带加速）
        DecayStats(deltaTime);
        
        // 检查危险状态
        CheckCriticalState(deltaTime);
        
        // 更新动画状态
        UpdateAnimationState();
    }
    
    /// <summary>
    /// 属性衰减（带加速机制 - 线性增长）
    /// </summary>
    private void DecayStats(float deltaTime)
    {
        // 计算加速倍率（每15秒增加10倍）
        // 0秒: 1x, 15秒: 10x, 30秒: 20x, 45秒: 30x...
        int intervals = Mathf.FloorToInt(catData.survivalTime / 15f);  // 完整的15秒间隔数
        float speedMultiplier = Mathf.Max(1f, intervals * accelerationRate);  // 线性增长
        
        // 三个属性有不同的基础衰减速度
        // 饱食度衰减最快，清洁度次之，心情最慢
        float hungerSpeed = catData.hungerDecayRate * speedMultiplier;
        float happinessSpeed = catData.happinessDecayRate * speedMultiplier;
        float hygieneSpeed = catData.hygieneDecayRate * speedMultiplier;
        
        // 每2秒输出一次衰减信息（120帧 ≈ 2秒）
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"⏱ DecayStats: hunger={catData.hunger:F1}, happiness={catData.happiness:F1}, hygiene={catData.hygiene:F1} | 倍率={speedMultiplier:F2}x");
        }
        
        catData.hunger = Mathf.Max(0, catData.hunger - hungerSpeed * deltaTime);
        catData.happiness = Mathf.Max(0, catData.happiness - happinessSpeed * deltaTime);
        catData.hygiene = Mathf.Max(0, catData.hygiene - hygieneSpeed * deltaTime);
        
        // 触发属性改变事件
        OnStatsChanged?.Invoke();
    }
    
    /// <summary>
    /// 检查危险状态
    /// </summary>
    private void CheckCriticalState(float deltaTime)
    {
        if (catData.IsInCriticalState())
        {
            catData.currentCriticalTime += deltaTime;
            
            // 每5秒提示一次危险状态
            if (Mathf.FloorToInt(catData.currentCriticalTime) % 5 == 0 && Time.frameCount % 300 == 0)
            {
                Debug.LogWarning($"CatManager: 危险状态！hunger={catData.hunger:F1}, happiness={catData.happiness:F1}, hygiene={catData.hygiene:F1}, 已持续{catData.currentCriticalTime:F1}秒");
            }
            
            if (catData.ShouldDie())
            {
                Die();
            }
        }
        else
        {
            catData.currentCriticalTime = 0f;
        }
    }
    
    /// <summary>
    /// 喂食
    /// </summary>
    public void Feed(FoodType foodType)
    {
        if (isDead) return;
        
        float hungerIncrease = 0f;
        float happinessIncrease = 0f;
        
        switch (foodType)
        {
            case FoodType.BasicFood:
                hungerIncrease = 20f;
                happinessIncrease = 5f;
                break;
            case FoodType.PremiumFood:
                hungerIncrease = 35f;
                happinessIncrease = 15f;
                break;
            case FoodType.Snack:
                hungerIncrease = 10f;
                happinessIncrease = 20f;
                break;
        }
        
        catData.hunger = Mathf.Min(100, catData.hunger + hungerIncrease);
        catData.happiness = Mathf.Min(100, catData.happiness + happinessIncrease);
        
        OnStatsChanged?.Invoke();
        PlayAction(CatState.Eating, 2f);
    }
    
    /// <summary>
    /// 玩耍
    /// </summary>
    public void Play(ToyType toyType)
    {
        if (isDead) return;
        
        float happinessIncrease = 0f;
        
        switch (toyType)
        {
            case ToyType.Stick:
                happinessIncrease = 25f;
                break;
            case ToyType.Ball:
                happinessIncrease = 20f;
                break;
        }
        
        catData.happiness = Mathf.Min(100, catData.happiness + happinessIncrease);
        catData.hunger = Mathf.Max(0, catData.hunger - 5f); // 玩耍会消耗一些饱食度
        
        OnStatsChanged?.Invoke();
        PlayAction(CatState.Playing, 3f);
    }
    
    /// <summary>
    /// 清洁
    /// </summary>
    public void Clean(CleanType cleanType)
    {
        if (isDead) return;
        
        float hygieneIncrease = 0f;
        float happinessIncrease = 0f;
        
        switch (cleanType)
        {
            case CleanType.BasicBath:
                hygieneIncrease = 30f;
                happinessIncrease = 5f;
                break;
            case CleanType.PremiumBath:
                hygieneIncrease = 50f;
                happinessIncrease = 15f;
                break;
        }
        
        catData.hygiene = Mathf.Min(100, catData.hygiene + hygieneIncrease);
        catData.happiness = Mathf.Min(100, catData.happiness + happinessIncrease);
        
        OnStatsChanged?.Invoke();
        PlayAction(CatState.Bathing, 3f);
    }
    
    /// <summary>
    /// 播放动作动画
    /// </summary>
    private void PlayAction(CatState state, float duration)
    {
        if (currentActionCoroutine != null)
        {
            StopCoroutine(currentActionCoroutine);
        }
        
        currentActionCoroutine = StartCoroutine(ActionCoroutine(state, duration));
    }
    
    private IEnumerator ActionCoroutine(CatState state, float duration)
    {
        currentState = state;
        UpdateAnimationState();
        
        yield return new WaitForSeconds(duration);
        
        currentState = CatState.Idle;
        UpdateAnimationState();
        currentActionCoroutine = null;
    }
    
    /// <summary>
    /// 更新动画状态
    /// </summary>
    private void UpdateAnimationState()
    {
        if (catAnimator == null) return;
        
        // 根据当前状态设置动画
        CatState stateToShow = currentState;
        
        // 如果不在执行动作，根据属性判断状态
        if (currentState == CatState.Idle)
        {
            if (catData.IsInCriticalState())
            {
                stateToShow = CatState.Sick;
            }
            else if (catData.happiness > 80 && catData.hunger > 70 && catData.hygiene > 70)
            {
                stateToShow = CatState.Happy;
            }
            else if (catData.happiness < 30 || catData.hunger < 30 || catData.hygiene < 30)
            {
                stateToShow = CatState.Sad;
            }
        }
        
        // 设置动画参数
        catAnimator.SetInteger("State", (int)stateToShow);
    }
    
    /// <summary>
    /// 小猫死亡
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        
        Debug.LogWarning($"CatManager: 猫咪死亡！hunger={catData.hunger:F1}, happiness={catData.happiness:F1}, hygiene={catData.hygiene:F1}, criticalTime={catData.currentCriticalTime:F1}秒");
        
        isDead = true;
        currentState = CatState.Angel;
        UpdateAnimationState();
        
        OnCatDied?.Invoke();
    }
    
    /// <summary>
    /// 重置游戏
    /// </summary>
    public void ResetGame()
    {
        isDead = false;
        catData.Reset();
        currentState = CatState.Idle;
        UpdateAnimationState();
        OnStatsChanged?.Invoke();
    }
    
    /// <summary>
    /// 获取猫咪数据
    /// </summary>
    public CatData GetCatData()
    {
        return catData;
    }
    
    /// <summary>
    /// 开始游戏（由StartupManager调用）
    /// </summary>
    public void StartGame()
    {
        Debug.Log($"CatManager.StartGame() 被调用！当前 gameStarted={gameStarted}");
        
        gameStarted = true;
        gameStartTime = Time.time;
        
        Debug.Log($"CatManager: gameStarted 已设置为 {gameStarted}");
        
        // 重置数据为满值
        catData.hunger = 100f;
        catData.happiness = 100f;
        catData.hygiene = 100f;
        catData.survivalTime = 0f;
        catData.currentCriticalTime = 0f;
        
        OnStatsChanged?.Invoke();
        
        Debug.Log($"CatManager: 游戏开始！属性重置为满值，开始衰减 (hunger={catData.hunger}, happiness={catData.happiness}, hygiene={catData.hygiene})");
    }
    
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        gameStarted = false;
        Debug.Log("CatManager: 游戏暂停");
    }
}

