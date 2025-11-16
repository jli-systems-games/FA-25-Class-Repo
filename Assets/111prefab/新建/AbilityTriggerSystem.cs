using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 能力触发系统 - 负责主动技能触发、CD管理、效果激活
/// </summary>
public class AbilityTriggerSystem : MonoBehaviour
{
    public static AbilityTriggerSystem instance;

    [Header("技能状态")]
    public bool aliceSkillActive = false;
    public bool redQueenSkillActive = false;
    public bool whiteQueenSkillActive = false;

    [Header("Alice 击杀计数")]
    public int aliceKillCounter = 0;
    public int aliceKillRequired = 10;
    public bool aliceCountingActive = true;

    [Header("技能CD")]
    public float redQueenCooldown = 10f;
    public float whiteQueenCooldown = 20f;

    // ⭐ 修改：改为 public，让 UI 可以读取
    public float redQueenCDRemaining = 0f;      // Red Queen CD剩余时间
    public float whiteQueenCDRemaining = 0f;    // White Queen CD剩余时间

    [Header("特效Prefab")]
    public GameObject whiteQueenLv0EffectPrefab;
    public GameObject whiteQueenLv2EffectPrefab;

    [Header("技能激活时启用的场景物体")]
    public List<GameObject> aliceLv0Effects = new List<GameObject>();
    public List<GameObject> aliceLv1Effects = new List<GameObject>();
    public List<GameObject> aliceLv2Effects = new List<GameObject>();

    public List<GameObject> redQueenLv0Effects = new List<GameObject>();
    public List<GameObject> redQueenLv1Effects = new List<GameObject>();
    public List<GameObject> redQueenLv2Effects = new List<GameObject>();

    public List<GameObject> whiteQueenLv0Effects = new List<GameObject>();
    public List<GameObject> whiteQueenLv1Effects = new List<GameObject>();
    public List<GameObject> whiteQueenLv2Effects = new List<GameObject>();

    [Header("White Queen Lv2 特效位置")]
    public Vector3 whiteQueenLv2EffectPosition = new Vector3(0, 14.5f, 22.6f);
    public Vector3 whiteQueenLv2EffectRotation = new Vector3(-34.03f, 0, 0);
    public Vector3 whiteQueenLv2EffectScale = new Vector3(23.56f, 23.56f, 23.56f);

    private BallLauncher launcherRef;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        launcherRef = FindObjectOfType<BallLauncher>();
        if (launcherRef == null)
        {
            Debug.LogError("未找到 BallLauncher！");
        }

        // 确保所有特效物体初始状态为禁用
        InitializeEffectObjects();

