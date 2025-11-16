using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("胜利条件")]
    public int winTargetCount = 50;     // 击败目标数量
    public int maxAllowedEscapes = 10;  // 最大允许逃脱数

    [Header("场景跳转")]
    public string loseSceneName = "FailScene";
    public string winSceneName = "VictoryScene";

    [Header("玩家位置")]
    public Transform playerTransform;      // 玩家位置，用于判断逃脱线

    [Header("实时数据")]
    public int hitCount = 0;        // 当前击败数
    public int escapeCount = 0;     // 当前逃脱数

    private bool gameOver = false;  // 游戏是否结束

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
        // 如果未设置玩家位置，自动查找
        if (playerTransform == null)
        {
            BallLauncher launcher = FindObjectOfType<BallLauncher>();
            if (launcher != null)
            {
                playerTransform = launcher.transform;
            }
        }

        // 重置数据
        hitCount = 0;
        escapeCount = 0;
        gameOver = false;

        Debug.Log($"游戏开始！目标：击败{winTargetCount}个敌人，最多允许{maxAllowedEscapes}次逃脱");
    }

    // 目标被击中
    public void OnTargetHit()
    {
        if (gameOver) return;

        hitCount++;
        Debug.Log($"击败进度: {hitCount}/{winTargetCount}");

        // 通知能力触发系统（用于 Alice 技能触发）
        if (AbilityTriggerSystem.instance != null)
        {
            AbilityTriggerSystem.instance.OnEnemyKilledNotification();
        }

        // 检查胜利
        if (hitCount >= winTargetCount)
        {
            TriggerWin();
        }
    }

    // 目标逃脱
    public void OnTargetEscaped()
    {
        if (gameOver) return;

        escapeCount++;
        Debug.Log($"逃脱次数: {escapeCount}/{maxAllowedEscapes}");

        // 检查失败
        if (escapeCount >= maxAllowedEscapes)
        {
            TriggerLose();
        }
    }

    // 判断是否逃脱（Z值小于玩家）
    public bool CheckIfEscaped(Vector3 targetPos)
    {
        if (playerTransform == null) return false;

        return targetPos.z < playerTransform.position.z;
    }

    // 触发胜利
    void TriggerWin()
    {
        if (gameOver) return;
        gameOver = true;

        Debug.Log("游戏胜利！");

        // 延迟跳转
        StartCoroutine(SwitchSceneAfterDelay(winSceneName, 2f));
    }

    // 触发失败
    void TriggerLose()
    {
        if (gameOver) return;
        gameOver = true;

        Debug.Log("游戏失败！逃脱过多...");

        // 延迟跳转
        StartCoroutine(SwitchSceneAfterDelay(loseSceneName, 2f));
    }

    IEnumerator SwitchSceneAfterDelay(string targetScene, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(targetScene);
    }

    // 获取进度信息
    public string GetGameStats()
    {
        return $"击败: {hitCount}/{winTargetCount}  逃脱: {escapeCount}/{maxAllowedEscapes}";
    }
}