using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 玩家能力管理器 - 增强版
/// 支持技能特效、UI图标状态管理、技能音效
/// </summary>
public class PlayerAbilityManager : MonoBehaviour
{
    #region 单例模式
    private static PlayerAbilityManager instance;
    public static PlayerAbilityManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerAbilityManager>();

                if (instance == null)
                {
                    GameObject go = new GameObject("PlayerAbilityManager");
                    instance = go.AddComponent<PlayerAbilityManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    #region 能力等级
    [Header("主动技能等级")]
    public int redQueenLevel = 0;
    public int aliceLevel = 0;

    [Header("被动加成层数")]
    public int fireRateStacks = 0;
    public int bulletSpeedStacks = 0;
    public int bulletSizeStacks = 0;
    #endregion

    #region 被动加成数值
    [Header("被动加成数值设置")]
    public float fireRateBoostPerStack = 0.15f;
    public float bulletSpeedBoostPerStack = 0.20f;
    public float bulletSizeBoostPerStack = 0.25f;
    #endregion

    #region 红皇后技能设置
    [Header("红皇后技能设置")]
    public float redQueen1SpeedMultiplier = 2.5f;
    public float redQueen1Duration = 2f;
    public float redQueen1Cooldown = 10f;

    public float redQueen2SpeedMultiplier = 3.5f;
    public float redQueen2Duration = 3f;
    public float redQueen2Cooldown = 8f;
    #endregion

    #region 爱丽丝技能设置
    [Header("爱丽丝技能设置")]
    public int alice1ExtraShots = 1;
    public float alice1Duration = 3f;
    public float alice1Cooldown = 12f;

    public int alice2ExtraShots = 3;
    public float alice2Duration = 4f;
    public float alice2Cooldown = 10f;
    #endregion

    #region 技能特效设置
    [Header("技能特效Prefab")]
    [Tooltip("红皇后技能激活时生成的特效prefab")]
    public GameObject redQueenEffectPrefab;

    [Tooltip("爱丽丝技能激活时生成的特效prefab")]
    public GameObject aliceEffectPrefab;

    [Tooltip("特效生成的父物体（通常是玩家）")]
    public Transform effectParent;

    [Header("技能音效")]
    [Tooltip("红皇后技能激活音效")]
    public AudioClip redQueenSFX;

    [Tooltip("爱丽丝技能激活音效")]
    public AudioClip aliceSFX;

    [Tooltip("音效音量")]
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;

    private GameObject currentRedQueenEffect;
    private GameObject currentAliceEffect;
    private AudioSource audioSource;
    #endregion

    #region UI图标设置
    [Header("技能UI图标")]
    [Tooltip("红皇后技能的UI图标Image")]
    public Image redQueenIconImage;

    [Tooltip("爱丽丝技能的UI图标Image")]
    public Image aliceIconImage;

    [Header("UI颜色设置")]
    [Tooltip("技能可用时的颜色（白色）")]
    public Color availableColor = Color.white;

    [Tooltip("技能CD中的颜色（灰色）")]
    public Color cooldownColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Tooltip("技能未解锁的颜色（深灰色）")]
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [Header("UI自动查找设置")]
    [Tooltip("是否自动查找UI图标（适用于每个场景UI不同的情况）")]
    public bool autoFindUIIcons = true;

    [Tooltip("红皇后图标的物体名称")]
    public string redQueenIconName = "QQFSkill";

    [Tooltip("爱丽丝图标的物体名称")]
    public string aliceIconName = "AliceSkill";

    [Header("特效父物体自动查找")]
    [Tooltip("是否自动查找特效父物体")]
    public bool autoFindEffectParent = true;

    [Tooltip("特效父物体的名称")]
    public string effectParentName = "Player Alice";
    #endregion

    #region 技能状态
    [Header("技能运行状态 (只读)")]
    [SerializeField] private bool redQueenActive = false;
    [SerializeField] private bool aliceActive = false;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float redQueenCooldownTimer = 0f;
    [SerializeField] private float aliceCooldownTimer = 0f;

    public bool IsRedQueenActive => redQueenActive;
    public bool IsAliceActive => aliceActive;
    public bool IsInvincible => isInvincible;
    public float RedQueenCooldownRemaining => redQueenCooldownTimer;
    public float AliceCooldownRemaining => aliceCooldownTimer;
    #endregion

    #region 调试设置
    [Header("调试")]
    public bool showDebugInfo = true;
    #endregion

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 添加 AudioSource 组件用于播放音效
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = sfxVolume;

            if (showDebugInfo)
            {
                Debug.Log("PlayerAbilityManager 初始化完成");
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 初始化UI
        UpdateAllIconStates();
    }