        if (SkillDataStorage.instance != null)
        {
            SkillDataStorage.instance.PrintCurrentSkills();
        }
    }

    // 初始化所有特效物体状态
    void InitializeEffectObjects()
    {
        Debug.Log("🔧 初始化所有技能特效物体状态...");

        DeactivateEffectObjects(aliceLv0Effects);
        DeactivateEffectObjects(aliceLv1Effects);
        DeactivateEffectObjects(aliceLv2Effects);
        DeactivateEffectObjects(redQueenLv0Effects);
        DeactivateEffectObjects(redQueenLv1Effects);
        DeactivateEffectObjects(redQueenLv2Effects);
        DeactivateEffectObjects(whiteQueenLv0Effects);
        DeactivateEffectObjects(whiteQueenLv1Effects);
        DeactivateEffectObjects(whiteQueenLv2Effects);

        Debug.Log("✅ 所有技能特效物体已设为禁用状态");
    }

    void Update()
    {
        // 更新CD计时器
        if (redQueenCDRemaining > 0)
        {
            redQueenCDRemaining -= Time.deltaTime;
        }

        if (whiteQueenCDRemaining > 0)
        {
            whiteQueenCDRemaining -= Time.deltaTime;
        }

        HandleKeyboardInput();
    }

    void HandleKeyboardInput()
    {
        if (SkillDataStorage.instance == null) return;

        // G键 - Red Queen
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (SkillDataStorage.instance.redQueenSkillLevel > 0)
            {
                TryActivateRedQueen();
            }
        }

        // H键 - White Queen
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (SkillDataStorage.instance.whiteQueenSkillLevel > 0)
            {
                TryActivateWhiteQueen();
            }
        }
    }

    // ===== Alice 技能 =====

    public void OnEnemyKilledNotification()
    {
        if (SkillDataStorage.instance == null || SkillDataStorage.instance.aliceSkillLevel == 0)
            return;

        if (aliceSkillActive || !aliceCountingActive)
            return;

        aliceKillCounter++;
        Debug.Log($"Alice 击杀计数: {aliceKillCounter}/{aliceKillRequired}");

        if (aliceKillCounter >= aliceKillRequired)
        {
            aliceKillCounter = 0;
            StartCoroutine(ActivateAliceSkill());
        }
    }

    IEnumerator ActivateAliceSkill()
    {
        if (SkillDataStorage.instance == null) yield break;

        int skillLevel = SkillDataStorage.instance.aliceSkillLevel;
        aliceSkillActive = true;
        aliceCountingActive = false;

        int projectileCount = 0;
        float durationTime = 0f;
        float intervalMult = 0.7f;
        List<GameObject> effectsToActivate = null;

        switch (skillLevel)
        {
            case 1: // Lv0
                projectileCount = 2;
                durationTime = 5f;
                intervalMult = 0.7f;
                effectsToActivate = aliceLv0Effects;
                break;
            case 2: // Lv1
                projectileCount = 3;
                durationTime = 10f;
                intervalMult = 0.7f;
                effectsToActivate = aliceLv1Effects;
                break;
            case 3: // Lv2
                projectileCount = 6;
                durationTime = 10f;
                intervalMult = 0.3f;
                effectsToActivate = aliceLv2Effects;
                break;
        }

        Debug.Log($"Alice 技能激活！Lv{skillLevel - 1}, {projectileCount}弹道, {durationTime}秒");

        // ⭐ 激活瞬间：先关闭所有Alice特效，再打开对应等级的
        ForceDeactivateSkillCategory("Alice");
        ActivateEffectObjects(effectsToActivate);

        if (launcherRef != null)
        {
            launcherRef.aliceMultiShotActive = true;
            launcherRef.aliceMultiShotCount = projectileCount;
            launcherRef.aliceBulletScale = 0.6f;
            launcherRef.tempIntervalMultiplier = intervalMult;
        }

        yield return new WaitForSeconds(durationTime);

        if (launcherRef != null)
        {
            launcherRef.aliceMultiShotActive = false;
            launcherRef.aliceMultiShotCount = 1;
            launcherRef.aliceBulletScale = 1f;
            launcherRef.tempIntervalMultiplier = 1f;
        }

        DeactivateEffectObjects(effectsToActivate);

        aliceSkillActive = false;
        aliceCountingActive = true;

        Debug.Log("Alice 技能结束");
    }

    // ===== Red Queen 技能 =====

    void TryActivateRedQueen()
    {
        if (redQueenSkillActive)
        {
            Debug.Log("Red Queen 技能正在激活中！");
            return;
        }

        if (redQueenCDRemaining > 0)
        {
            Debug.Log($"Red Queen CD中，还需 {redQueenCDRemaining:F1} 秒");
            return;
        }

        StartCoroutine(ActivateRedQueenSkill());
    }

    IEnumerator ActivateRedQueenSkill()
    {
        if (SkillDataStorage.instance == null) yield break;

        int skillLevel = SkillDataStorage.instance.redQueenSkillLevel;
        redQueenSkillActive = true;
        redQueenCDRemaining = redQueenCooldown;

        List<GameObject> effectsToActivate = null;

        Debug.Log($"Red Queen 技能激活！Lv{skillLevel - 1}");

        // ⭐ 激活瞬间：先关闭所有Red Queen特效
        ForceDeactivateSkillCategory("RedQueen");

        switch (skillLevel)
        {
            case 1: // Lv0
                effectsToActivate = redQueenLv0Effects;
                if (launcherRef != null)
                {
                    launcherRef.redQueenSizeMultiplier = 2f;
                    launcherRef.redQueenForceMultiplier = 1f;
                    launcherRef.redQueenVelocityMultiplier = 2f;
                }
                ActivateEffectObjects(effectsToActivate);
                yield return new WaitForSeconds(5f);
                DeactivateEffectObjects(effectsToActivate);
                break;

            case 2: // Lv1
                effectsToActivate = redQueenLv1Effects;
                if (launcherRef != null)
                {
                    launcherRef.redQueenSizeMultiplier = 3f;
                    launcherRef.redQueenForceMultiplier = 1.5f;
                    launcherRef.redQueenVelocityMultiplier = 2f;
                }
                ActivateEffectObjects(effectsToActivate);
                yield return new WaitForSeconds(5f);
                DeactivateEffectObjects(effectsToActivate);
                break;

            case 3: // Lv2
                effectsToActivate = redQueenLv2Effects;
                ActivateEffectObjects(effectsToActivate);

                if (launcherRef != null)
                {
                    launcherRef.redQueenVelocityMultiplier = 2f;

                    for (int i = 0; i < 3; i++)
                    {
                        launcherRef.FireRedQueenGiantBall(7f);
                        if (i < 2)
                        {
                            yield return new WaitForSeconds(2f);
                        }
                    }
                }

                DeactivateEffectObjects(effectsToActivate);
                break;
        }

        if (launcherRef != null)
        {
            launcherRef.redQueenSizeMultiplier = 1f;
            launcherRef.redQueenForceMultiplier = 1f;
            launcherRef.redQueenVelocityMultiplier = 1f;
        }

        redQueenSkillActive = false;
        Debug.Log("Red Queen 技能结束");
    }

    // ===== White Queen 技能 =====

    void TryActivateWhiteQueen()
    {
        if (whiteQueenSkillActive)
        {
            Debug.Log("White Queen 技能正在激活中！");
            return;
        }

        if (whiteQueenCDRemaining > 0)
        {
            Debug.Log($"White Queen CD中，还需 {whiteQueenCDRemaining:F1} 秒");
            return;
        }

        StartCoroutine(ActivateWhiteQueenSkill());
    }

    IEnumerator ActivateWhiteQueenSkill()
    {
        if (SkillDataStorage.instance == null) yield break;

        int skillLevel = SkillDataStorage.instance.whiteQueenSkillLevel;
        whiteQueenSkillActive = true;
        whiteQueenCDRemaining = whiteQueenCooldown;

        float durationTime = (skillLevel == 3) ? 4f : 10f;
        float aoeRadiusValue = 5f;
        float effectScaleValue = 1f;
        float speedMult = 1f;
        List<GameObject> effectsToActivate = null;

        switch (skillLevel)
        {
            case 1: // Lv0
                aoeRadiusValue = 5f;
                effectScaleValue = 1f;
                speedMult = 1f;
                effectsToActivate = whiteQueenLv0Effects;
                break;
            case 2: // Lv1
                aoeRadiusValue = 8f;
                effectScaleValue = 1.5f;
                speedMult = 1.3f;
                effectsToActivate = whiteQueenLv1Effects;
                break;
            case 3: // Lv2
                aoeRadiusValue = 999f;
                effectScaleValue = 2f;
                speedMult = 1f;
                effectsToActivate = whiteQueenLv2Effects;
                break;
        }

        Debug.Log($"White Queen 技能激活！Lv{skillLevel - 1}, AOE半径:{aoeRadiusValue}, {durationTime}秒");

        // ⭐ 激活瞬间：先关闭所有White Queen特效
        ForceDeactivateSkillCategory("WhiteQueen");
        ActivateEffectObjects(effectsToActivate);

        if (launcherRef != null)
        {
            launcherRef.whiteQueenActive = true;
            launcherRef.whiteQueenLevel = skillLevel;
            launcherRef.whiteQueenAOERadius = aoeRadiusValue;
            launcherRef.whiteQueenEffectScale = effectScaleValue;
            launcherRef.tempIntervalMultiplier *= speedMult;
        }

        yield return new WaitForSeconds(durationTime);

        if (launcherRef != null)
        {
            launcherRef.whiteQueenActive = false;
            launcherRef.whiteQueenLevel = 0;
            launcherRef.whiteQueenAOERadius = 0f;
            launcherRef.whiteQueenEffectScale = 1f;
            launcherRef.tempIntervalMultiplier /= speedMult;
        }

        DeactivateEffectObjects(effectsToActivate);

        whiteQueenSkillActive = false;
        Debug.Log("White Queen 技能结束");
    }

    public void SpawnWhiteQueenEffect(Vector3 position, int level)
    {
        Debug.Log($"[SpawnWhiteQueenEffect] 开始生成特效 - 位置:{position}, 等级:{level}");

        GameObject effectPrefab = null;
        Vector3 spawnPos = position;
        Quaternion rotation = Quaternion.identity;
        Vector3 scale = Vector3.one;

        if (level == 3)
        {
            effectPrefab = whiteQueenLv2EffectPrefab;
            spawnPos = whiteQueenLv2EffectPosition;
            rotation = Quaternion.Euler(whiteQueenLv2EffectRotation);
            scale = whiteQueenLv2EffectScale;
            Debug.Log($"[SpawnWhiteQueenEffect] Lv2模式 - Prefab:{(effectPrefab != null ? effectPrefab.name : "NULL")}");
        }
        else
        {
            effectPrefab = whiteQueenLv0EffectPrefab;
            if (launcherRef != null)
            {
                scale = Vector3.one * launcherRef.whiteQueenEffectScale;
                Debug.Log($"[SpawnWhiteQueenEffect] Lv0/1模式 - Prefab:{(effectPrefab != null ? effectPrefab.name : "NULL")}, Scale:{scale}");
            }
            else
            {
                Debug.LogWarning("[SpawnWhiteQueenEffect] launcherRef 为空！");
            }
        }

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, spawnPos, rotation);
            effect.transform.localScale = scale;
            Destroy(effect, 1.5f);
            Debug.Log($"✅ [SpawnWhiteQueenEffect] 特效已成功生成！名称:{effect.name}, 位置:{spawnPos}, 缩放:{scale}");
        }
        else
        {
            Debug.LogError($"❌ [SpawnWhiteQueenEffect] 特效Prefab未设置！等级:{level}, 请在Inspector中设置 whiteQueenLv0EffectPrefab！");
        }
    }

    void ActivateEffectObjects(List<GameObject> effectList)
    {
        if (effectList == null || effectList.Count == 0) return;

        foreach (GameObject obj in effectList)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"✅ [激活特效] {obj.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ [激活特效] 列表中包含空引用!");
            }
        }
    }

    void DeactivateEffectObjects(List<GameObject> effectList)
    {
        if (effectList == null || effectList.Count == 0) return;

        foreach (GameObject obj in effectList)
        {
            if (obj != null)
            {
                // 只禁用当前激活的物体
                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                    Debug.Log($"❌ [禁用特效] {obj.name}");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ [禁用特效] 列表中包含空引用!");
            }
        }
    }

    // ⭐ 新增：强制禁用某个技能分类的所有等级特效
    void ForceDeactivateSkillCategory(string category)
    {
        Debug.Log($"🔒 [强制清理] {category} 技能分类的所有特效");

        switch (category)
        {
            case "Alice":
                DeactivateEffectObjects(aliceLv0Effects);
                DeactivateEffectObjects(aliceLv1Effects);
                DeactivateEffectObjects(aliceLv2Effects);
                break;

            case "RedQueen":
                DeactivateEffectObjects(redQueenLv0Effects);
                DeactivateEffectObjects(redQueenLv1Effects);
                DeactivateEffectObjects(redQueenLv2Effects);
                break;

            case "WhiteQueen":
                DeactivateEffectObjects(whiteQueenLv0Effects);
                DeactivateEffectObjects(whiteQueenLv1Effects);
                DeactivateEffectObjects(whiteQueenLv2Effects);
                break;
        }
    }
}