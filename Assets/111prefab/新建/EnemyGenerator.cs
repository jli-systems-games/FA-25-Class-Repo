using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDifficultySpawner : MonoBehaviour
{
    [Header("敌人模板")]
    public List<GameObject> enemyTemplates;

    [Header("初始生成参数")]
    public float baseCreationInterval = 1f;
    public float baseTravelSpeed = 5f;
    public float baseScale = 1f;
    public float existenceDuration = 10f;

    [Header("难度递增参数（基于时间）")]
    public float difficultyRampTime = 300f;
    public float finalIntervalMultiplier = 0.3f;
    public float finalSpeedMultiplier = 3f;
    public float finalScaleMultiplier = 0.4f;

    [Header("出生位置范围")]
    public float leftBound = -5f;
    public float rightBound = 6.3f;
    public float heightPos = 23.54f;
    public float depthPos = 47.87f;

    [Header("被动技能加成")]
    public float passiveGenerateSpeedMultiplier = 1f;  // 生成速度加成
    public float passiveEnemySpeedMultiplier = 1f;     // 敌人移动速度加成

    // 当前难度参数
    private float currentInterval;
    private float currentSpeed;
    private float currentScale;
    private float gameElapsedTime = 0f;

    void Start()
    {
        // 应用被动技能
        ApplyPassiveSkills();

        // 初始化难度参数
        currentInterval = baseCreationInterval * passiveGenerateSpeedMultiplier;  // 应用生成速度加成
        currentSpeed = baseTravelSpeed * passiveEnemySpeedMultiplier;              // 应用敌人速度加成
        currentScale = baseScale;
        gameElapsedTime = 0f;

        StartCoroutine(GenerationLoop());
    }

    // 应用被动技能
    void ApplyPassiveSkills()
    {
        if (SkillDataStorage.instance == null) return;

        // 4. 敌人生成速度减慢 (passiveBonus4) - 增加生成间隔
        if (SkillDataStorage.instance.passiveBonus4 > 0)
        {
            passiveGenerateSpeedMultiplier = 1.5f; // 生成间隔增加50% (生成变慢)
            Debug.Log("✅ 被动技能生效: 敌人生成速度减慢50%");
        }

        // 5. 敌人移动速度减慢 (passiveBonus5)
        if (SkillDataStorage.instance.passiveBonus5 > 0)
        {
            passiveEnemySpeedMultiplier = 0.7f; // 移动速度降低30%
            Debug.Log("✅ 被动技能生效: 敌人移动速度减慢30%");
        }
    }

    void Update()
    {
        gameElapsedTime += Time.deltaTime;
        UpdateDifficultyByTime();
    }

    void UpdateDifficultyByTime()
    {
        float difficultyProgress = Mathf.Clamp01(gameElapsedTime / difficultyRampTime);

        // 基础值应用被动加成后再进行难度递增
        float baseIntervalWithPassive = baseCreationInterval * passiveGenerateSpeedMultiplier;
        float baseSpeedWithPassive = baseTravelSpeed * passiveEnemySpeedMultiplier;

        currentInterval = Mathf.Lerp(baseIntervalWithPassive, baseIntervalWithPassive * finalIntervalMultiplier, difficultyProgress);
        currentSpeed = Mathf.Lerp(baseSpeedWithPassive, baseSpeedWithPassive * finalSpeedMultiplier, difficultyProgress);
        currentScale = Mathf.Lerp(baseScale, baseScale * finalScaleMultiplier, difficultyProgress);
    }

    IEnumerator GenerationLoop()
    {
        while (true)
        {
            CreateEnemy();
            yield return new WaitForSeconds(currentInterval);
        }
    }

    void CreateEnemy()
    {
        if (enemyTemplates.Count == 0)
        {
            Debug.LogWarning("敌人模板列表为空！");
            return;
        }

        GameObject template = enemyTemplates[Random.Range(0, enemyTemplates.Count)];
        float randomHorizontal = Random.Range(leftBound, rightBound);
        Vector3 birthPosition = new Vector3(randomHorizontal, heightPos, depthPos);

        GameObject enemy = Instantiate(template, birthPosition, Quaternion.identity);
        enemy.transform.localScale *= currentScale;

        StartCoroutine(MoveEnemy(enemy, currentSpeed));
    }

    IEnumerator MoveEnemy(GameObject enemy, float speed)
    {
        float lifeTimer = 0;
        bool markedAsEscaped = false;

        while (lifeTimer < existenceDuration && enemy != null)
        {
            lifeTimer += Time.deltaTime;
            enemy.transform.position += Vector3.back * speed * Time.deltaTime;

            if (!markedAsEscaped && GameController.instance != null)
            {
                if (GameController.instance.CheckIfEscaped(enemy.transform.position))
                {
                    markedAsEscaped = true;
                    GameController.instance.OnTargetEscaped();
                    Debug.Log("敌人已逃脱！");
                    Destroy(enemy);
                    yield break;
                }
            }

            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider != null)
            {
                Collider[] overlaps = Physics.OverlapBox(
                    enemyCollider.bounds.center,
                    enemyCollider.bounds.extents,
                    enemy.transform.rotation
                );

                foreach (var overlap in overlaps)
                {
                    if (overlap.gameObject != enemy && overlap.CompareTag("cup"))
                    {
                        if (GameController.instance != null)
                        {
                            GameController.instance.OnTargetHit();
                        }
                        Destroy(enemy);
                        yield break;
                    }
                }
            }

            yield return null;
        }

        if (enemy != null && !markedAsEscaped && GameController.instance != null)
        {
            GameController.instance.OnTargetEscaped();
            Debug.Log("敌人超时逃脱！");
        }

        if (enemy != null)
        {
            Destroy(enemy);
        }
    }
}