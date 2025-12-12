using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GreatAchievement.Systems;

public class UnlockAbilityTrigger : MonoBehaviour
{
    public enum AbilityType
    {
        DoubleJump,
        Dash,
        WallSlide
    }

    [Header("Ability Settings")]
    public AbilityType abilityToUnlock;
    [Tooltip("是否实际激活技能。如果为false，只显示UI但不解锁技能")]
    public bool shouldUnlockAbility = true;

    [Header("UI Settings")]
    [Tooltip("包含整个UI的父物体")]
    public GameObject uiPanel;
    [Tooltip("第一层（背景）")]
    public GameObject layer1;
    [Tooltip("第二层（装饰/图标）")]
    public GameObject layer2;
    [Tooltip("第三层（文字/提示）")]
    public GameObject layer3;
    
    [Tooltip("每一层淡入的持续时间")]
    public float fadeDuration = 0.5f;
    [Tooltip("层与层出现的间隔时间")]
    public float layerDelay = 0.3f;

    private bool hasTriggered = false;

    private void Start()
    {
        // 确保UI初始状态是隐藏的
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            if (layer1 != null) { layer1.SetActive(false); ResetCanvasGroup(layer1); }
            if (layer2 != null) { layer2.SetActive(false); ResetCanvasGroup(layer2); }
            if (layer3 != null) { layer3.SetActive(false); ResetCanvasGroup(layer3); }
        }
    }

    private void ResetCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 调试日志
        Debug.Log($"UnlockAbilityTrigger: OnEnter with {other.gameObject.name} (Layer: {other.gameObject.layer})");

        // 修改检测方式：不再依赖 Tag，而是检测 CharacterController2D 组件及其标识符
        // 使用 GetComponentInParent 以防 Collider 在子物体上
        CharacterController2D player = other.GetComponentInParent<CharacterController2D>();
        
        if (player != null && player.playerIdentifier == "Player" && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("触发成功！通过组件检测确认玩家身份。开始解锁流程...");
            StartCoroutine(UnlockSequence(player.gameObject));
        }
        else
        {
            if (player == null)
                Debug.Log("触发忽略：未找到 CharacterController2D 组件");
            else if (player.playerIdentifier != "Player")
                Debug.Log($"触发忽略：标识符不匹配 (当前: {player.playerIdentifier})");
        }
    }

    // 增加碰撞检测作为调试辅助
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("注意：玩家与物体发生了物理碰撞 (OnCollisionEnter2D)，而不是触发器事件。请检查此物体的 Collider 2D 组件，务必勾选 'Is Trigger'！");
        }
    }

    private IEnumerator UnlockSequence(GameObject playerObj)
    {
        // 1. 冻结游戏 (使用 unscaledDeltaTime 来处理动画)
        Time.timeScale = 0f;

        // 2. 更新数据 (GameManager & PlayerController) - 仅在 shouldUnlockAbility 为 true 时执行
        if (shouldUnlockAbility)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnlockAbility(abilityToUnlock.ToString());
                Debug.Log($"GameManager: UnlockAbility {abilityToUnlock} CALLED.");
            }
            else
            {
                Debug.LogError("UnlockAbilityTrigger: GameManager not found!");
            }

            CharacterController2D playerController = playerObj.GetComponent<CharacterController2D>();
            if (playerController != null)
            {
                switch (abilityToUnlock)
                {
                    case AbilityType.DoubleJump:
                        playerController.enableDoubleJump = true;
                        break;
                    case AbilityType.Dash:
                        playerController.enableDash = true;
                        break;
                    case AbilityType.WallSlide:
                        playerController.enableWallSlide = true;
                        break;
                }
                Debug.Log("PlayerController: Ability enabled locally.");
            }
            else
            {
                Debug.LogError("UnlockAbilityTrigger: CharacterController2D not found on Player!");
            }
        }
        else
        {
            Debug.Log($"UnlockAbilityTrigger: shouldUnlockAbility is false, skipping ability unlock for {abilityToUnlock}.");
        }

        // 3. UI 动画展示
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);

            // Layer 1: Background
            if (layer1 != null) yield return FadeGameObject(layer1, 0, 1, fadeDuration);
            yield return new WaitForSecondsRealtime(layerDelay);

            // Layer 2: Icon/Decor
            if (layer2 != null) yield return FadeGameObject(layer2, 0, 1, fadeDuration);
            yield return new WaitForSecondsRealtime(layerDelay);

            // Layer 3: Text
            if (layer3 != null) yield return FadeGameObject(layer3, 0, 1, fadeDuration);

            // 4. 等待按键 (防误触延迟)
            yield return new WaitForSecondsRealtime(0.5f);
            
            Debug.Log("UnlockAbilityTrigger: Waiting for key press...");
            while (!Input.anyKeyDown)
            {
                yield return null;
            }

            // 5. 淡出关闭
            Debug.Log("UnlockAbilityTrigger: Key pressed, closing UI...");
            StartCoroutine(FadeGameObject(layer3, 1, 0, fadeDuration));
            StartCoroutine(FadeGameObject(layer2, 1, 0, fadeDuration));
            yield return FadeGameObject(layer1, 1, 0, fadeDuration);

            uiPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("UnlockAbilityTrigger: UI Panel not assigned! Skipping UI sequence.");
        }

        // 6. 恢复游戏
        Time.timeScale = 1f;
        Debug.Log("UnlockAbilityTrigger: Sequence complete. TimeScale restored.");

        // 7. 禁用自身
        gameObject.SetActive(false);
    }

    private IEnumerator FadeGameObject(GameObject obj, float start, float end, float duration)
    {
        if (obj == null) yield break;

        obj.SetActive(true);
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        float timer = 0f;
        cg.alpha = start;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; // 重要：timeScale=0 时必须用 unscaledDeltaTime
            cg.alpha = Mathf.Lerp(start, end, timer / duration);
            yield return null;
        }
        cg.alpha = end;
        
        // 如果是淡出到0，最后禁用物体
        if (Mathf.Approximately(end, 0f))
        {
            obj.SetActive(false);
        }
    }
}
