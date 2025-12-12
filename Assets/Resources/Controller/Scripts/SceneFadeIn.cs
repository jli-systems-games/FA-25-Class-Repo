using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 场景入场效果 - 从白色渐渐变透明
/// 将此脚本添加到场景中的任意对象上（推荐添加到Canvas或空对象）
/// </summary>
public class SceneFadeIn : MonoBehaviour
{
    [Header("淡入效果设置")]
    [Tooltip("淡入持续时间（秒）")]
    public float fadeInDuration = 1f;

    [Tooltip("淡入颜色")]
    public Color fadeColor = Color.white;

    [Tooltip("场景开始时自动播放")]
    public bool playOnStart = true;

    [Tooltip("延迟开始时间（秒）")]
    public float startDelay = 0f;

    private GameObject fadeObject;
    private CanvasGroup canvasGroup;

    void Start()
    {
        if (playOnStart)
        {
            StartCoroutine(FadeInSequence());
        }
    }

    /// <summary>
    /// 淡入序列
    /// </summary>
    IEnumerator FadeInSequence()
    {
        // 延迟
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }

        // 检查是否有现有的淡出遮罩（从主菜单或死亡带来的）
        GameObject existingFade = GameObject.Find("FadeCanvas");
        if (existingFade == null)
        {
            existingFade = GameObject.Find("DeathFadeCanvas");
        }

        if (existingFade != null)
        {
            // 使用现有的遮罩
            fadeObject = existingFade;
            canvasGroup = existingFade.GetComponentInChildren<CanvasGroup>();
            
            if (canvasGroup == null)
            {
                // 如果找不到CanvasGroup，尝试直接在GameObject上查找
                CanvasGroup[] groups = existingFade.GetComponentsInChildren<CanvasGroup>();
                if (groups.Length > 0)
                {
                    canvasGroup = groups[0];
                }
            }
        }
        else
        {
            // 创建新的白色遮罩
            CreateFadeOverlay();
        }

        // 确保遮罩完全不透明
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        // 淡出遮罩（从白色到透明）
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
            }
            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        // 销毁遮罩
        if (fadeObject != null)
        {
            Destroy(fadeObject);
        }
    }

    /// <summary>
    /// 创建淡入遮罩
    /// </summary>
    void CreateFadeOverlay()
    {
        // 创建 Canvas
        GameObject canvasObject = new GameObject("SceneFadeCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // 确保在最上层

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 创建遮罩面板
        fadeObject = new GameObject("FadePanel");
        fadeObject.transform.SetParent(canvasObject.transform, false);

        RectTransform rectTransform = fadeObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        Image image = fadeObject.AddComponent<Image>();
        image.color = fadeColor;

        canvasGroup = fadeObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f; // 初始完全不透明
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        // 确保在淡入完成后销毁整个Canvas
        fadeObject = canvasObject;
    }

    /// <summary>
    /// 手动触发淡入（可通过其他脚本调用）
    /// </summary>
    public void TriggerFadeIn()
    {
        StartCoroutine(FadeInSequence());
    }

    /// <summary>
    /// 手动触发淡入（带自定义时间）
    /// </summary>
    public void TriggerFadeIn(float duration)
    {
        fadeInDuration = duration;
        StartCoroutine(FadeInSequence());
    }
}