    void Update()
    {
        // 自动查找UI图标（如果丢失或者切换了场景）
        if (autoFindUIIcons)
        {
            TryFindUIIcons();
        }

        // 更新冷却计时器
        if (redQueenCooldownTimer > 0)
        {
            redQueenCooldownTimer -= Time.deltaTime;
        }

        if (aliceCooldownTimer > 0)
        {
            aliceCooldownTimer -= Time.deltaTime;
        }

        // 更新UI图标状态
        UpdateAllIconStates();

        // 检测技能按键
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (showDebugInfo)
            {
                Debug.Log($"按下E键 - 红皇后等级: {redQueenLevel}");
            }

            if (redQueenLevel > 0)
            {
                ActivateRedQueen();
            }
            else if (showDebugInfo)
            {
                Debug.Log("红皇后技能未解锁！");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (showDebugInfo)
            {
                Debug.Log($"按下Q键 - 爱丽丝等级: {aliceLevel}");
            }

            if (aliceLevel > 0)
            {
                ActivateAlice();
            }
            else if (showDebugInfo)
            {
                Debug.Log("爱丽丝技能未解锁！");
            }
        }
    }

    #region 主动技能激活
    public void ActivateRedQueen()
    {
        if (redQueenLevel == 0)
        {
            if (showDebugInfo) Debug.Log("红皇后技能未解锁！");
            return;
        }

        if (redQueenActive)
        {
            if (showDebugInfo) Debug.Log("红皇后技能正在使用中！");
            return;
        }

        if (redQueenCooldownTimer > 0)
        {
            if (showDebugInfo) Debug.Log($"红皇后技能冷却中，剩余 {redQueenCooldownTimer:F1} 秒");
            return;
        }

        // 播放音效
        PlaySFX(redQueenSFX);

        float duration = redQueenLevel == 1 ? redQueen1Duration : redQueen2Duration;
        float cooldown = redQueenLevel == 1 ? redQueen1Cooldown : redQueen2Cooldown;

        StartCoroutine(RedQueenCoroutine(duration, cooldown));

        if (showDebugInfo)
        {
            Debug.Log($"✨ 激活红皇后技能 Lv.{redQueenLevel}！持续 {duration} 秒");
        }
    }

