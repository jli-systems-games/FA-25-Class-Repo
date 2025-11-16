using System.Collections;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    [Header("投掷配置")]
    public GameObject projectilePrefab;  // 普通投射物预制体
    public GameObject whiteQueenProjectilePrefab;  // 白皇后专用子弹预制体 ⭐ 新增
    public float minPower = 7f;
    public float maxPower = 15f;
    public float minTiltAngle = 60f;
    public float maxTiltAngle = 80f;
    public float horizontalSpread = 50f;
    public float launchDelay = 1f;
    public float projectileLifespan = 10f;
    public float gravityForce = 9.81f;
    public float collisionRadius = 0.5f;

    [Header("被动技能加成")]
    public float passiveForceMultiplier = 1f;      // 力度加成
    public float passiveRangeMultiplier = 1f;      // 发射范围加成
    public float passiveLaunchSpeedMultiplier = 1f; // 发射速度加成

    [Header("技能效果参数")]
    // Alice
    public bool aliceMultiShotActive = false;
    public int aliceMultiShotCount = 1;
    public float aliceBulletScale = 1f;
    public float aliceSpreadDistance = 1.5f;

    // Red Queen
    public float redQueenSizeMultiplier = 1f;
    public float redQueenForceMultiplier = 1f;
    public float redQueenVelocityMultiplier = 1f;
    public bool redQueenNoRotation = false;

    [Header("Red Queen Lv2 巨球专用参数")]
    public float redQueenGiantBallSize = 7f;
    public float redQueenGiantBallPowerMultiplier = 0.6f;

    // White Queen
    public bool whiteQueenActive = false;
    public int whiteQueenLevel = 0;
    public float whiteQueenAOERadius = 5f;
    public float whiteQueenEffectScale = 1f;

    public float tempIntervalMultiplier = 1f;

    private Coroutine launchRoutine;
    private bool firingPaused = false;

    void Start()
    {
        // 应用被动技能加成
        ApplyPassiveSkills();

        launchRoutine = StartCoroutine(ContinuousLaunch());
    }

    // 应用被动技能加成
    void ApplyPassiveSkills()
    {
        if (SkillDataStorage.instance == null) return;

        // 1. 力度加成 (passiveBonus1) - 大幅提升
        if (SkillDataStorage.instance.passiveBonus1 > 0)
        {
            passiveForceMultiplier = 2.0f; // 力度翻倍 (+100%)
            Debug.Log("✅ 被动技能生效: 力度提升100% (翻倍)");
        }

        // 2. 发射范围加成 (passiveBonus2) - 大幅提升
        if (SkillDataStorage.instance.passiveBonus2 > 0)
        {
            passiveRangeMultiplier = 2.0f; // 范围翻倍 (+100%)
            horizontalSpread *= passiveRangeMultiplier;
            Debug.Log($"✅ 被动技能生效: 发射范围提升100% (翻倍) -> {horizontalSpread}");
        }

        // 3. 发射速度加成 (passiveBonus3) - 大幅加快
        if (SkillDataStorage.instance.passiveBonus3 > 0)
        {
            passiveLaunchSpeedMultiplier = 0.5f; // 延迟减少50% (发射速度翻倍)
            launchDelay *= passiveLaunchSpeedMultiplier;
            Debug.Log($"✅ 被动技能生效: 发射速度提升100% (延迟减半) -> 延迟{launchDelay}秒");
        }
    }

    IEnumerator ContinuousLaunch()
    {
        while (true)
        {
            if (!firingPaused)
            {
                FireProjectile();
            }
            yield return new WaitForSeconds(launchDelay * tempIntervalMultiplier);
        }
    }

    void FireProjectile()
    {
        // White Queen Lv2 (全场清屏模式) - 强制单发,不受Alice影响
        if (whiteQueenActive && whiteQueenLevel == 3)
        {
            FireSingleShot();
            return;
        }

        // 其他情况 - Alice 多弹道模式
        if (aliceMultiShotActive && aliceMultiShotCount > 1)
        {
            FireMultiShot();
        }
        else
        {
            FireSingleShot();
        }
    }

    void FireSingleShot()
    {
        // 应用力度加成
        float throwPower = Random.Range(minPower, maxPower) * redQueenForceMultiplier * passiveForceMultiplier;
        float verticalAngle = Random.Range(minTiltAngle, maxTiltAngle);
        float horizontalAngle = Random.Range(-horizontalSpread, horizontalSpread);

        Vector3 launchDirection = Quaternion.Euler(-verticalAngle, horizontalAngle, 0) * transform.forward;

        float finalScale = redQueenSizeMultiplier;
        StartCoroutine(ProjectileTrajectory(launchDirection * throwPower, transform.position, finalScale));
    }

    void FireMultiShot()
    {
        // 应用力度加成
        float throwPower = Random.Range(minPower, maxPower) * redQueenForceMultiplier * passiveForceMultiplier;
        float verticalAngle = Random.Range(minTiltAngle, maxTiltAngle);

        bool useRandomAngles = (aliceMultiShotCount == 6);

        for (int i = 0; i < aliceMultiShotCount; i++)
        {
            float horizontalAngle;

            if (useRandomAngles)
            {
                horizontalAngle = Random.Range(-horizontalSpread, horizontalSpread);
            }
            else
            {
                float angleStep = horizontalSpread * 2f / (aliceMultiShotCount - 1);
                horizontalAngle = -horizontalSpread + (i * angleStep);
            }

            Vector3 launchDirection = Quaternion.Euler(-verticalAngle, horizontalAngle, 0) * transform.forward;

            float offsetX = (i - (aliceMultiShotCount - 1) / 2f) * aliceSpreadDistance;
            Vector3 spawnPos = transform.position + transform.right * offsetX;

            float finalScale = aliceBulletScale * redQueenSizeMultiplier;
            StartCoroutine(ProjectileTrajectory(launchDirection * throwPower, spawnPos, finalScale));
        }
    }

    IEnumerator ProjectileTrajectory(Vector3 initialVelocity, Vector3 spawnPoint, float scale = 1f, bool noRotation = false, bool isRedQueenGiant = false)
    {
        // ⭐⭐⭐ 核心修改：根据技能状态选择子弹Prefab ⭐⭐⭐
        GameObject selectedPrefab = projectilePrefab;

        // 白皇后技能激活时,使用专用子弹(优先级最高,覆盖Alice和Red Queen的子弹外观)
        if (whiteQueenActive && whiteQueenProjectilePrefab != null)
        {
            selectedPrefab = whiteQueenProjectilePrefab;
            Debug.Log("[子弹选择] 使用白皇后专用子弹");
        }

        GameObject projectile = Instantiate(selectedPrefab, spawnPoint, Quaternion.identity);
        projectile.transform.localScale *= scale;  // 红皇后的缩放依然会作用在白皇后子弹上

        // 应用 Red Queen 速度倍率
        initialVelocity *= redQueenVelocityMultiplier;

        Vector3 currentPos = spawnPoint;
        float elapsedTime = 0;

        Vector3 spinRotation = new Vector3(
            Random.Range(-360f, 360f),
            Random.Range(-360f, 360f),
            Random.Range(-360f, 360f)
        );

        while (elapsedTime < projectileLifespan && projectile != null)
        {
            elapsedTime += Time.fixedDeltaTime;
            initialVelocity.y -= gravityForce * Time.fixedDeltaTime;
            Vector3 nextPosition = currentPos + initialVelocity * Time.fixedDeltaTime;

            if (!noRotation)
            {
                projectile.transform.Rotate(spinRotation * Time.fixedDeltaTime);
            }

            Collider[] nearbyObjects = Physics.OverlapSphere(nextPosition, collisionRadius * scale);

            // White Queen Lv2 全场清屏
            if (whiteQueenActive && whiteQueenLevel == 3 && !isRedQueenGiant && nearbyObjects.Length > 0)
            {
                bool hasValidCollision = false;
                foreach (var obj in nearbyObjects)
                {
                    if (obj.gameObject == projectile)
                        continue;

                    if (obj.gameObject != null)
                    {
                        hasValidCollision = true;
                        break;
                    }
                }

                if (hasValidCollision)
                {
                    Debug.Log("White Queen Lv2: 碰撞触发全场清屏!");
                    ClearAllEnemies();

                    if (AbilityTriggerSystem.instance != null)
                    {
                        AbilityTriggerSystem.instance.SpawnWhiteQueenEffect(nextPosition, whiteQueenLevel);
                    }

                    Destroy(projectile);
                    yield break;
                }
            }

            // 普通碰撞检测
            if (nearbyObjects.Length > 0)
            {
                foreach (var obj in nearbyObjects)
                {
                    if (projectile == null || obj == null || obj.gameObject == null)
                        continue;

                    if (obj.gameObject == projectile)
                        continue;

                    if (obj.CompareTag("enemy"))
                    {
                        Debug.Log($"[碰撞检测] 击中敌人: {obj.gameObject.name}");

                        Vector3 hitPoint = nextPosition;

                        if (GameController.instance != null)
                        {
                            GameController.instance.OnTargetHit();
                        }

                        if (!whiteQueenActive)
                        {
                            Rigidbody targetBody = obj.GetComponent<Rigidbody>();
                            if (targetBody != null)
                            {
                                targetBody.AddForce(initialVelocity.normalized * 10f, ForceMode.Impulse);
                            }
                        }

                        Destroy(obj.gameObject);

                        // White Queen AOE效果(Lv0/Lv1)
                        if (whiteQueenActive && whiteQueenLevel < 3)
                        {
                            PerformWhiteQueenAOE(hitPoint);
                        }

                        if (!isRedQueenGiant)
                        {
                            Destroy(projectile);
                            yield break;
                        }
                    }
                }
            }

            if (projectile != null)
            {
                currentPos = nextPosition;
                projectile.transform.position = currentPos;
            }

            yield return new WaitForFixedUpdate();
        }

        if (projectile != null)
        {
            Destroy(projectile);
        }
    }

    void PerformWhiteQueenAOE(Vector3 hitPosition)
    {
        Debug.Log($"[White Queen AOE] 触发位置: {hitPosition}, 等级: {whiteQueenLevel}, 半径: {whiteQueenAOERadius}");

        if (AbilityTriggerSystem.instance != null)
        {
            AbilityTriggerSystem.instance.SpawnWhiteQueenEffect(hitPosition, whiteQueenLevel);
        }

        Collider[] enemiesInRange = Physics.OverlapSphere(hitPosition, whiteQueenAOERadius);
        int destroyedCount = 0;

        foreach (var enemy in enemiesInRange)
        {
            if (enemy != null && enemy.gameObject != null && enemy.CompareTag("enemy"))
            {
                Destroy(enemy.gameObject);
                destroyedCount++;

                if (GameController.instance != null)
                {
                    GameController.instance.OnTargetHit();
                }
            }
        }

        Debug.Log($"[White Queen AOE] 总共销毁 {destroyedCount} 个范围内敌人");
    }

    public void PauseNormalFiring()
    {
        firingPaused = true;
    }

    public void ResumeNormalFiring()
    {
        firingPaused = false;
    }

    public void FireRedQueenGiantBall(float sizeMultiplier)
    {
        // 应用力度加成到巨球
        float throwPower = Random.Range(minPower, maxPower) * redQueenGiantBallPowerMultiplier * passiveForceMultiplier;
        float verticalAngle = Random.Range(minTiltAngle, maxTiltAngle);
        float horizontalAngle = Random.Range(-horizontalSpread, horizontalSpread);

        Vector3 launchDirection = Quaternion.Euler(-verticalAngle, horizontalAngle, 0) * transform.forward;

        StartCoroutine(ProjectileTrajectory(launchDirection * throwPower, transform.position, redQueenGiantBallSize, true, true));

        Debug.Log($"发射 Red Queen 巨型球 (大小:{redQueenGiantBallSize}倍, 力量:{redQueenGiantBallPowerMultiplier}倍)");
    }

    void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        int clearedCount = 0;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
                clearedCount++;

                if (GameController.instance != null)
                {
                    GameController.instance.OnTargetHit();
                }
            }
        }

        Debug.Log($"White Queen Lv2 全场清屏: 清除了 {clearedCount} 个敌人");
    }
}