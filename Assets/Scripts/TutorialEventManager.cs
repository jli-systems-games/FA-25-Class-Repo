using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class TutorialEventManager : MonoBehaviour
{
    [Header("Events Library")]
    [Tooltip("在这里配置所有教程事件")]
    public List<TutorialEvent> events = new List<TutorialEvent>();

    [Header("Arrow Indicator")]
    [Tooltip("教程箭头对象（可选）")]
    public GameObject tutorialArrow;

    [Tooltip("箭头相对于目标的偏移")]
    public Vector3 arrowOffset = new Vector3(0, 50f, 0);

    [Header("Highlight Settings")]
    [Tooltip("高亮效果的颜色")]
    public Color highlightColor = Color.yellow;

    [Tooltip("高亮闪烁速度")]
    public float highlightSpeed = 2f;

    private Dictionary<string, TutorialEvent> eventDictionary;
    private Dictionary<GameObject, HighlightEffect> activeHighlights = new Dictionary<GameObject, HighlightEffect>();

    // 🟢 新增：事件完成回调
    public UnityAction<string> OnEventCompleted;

    // 🟢 新增：当前等待完成的事件
    private string currentWaitingEvent = null;
    private bool isWaitingForCompletion = false;

    void Awake()
    {
        // 构建事件字典方便快速查找
        eventDictionary = new Dictionary<string, TutorialEvent>();
        foreach (var evt in events)
        {
            if (!string.IsNullOrEmpty(evt.eventName))
            {
                eventDictionary[evt.eventName] = evt;
            }
            else
            {
                Debug.LogWarning("发现一个没有名字的TutorialEvent，已跳过");
            }
        }

        // 初始化箭头
        if (tutorialArrow != null)
        {
            tutorialArrow.SetActive(false);
        }
    }

    /// <summary>
    /// 触发指定名称的教程事件
    /// </summary>
    public void TriggerEvent(string eventName)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            TutorialEvent evt = eventDictionary[eventName];

            if (evt.delaySeconds > 0)
            {
                StartCoroutine(TriggerEventDelayed(evt));
            }
            else
            {
                ExecuteEvent(evt);
            }
        }
        else
        {
            Debug.LogWarning($"找不到教程事件: {eventName}");
        }
    }

    /// <summary>
    /// 🟢 开始等待玩家完成某个事件
    /// </summary>
    public void StartWaitingForEvent(string eventName)
    {
        currentWaitingEvent = eventName;
        isWaitingForCompletion = true;
        Debug.Log($"开始等待玩家完成事件: {eventName}");

        // 先触发事件的视觉效果
        TriggerEvent(eventName);
    }

    /// <summary>
    /// 🟢 手动标记事件完成（给外部脚本调用）
    /// </summary>
    public void CompleteEvent(string eventName)
    {
        if (currentWaitingEvent == eventName && isWaitingForCompletion)
        {
            Debug.Log($"✓ 事件完成: {eventName}");
            isWaitingForCompletion = false;
            currentWaitingEvent = null;

            // 触发完成回调
            OnEventCompleted?.Invoke(eventName);
        }
    }

    /// <summary>
    /// 🟢 检查是否正在等待事件完成
    /// </summary>
    public bool IsWaitingForCompletion()
    {
        return isWaitingForCompletion;
    }

    /// <summary>
    /// 🟢 获取当前等待的事件名称
    /// </summary>
    public string GetCurrentWaitingEvent()
    {
        return currentWaitingEvent;
    }

    private IEnumerator TriggerEventDelayed(TutorialEvent evt)
    {
        yield return new WaitForSeconds(evt.delaySeconds);
        ExecuteEvent(evt);
    }

    private void ExecuteEvent(TutorialEvent evt)
    {
        Debug.Log($"执行教程事件: {evt.eventName}");

        // 1. 激活对象
        if (evt.objectsToActivate != null)
        {
            foreach (var obj in evt.objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    Debug.Log($"  → 激活: {obj.name}");
                }
            }
        }

        // 2. 停用对象
        if (evt.objectsToDeactivate != null)
        {
            foreach (var obj in evt.objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    Debug.Log($"  → 停用: {obj.name}");
                }
            }
        }

        // 3. 高亮对象
        if (evt.objectToHighlight != null)
        {
            HighlightObject(evt.objectToHighlight);
        }

        // 4. 显示箭头
        if (evt.arrowTarget != null)
        {
            ShowArrow(evt.arrowTarget);
        }

        // 5. 播放动画
        if (evt.animator != null && !string.IsNullOrEmpty(evt.animationToPlay))
        {
            evt.animator.Play(evt.animationToPlay);
            Debug.Log($"  → 播放动画: {evt.animationToPlay}");
        }

        // 6. 播放音效
        if (evt.soundEffect != null)
        {
            AudioSource.PlayClipAtPoint(evt.soundEffect, Camera.main.transform.position, evt.soundVolume);
            Debug.Log($"  → 播放音效: {evt.soundEffect.name}");
        }
    }

    /// <summary>
    /// 高亮一个对象
    /// </summary>
    private void HighlightObject(GameObject target)
    {
        if (target == null) return;

        // 如果对象已经有高亮效果，先移除
        if (activeHighlights.ContainsKey(target))
        {
            StopHighlight(target);
        }

        // 添加或获取HighlightEffect组件
        HighlightEffect highlight = target.GetComponent<HighlightEffect>();
        if (highlight == null)
        {
            highlight = target.AddComponent<HighlightEffect>();
        }

        highlight.highlightColor = highlightColor;
        highlight.pulseSpeed = highlightSpeed;
        highlight.StartHighlight();

        activeHighlights[target] = highlight;
        Debug.Log($"  → 高亮: {target.name}");
    }

    /// <summary>
    /// 停止高亮一个对象
    /// </summary>
    public void StopHighlight(GameObject target)
    {
        if (target != null && activeHighlights.ContainsKey(target))
        {
            HighlightEffect highlight = activeHighlights[target];
            if (highlight != null)
            {
                highlight.StopHighlight();
            }
            activeHighlights.Remove(target);
        }
    }

    /// <summary>
    /// 停止所有高亮
    /// </summary>
    public void StopAllHighlights()
    {
        foreach (var kvp in activeHighlights)
        {
            if (kvp.Value != null)
            {
                kvp.Value.StopHighlight();
            }
        }
        activeHighlights.Clear();
    }

    /// <summary>
    /// 显示箭头指向目标
    /// </summary>
    private void ShowArrow(GameObject target)
    {
        if (tutorialArrow == null || target == null) return;

        tutorialArrow.SetActive(true);
        tutorialArrow.transform.position = target.transform.position + arrowOffset;

        Debug.Log($"  → 箭头指向: {target.name}");
    }

    /// <summary>
    /// 隐藏箭头
    /// </summary>
    public void HideArrow()
    {
        if (tutorialArrow != null)
        {
            tutorialArrow.SetActive(false);
        }
    }

    /// <summary>
    /// 通过对象名查找并激活
    /// </summary>
    public void ActivateObjectByName(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj != null)
        {
            obj.SetActive(true);
            Debug.Log($"激活对象: {objectName}");
        }
        else
        {
            Debug.LogWarning($"找不到对象: {objectName}");
        }
    }

    /// <summary>
    /// 通过对象名查找并停用
    /// </summary>
    public void DeactivateObjectByName(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj != null)
        {
            obj.SetActive(false);
            Debug.Log($"停用对象: {objectName}");
        }
    }
}

/// <summary>
/// 简单的高亮效果组件
/// </summary>
public class HighlightEffect : MonoBehaviour
{
    public Color highlightColor = Color.yellow;
    public float pulseSpeed = 2f;

    private SpriteRenderer spriteRenderer;
    private Image uiImage;
    private Color originalColor;
    private bool isHighlighting = false;
    private float pulseTimer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiImage = GetComponent<Image>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else if (uiImage != null)
        {
            originalColor = uiImage.color;
        }
    }

    public void StartHighlight()
    {
        isHighlighting = true;
        pulseTimer = 0f;
    }

    public void StopHighlight()
    {
        isHighlighting = false;

        // 恢复原始颜色
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        else if (uiImage != null)
        {
            uiImage.color = originalColor;
        }
    }

    void Update()
    {
        if (!isHighlighting) return;

        pulseTimer += Time.deltaTime * pulseSpeed;
        float pulse = Mathf.PingPong(pulseTimer, 1f);
        Color currentColor = Color.Lerp(originalColor, highlightColor, pulse);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = currentColor;
        }
        else if (uiImage != null)
        {
            uiImage.color = currentColor;
        }
    }
}