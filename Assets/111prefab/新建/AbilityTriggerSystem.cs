using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbilityTriggerSystem : MonoBehaviour
{
    public static AbilityTriggerSystem instance;

    [Header("技能状态")]
    public bool aliceSkillActive = false;
    public bool redQueenSkillActive = false;
    public bool whiteQueenSkillActive = false;

    [Header("击杀")]
    public int aliceKillCounter = 0;
    public int aliceKillRequired = 10;
    public bool aliceCountingActive = true;

    [Header("技能")]
    public float redQueenCooldown = 10f;
    public float whiteQueenCooldown = 20f;

    public float redQueenCDRemaining = 0f;
    public float whiteQueenCDRemaining = 0f;

    [Header("特效")]
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

    [Header("W1l22 特效位置")]
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
            // launcher找不到的话...算了先不管
        }

        InitializeEffectObjects();

        if (SkillDataStorage.instance != null)
        {
            SkillDataStorage.instance.PrintCurrentSkills();
        }
    }

    // 开场把所有特效关了
    void InitializeEffectObjects()
    {
        DeactivateEffectObjects(aliceLv0Effects);
        DeactivateEffectObjects(aliceLv1Effects);
        DeactivateEffectObjects(aliceLv2Effects);
        DeactivateEffectObjects(redQueenLv0Effects);
        DeactivateEffectObjects(redQueenLv1Effects);
        DeactivateEffectObjects(redQueenLv2Effects);
        DeactivateEffectObjects(whiteQueenLv0Effects);
        DeactivateEffectObjects(whiteQueenLv1Effects);
        DeactivateEffectObjects(whiteQueenLv2Effects);

        Debug.Log("所有技能特效物体已禁用");
    }

    void Update()
    {
        // cd倒计时
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

        // G键放红皇后
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (SkillDataStorage.instance.redQueenSkillLevel > 0)
            {
                TryActivateRedQueen();
            }
        }

        // H键放白皇后
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (SkillDataStorage.instance.whiteQueenSkillLevel > 0)
            {
                TryActivateWhiteQueen();
            }
        }
    }

    // alice的被动，杀够10个就触发
    public void OnEnemyKilledNotification()
    {
        if (SkillDataStorage.instance == null || SkillDataStorage.instance.aliceSkillLevel == 0)
            return;

        if (aliceSkillActive || !aliceCountingActive)
            return;

        aliceKillCounter++;
        Debug.Log("Alice击杀计数: " + aliceKillCounter + "/" + aliceKillRequired);

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

        // 根据等级设置参数
        switch (skillLevel)
        {
            case 1: // lv0: 2发, 5秒
                projectileCount = 2;
                durationTime = 5f;
                intervalMult = 0.7f;
                effectsToActivate = aliceLv0Effects;
                break;
            case 2: // lv1: 3发, 10秒
                projectileCount = 3;
                durationTime = 10f;
                intervalMult = 0.7f;
                effectsToActivate = aliceLv1Effects;
                break;
            case 3: // lv2: 6发, 10秒, 快速射击
                projectileCount = 6;
                durationTime = 10f;
                intervalMult = 0.3f;
                effectsToActivate = aliceLv2Effects;
                break;
        }

        Debug.Log("Alice技能激活 Lv" + (skillLevel - 1) + ", 弹道数:" + projectileCount + ", 持续:" + durationTime + "秒");

        // 先把alice的其他等级特效关了
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

        // 时间到了恢复原样
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

        Debug.Log("Alice技能结束");
    }

    // 红皇后主动技能
    void TryActivateRedQueen()
    {
        if (redQueenSkillActive)
        {
            Debug.Log("Red Queen技能正在使用中");
            return;
        }

        if (redQueenCDRemaining > 0)
        {
            Debug.Log("Red Queen CD中, 还需" + redQueenCDRemaining.ToString("F1") + "秒");
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

        Debug.Log("Red Queen技能激活 Lv" + (skillLevel - 1));

        ForceDeactivateSkillCategory("RedQueen");

        switch (skillLevel)
        {
            case 1: // lv0: 子弹变大2倍，持续5秒
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

            case 2: // lv1: 子弹变大3倍，击退更强
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

            case 3: // lv2: 发射3个巨型球
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

        // 技能结束后复原
        if (launcherRef != null)
        {
            launcherRef.redQueenSizeMultiplier = 1f;
            launcherRef.redQueenForceMultiplier = 1f;
            launcherRef.redQueenVelocityMultiplier = 1f;
        }

        redQueenSkillActive = false;
        Debug.Log("Red Queen技能结束");
    }

    // 白皇后主动技能
    void TryActivateWhiteQueen()
    {
        if (whiteQueenSkillActive)
        {
            Debug.Log("White Queen技能正在使用中");
            return;
        }

        if (whiteQueenCDRemaining > 0)
        {
            Debug.Log("White Queen CD中, 还需" + whiteQueenCDRemaining.ToString("F1") + "秒");
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

        float durationTime = (skillLevel == 3) ? 4f : 10f; // lv2只持续4秒
        float aoeRadiusValue = 5f;
        float effectScaleValue = 1f;
        float speedMult = 1f;
        List<GameObject> effectsToActivate = null;

        switch (skillLevel)
        {
            case 1: // lv0: 半径5, 持续10秒
                aoeRadiusValue = 5f;
                effectScaleValue = 1f;
                speedMult = 1f;
                effectsToActivate = whiteQueenLv0Effects;
                break;
            case 2: // lv1: 半径8, 攻速加快
                aoeRadiusValue = 8f;
                effectScaleValue = 1.5f;
                speedMult = 1.3f;
                effectsToActivate = whiteQueenLv1Effects;
                break;
            case 3: // lv2: 全屏aoe，只持续4秒
                aoeRadiusValue = 999f;
                effectScaleValue = 2f;
                speedMult = 1f;
                effectsToActivate = whiteQueenLv2Effects;
                break;
        }

        Debug.Log("White Queen技能激活 Lv" + (skillLevel - 1) + ", AOE半径:" + aoeRadiusValue + ", 持续:" + durationTime + "秒");

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

        // 技能时间结束
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
        Debug.Log("White Queen技能结束");
    }

    // launcher那边会调这个来生成aoe特效
    public void SpawnWhiteQueenEffect(Vector3 position, int level)
    {
        Debug.Log("[SpawnWhiteQueenEffect] 开始生成特效 - 位置:" + position + ", 等级:" + level);

        GameObject effectPrefab = null;
        Vector3 spawnPos = position;
        Quaternion rotation = Quaternion.identity;
        Vector3 scale = Vector3.one;

        if (level == 3) // lv2用特殊的大范围特效
        {
            effectPrefab = whiteQueenLv2EffectPrefab;
            spawnPos = whiteQueenLv2EffectPosition;
            rotation = Quaternion.Euler(whiteQueenLv2EffectRotation);
            scale = whiteQueenLv2EffectScale;
            Debug.Log("[SpawnWhiteQueenEffect] Lv2模式 - Prefab:" + (effectPrefab != null ? effectPrefab.name : "NULL"));
        }
        else // lv0和lv1用同一个prefab但缩放不同
        {
            effectPrefab = whiteQueenLv0EffectPrefab;
            if (launcherRef != null)
            {
                scale = Vector3.one * launcherRef.whiteQueenEffectScale;
                Debug.Log("[SpawnWhiteQueenEffect] Lv0/1模式 - Prefab:" + (effectPrefab != null ? effectPrefab.name : "NULL") + ", Scale:" + scale);
            }
            else
            {
                Debug.LogWarning("[SpawnWhiteQueenEffect] launcherRef为空");
            }
        }

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, spawnPos, rotation);
            effect.transform.localScale = scale;
            Destroy(effect, 1.5f); // 1.5秒后销毁
            Debug.Log("[SpawnWhiteQueenEffect] 特效已生成 - 名称:" + effect.name + ", 位置:" + spawnPos + ", 缩放:" + scale);
        }
        else
        {
            Debug.LogError("[SpawnWhiteQueenEffect] 特效Prefab未设置, 等级:" + level + ", 请在Inspector中设置whiteQueenLv0EffectPrefab");
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
                Debug.Log("[激活特效] " + obj.name);
            }
            else
            {
                Debug.LogWarning("[激活特效] 列表中包含空引用");
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
                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                    Debug.Log("[禁用特效] " + obj.name);
                }
            }
            else
            {
                Debug.LogWarning("[禁用特效] 列表中包含空引用");
            }
        }
    }

    // 强制关闭某个技能的所有等级特效，防止特效堆叠
    void ForceDeactivateSkillCategory(string category)
    {
        Debug.Log("[强制清理] " + category + "技能分类的所有特效");

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