    public void ActivateAlice()
    {
        if (aliceLevel == 0)
        {
            if (showDebugInfo) Debug.Log("爱丽丝技能未解锁！");
            return;
        }

        if (aliceActive)
        {
            if (showDebugInfo) Debug.Log("爱丽丝技能正在使用中！");
            return;
        }

        if (aliceCooldownTimer > 0)
        {
            if (showDebugInfo) Debug.Log($"爱丽丝技能冷却中，剩余 {aliceCooldownTimer:F1} 秒");
            return;
        }

        // 播放音效
        PlaySFX(aliceSFX);

        float duration = aliceLevel == 1 ? alice1Duration : alice2Duration;
        float cooldown = aliceLevel == 1 ? alice1Cooldown : alice2Cooldown;

        StartCoroutine(AliceCoroutine(duration, cooldown));

        if (showDebugInfo)
        {
            Debug.Log($"✨ 激活爱丽丝技能 Lv.{aliceLevel}！持续 {duration} 秒");
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.volume = sfxVolume;
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator RedQueenCoroutine(float duration, float cooldown)
    {
        redQueenActive = true;
        isInvincible = true;

        // 生成特效
        if (redQueenEffectPrefab != null && effectParent != null)
        {
            currentRedQueenEffect = Instantiate(redQueenEffectPrefab, effectParent.position, effectParent.rotation, effectParent);
            if (showDebugInfo) Debug.Log("生成红皇后特效");
        }

        yield return new WaitForSeconds(duration);

        redQueenActive = false;
        isInvincible = false;
        redQueenCooldownTimer = cooldown;

        // 销毁特效
        if (currentRedQueenEffect != null)
        {
            Destroy(currentRedQueenEffect);
            if (showDebugInfo) Debug.Log("销毁红皇后特效");
        }

        if (showDebugInfo)
        {
            Debug.Log("红皇后技能结束");
        }
    }

    private IEnumerator AliceCoroutine(float duration, float cooldown)
    {
        aliceActive = true;

        // 生成特效
        if (aliceEffectPrefab != null && effectParent != null)
        {
            currentAliceEffect = Instantiate(aliceEffectPrefab, effectParent.position, effectParent.rotation, effectParent);
            if (showDebugInfo) Debug.Log("生成爱丽丝特效");
        }

        yield return new WaitForSeconds(duration);

        aliceActive = false;
        aliceCooldownTimer = cooldown;

        // 销毁特效
        if (currentAliceEffect != null)
        {
            Destroy(currentAliceEffect);
            if (showDebugInfo) Debug.Log("销毁爱丽丝特效");
        }

        if (showDebugInfo)
        {
            Debug.Log("爱丽丝技能结束");
        }
    }
    #endregion

    #region UI图标状态更新
    void TryFindUIIcons()
    {
        // 如果红皇后图标丢失或为空，尝试查找
        if (redQueenIconImage == null)
        {
            GameObject obj = GameObject.Find(redQueenIconName);
            if (obj != null)
            {
                redQueenIconImage = obj.GetComponent<Image>();
                if (redQueenIconImage != null && showDebugInfo)
                {
                    Debug.Log($"自动找到红皇后图标: {redQueenIconName}");
                }
            }
        }

        // 如果爱丽丝图标丢失或为空，尝试查找
        if (aliceIconImage == null)
        {
            GameObject obj = GameObject.Find(aliceIconName);
            if (obj != null)
            {
                aliceIconImage = obj.GetComponent<Image>();
                if (aliceIconImage != null && showDebugInfo)
                {
                    Debug.Log($"自动找到爱丽丝图标: {aliceIconName}");
                }
            }
        }

        // 如果特效父物体丢失或为空，尝试查找
        if (autoFindEffectParent && effectParent == null)
        {
            GameObject obj = GameObject.Find(effectParentName);
            if (obj != null)
            {
                effectParent = obj.transform;
                if (showDebugInfo)
                {
                    Debug.Log($"自动找到特效父物体: {effectParentName}");
                }
            }
        }
    }

    void UpdateAllIconStates()
    {
        UpdateRedQueenIconState();
        UpdateAliceIconState();
    }

    void UpdateRedQueenIconState()
    {
        if (redQueenIconImage == null) return;

        if (redQueenLevel == 0)
        {
            // 未解锁：深灰色
            redQueenIconImage.color = lockedColor;
        }
        else if (redQueenActive || redQueenCooldownTimer > 0)
        {
            // CD中：灰色
            redQueenIconImage.color = cooldownColor;
        }
        else
        {
            // 可用：白色
            redQueenIconImage.color = availableColor;
        }
    }

    void UpdateAliceIconState()
    {
        if (aliceIconImage == null) return;

        if (aliceLevel == 0)
        {
            // 未解锁：深灰色
            aliceIconImage.color = lockedColor;
        }
        else if (aliceActive || aliceCooldownTimer > 0)
        {
            // CD中：灰色
            aliceIconImage.color = cooldownColor;
        }
        else
        {
            // 可用：白色
            aliceIconImage.color = availableColor;
        }
    }
    #endregion

    #region 能力升级方法
    public void UpgradeRedQueen()
    {
        if (redQueenLevel < 2)
        {
            redQueenLevel++;
            UpdateRedQueenIconState();
            if (showDebugInfo)
            {
                Debug.Log($"红皇后技能升级到 Lv.{redQueenLevel}");
            }
        }
    }

    public void UpgradeAlice()
    {
        if (aliceLevel < 2)
        {
            aliceLevel++;
            UpdateAliceIconState();
            if (showDebugInfo)
            {
                Debug.Log($"爱丽丝技能升级到 Lv.{aliceLevel}");
            }
        }
    }

    public void AddFireRateStack()
    {
        fireRateStacks++;
        if (showDebugInfo)
        {
            Debug.Log($"发射速度提升层数: {fireRateStacks}");
        }
    }

    public void AddBulletSpeedStack()
    {
        bulletSpeedStacks++;
        if (showDebugInfo)
        {
            Debug.Log($"子弹速度提升层数: {bulletSpeedStacks}");
        }
    }

    public void AddBulletSizeStack()
    {
        bulletSizeStacks++;
        if (showDebugInfo)
        {
            Debug.Log($"子弹大小提升层数: {bulletSizeStacks}");
        }
    }
    #endregion

    #region 计算加成后的数值
    public float GetMoveSpeedMultiplier()
    {
        if (!redQueenActive) return 1f;

        return redQueenLevel == 1 ? redQueen1SpeedMultiplier : redQueen2SpeedMultiplier;
    }

    public float GetModifiedFireRate(float baseFireRate)
    {
        float reduction = 1f - (fireRateStacks * fireRateBoostPerStack);
        return baseFireRate * Mathf.Max(0.1f, reduction);
    }

    public float GetModifiedBulletSpeed(float baseBulletSpeed)
    {
        float boost = 1f + (bulletSpeedStacks * bulletSpeedBoostPerStack);
        return baseBulletSpeed * boost;
    }

    public float GetModifiedBulletSize(float baseSize = 1f)
    {
        float boost = 1f + (bulletSizeStacks * bulletSizeBoostPerStack);
        return baseSize * boost;
    }

    public int GetAliceExtraShots()
    {
        if (!aliceActive) return 0;

        return aliceLevel == 1 ? alice1ExtraShots : alice2ExtraShots;
    }
    #endregion

    #region 重置和调试方法
    public void ResetAllAbilities()
    {
        redQueenLevel = 0;
        aliceLevel = 0;
        fireRateStacks = 0;
        bulletSpeedStacks = 0;
        bulletSizeStacks = 0;

        redQueenActive = false;
        aliceActive = false;
        isInvincible = false;
        redQueenCooldownTimer = 0f;
        aliceCooldownTimer = 0f;

        UpdateAllIconStates();

        if (showDebugInfo)
        {
            Debug.Log("所有能力已重置");
        }
    }

    public void PrintAbilityStatus()
    {
        Debug.Log("===== 当前能力状态 =====");
        Debug.Log($"红皇后 Lv.{redQueenLevel} | 爱丽丝 Lv.{aliceLevel}");
        Debug.Log($"发射速度: +{fireRateStacks}层 | 子弹速度: +{bulletSpeedStacks}层 | 子弹大小: +{bulletSizeStacks}层");
        Debug.Log("========================");
    }
    #endregion
}