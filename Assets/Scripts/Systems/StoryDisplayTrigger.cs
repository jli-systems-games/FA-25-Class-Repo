using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryDisplayTrigger : MonoBehaviour
{
    public enum TriggerMode
    {
        [Tooltip("玩家首次进入场景时触发（再次进入不重复）")]
        FirstTimeEnterScene,
        [Tooltip("玩家碰到Collider时触发")]
        OnColliderEnter,
        [Tooltip("两者皆可触发")]
        Both
    }

    [Header("触发设置")]
    [Tooltip("触发模式")]
    public TriggerMode triggerMode = TriggerMode.OnColliderEnter;
    
    [Tooltip("唯一标识符，用于记录是否已触发过（每个触发器需要不同的ID）")]
    public string triggerId = "Story_Default";
    
    [Tooltip("是否只能触发一次")]
    public bool triggerOnlyOnce = true;

    [Header("UI设置")]
    [Tooltip("故事显示的Canvas Panel")]
    public GameObject storyPanel;
    
    [Tooltip("按顺序显示的GameObject列表")]
    public List<GameObject> storyElements = new List<GameObject>();
    
    [Tooltip("每个元素淡入的持续时间")]
    public float fadeDuration = 0.5f;
    
    [Tooltip("元素之间出现的间隔时间")]
    public float elementDelay = 0.3f;
    
    [Tooltip("全部显示后等待多久才能按键退出（防误触）")]
    public float exitDelay = 0.5f;

    private bool hasTriggered = false;
    private bool isDisplaying = false;
    private Coroutine displayCoroutine;

    // 用于记录是否已触发过的PlayerPrefs Key前缀
    private const string PREFS_PREFIX = "StoryTrigger_";

    private void Start()
    {
        // 确保UI初始状态是隐藏的
        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }
        
        // 隐藏并重置所有故事元素
        foreach (var element in storyElements)
        {
            if (element != null)
            {
                element.SetActive(false);
                ResetCanvasGroup(element);
            }
        }

        // 检查是否已经触发过
        if (triggerOnlyOnce && PlayerPrefs.GetInt(PREFS_PREFIX + triggerId, 0) == 1)
        {
            hasTriggered = true;
            Debug.Log($"StoryDisplayTrigger [{triggerId}]: 已经触发过，跳过。");
            return;
        }

        // 如果是首次进入场景触发模式
        if (triggerMode == TriggerMode.FirstTimeEnterScene || triggerMode == TriggerMode.Both)
        {
            // 使用协程延迟一帧触发，确保场景完全加载
            StartCoroutine(CheckFirstTimeEnter());
        }
    }

    private IEnumerator CheckFirstTimeEnter()
    {
        // 等待一帧，确保场景完全初始化
        yield return null;
        
        if (!hasTriggered && !isDisplaying)
        {
            Debug.Log($"StoryDisplayTrigger [{triggerId}]: 首次进入场景，触发故事显示。");
            TriggerStory();
        }
    }

    private void ResetCanvasGroup(GameObject obj)
    {
        if (obj == null) return;
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只有碰撞触发模式才响应
        if (triggerMode != TriggerMode.OnColliderEnter && triggerMode != TriggerMode.Both)
            return;

        if (hasTriggered || isDisplaying)
            return;

        // 检测玩家
        CharacterController2D player = other.GetComponentInParent<CharacterController2D>();
        
        if (player != null && player.playerIdentifier == "Player")
        {
            Debug.Log($"StoryDisplayTrigger [{triggerId}]: 玩家碰到触发器，开始显示故事。");
            TriggerStory();
        }
    }

    /// <summary>
    /// 公共方法，可以从外部调用来触发故事显示
    /// </summary>
    public void TriggerStory()
    {
        if (hasTriggered || isDisplaying)
        {
            Debug.Log($"StoryDisplayTrigger [{triggerId}]: 已触发或正在显示，忽略。");
            return;
        }

        displayCoroutine = StartCoroutine(DisplayStorySequence());
    }

    private IEnumerator DisplayStorySequence()
    {
        isDisplaying = true;

        // 1. 冻结游戏
        Time.timeScale = 0f;

        // 2. 显示故事面板
        if (storyPanel != null)
        {
            storyPanel.SetActive(true);

            // 3. 按顺序显示每个元素
            foreach (var element in storyElements)
            {
                if (element != null)
                {
                    yield return FadeGameObject(element, 0f, 1f, fadeDuration);
                    yield return new WaitForSecondsRealtime(elementDelay);
                }
            }

            // 4. 等待防误触延迟
            yield return new WaitForSecondsRealtime(exitDelay);

            // 5. 等待任意按键（使用特殊方式防止按键传递到游戏）
            Debug.Log($"StoryDisplayTrigger [{triggerId}]: 等待按键退出...");
            
            // 等待所有按键释放
            while (Input.anyKey)
            {
                yield return null;
            }
            
            // 等待新的按键按下
            while (!Input.anyKeyDown)
            {
                yield return null;
            }

            // 6. 记录按下的按键，等待释放后再继续
            Debug.Log($"StoryDisplayTrigger [{triggerId}]: 检测到按键，等待释放...");
            
            // 等待按键释放
            while (Input.anyKey)
            {
                yield return null;
            }

            // 7. 淡出所有元素
            Debug.Log($"StoryDisplayTrigger [{triggerId}]: 关闭故事面板...");
            
            // 同时淡出所有元素
            List<Coroutine> fadeOutCoroutines = new List<Coroutine>();
            foreach (var element in storyElements)
            {
                if (element != null && element.activeSelf)
                {
                    fadeOutCoroutines.Add(StartCoroutine(FadeGameObject(element, 1f, 0f, fadeDuration)));
                }
            }

            // 等待最后一个淡出完成
            yield return new WaitForSecondsRealtime(fadeDuration);

            // 8. 隐藏面板
            storyPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"StoryDisplayTrigger [{triggerId}]: 未设置 storyPanel！");
        }

        // 9. 标记已触发
        hasTriggered = true;
        if (triggerOnlyOnce)
        {
            PlayerPrefs.SetInt(PREFS_PREFIX + triggerId, 1);
            PlayerPrefs.Save();
        }

        // 10. 额外等待一帧，确保按键状态完全重置
        yield return null;

        // 11. 恢复游戏
        Time.timeScale = 1f;
        isDisplaying = false;

        Debug.Log($"StoryDisplayTrigger [{triggerId}]: 故事显示完成。");

        // 12. 可选：禁用触发器
        if (triggerOnlyOnce)
        {
            gameObject.SetActive(false);
        }
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
            timer += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, end, timer / duration);
            yield return null;
        }
        cg.alpha = end;

        // 如果是淡出到0，禁用物体
        if (Mathf.Approximately(end, 0f))
        {
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// 重置触发器状态（用于调试或特殊需求）
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        PlayerPrefs.DeleteKey(PREFS_PREFIX + triggerId);
        PlayerPrefs.Save();
        gameObject.SetActive(true);
        Debug.Log($"StoryDisplayTrigger [{triggerId}]: 触发器已重置。");
    }

    /// <summary>
    /// 静态方法：重置所有故事触发器
    /// </summary>
    public static void ResetAllStoryTriggers()
    {
        // 查找所有以特定前缀开头的PlayerPrefs（需要手动管理）
        Debug.Log("StoryDisplayTrigger: 请在GameManager中管理所有触发器ID的重置。");
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 在编辑器中确保triggerId不为空
        if (string.IsNullOrEmpty(triggerId))
        {
            triggerId = "Story_" + gameObject.name + "_" + GetInstanceID();
        }
    }
#endif
}

