using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Demo 结束触发器 - 当玩家碰到后显示感谢面板
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DemoEndTrigger : MonoBehaviour
{
    [Header("面板设置")]
    [Tooltip("要显示的感谢面板")]
    public GameObject thankYouPanel;

    [Tooltip("返回主菜单按钮（可选）")]
    public Button returnToMenuButton;

    [Header("淡入效果设置")]
    [Tooltip("淡入持续时间（秒）")]
    public float fadeInDuration = 1.5f;

    [Tooltip("面板完全显示后是否暂停游戏")]
    public bool pauseGameAfterShow = false;

    [Tooltip("面板显示后是否禁用玩家控制")]
    public bool disablePlayerControl = true;

    [Tooltip("是否只触发一次")]
    public bool triggerOnce = true;

    [Header("返回主菜单设置")]
    [Tooltip("主菜单场景名称")]
    public string mainMenuSceneName = "Main";

    [Tooltip("白屏淡出时间（秒）")]
    public float fadeOutDuration = 1f;

    [Tooltip("淡出颜色")]
    public Color fadeOutColor = Color.white;

    private bool hasTriggered = false;
    private CanvasGroup canvasGroup;

    void Start()
    {
        // 确保 Collider 是触发器
        GetComponent<Collider2D>().isTrigger = true;

        // 初始化面板
        if (thankYouPanel != null)
        {
            // 添加 CanvasGroup 组件（如果没有）
            canvasGroup = thankYouPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = thankYouPanel.AddComponent<CanvasGroup>();
            }

            // 初始时隐藏面板
            canvasGroup.alpha = 0f;
            thankYouPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DemoEndTrigger: 未设置 Thank You Panel！");
        }

        // 绑定返回主菜单按钮
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.AddListener(OnReturnToMenu);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是玩家
        if (other.CompareTag("Player"))
        {
            // 如果只触发一次且已经触发过，则返回
            if (triggerOnce && hasTriggered)
                return;

            hasTriggered = true;
            ShowThankYouPanel(other.gameObject);
        }
    }

    /// <summary>
    /// 显示感谢面板
    /// </summary>
    void ShowThankYouPanel(GameObject player)
    {
        // 禁用玩家控制（但不暂停游戏）
        if (disablePlayerControl && player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement) playerMovement.enabled = false;

            CharacterController2D controller = player.GetComponent<CharacterController2D>();
            if (controller) controller.enabled = false;

            // 停止玩家移动
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        // 显示鼠标（用于与UI交互）
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.ShowCursor();
        }
        else
        {
            // 如果没有 CursorManager，直接显示鼠标
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (thankYouPanel != null)
        {
            thankYouPanel.SetActive(true);
            StartCoroutine(FadeInPanel());
        }
    }

    /// <summary>
    /// 淡入效果协程
    /// </summary>
    IEnumerator FadeInPanel()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            yield return null;
        }

        // 确保完全显示
        canvasGroup.alpha = 1f;

        // 如果设置了暂停游戏
        if (pauseGameAfterShow)
        {
            Time.timeScale = 0f;
        }
        
        // 面板会停留在屏幕上，玩家无法移动，游戏继续运行（背景动画等）
    }

    /// <summary>
    /// 隐藏面板（可以通过按钮调用）
    /// </summary>
    public void HidePanel()
    {
        StartCoroutine(FadeOutPanel());
    }

    /// <summary>
    /// 淡出效果协程
    /// </summary>
    IEnumerator FadeOutPanel()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        thankYouPanel.SetActive(false);

        // 恢复游戏时间
        if (pauseGameAfterShow)
        {
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// 重置触发状态（用于测试）
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    /// <summary>
    /// 返回主菜单（按钮调用）
    /// </summary>
    public void OnReturnToMenu()
    {
        StartCoroutine(ReturnToMenuWithFade());
    }

    /// <summary>
    /// 白屏淡出并返回主菜单
    /// </summary>
    IEnumerator ReturnToMenuWithFade()
    {
        // 创建白色淡出遮罩
        GameObject fadeCanvas = CreateWhiteFadeOut();
        CanvasGroup fadeGroup = fadeCanvas.GetComponentInChildren<CanvasGroup>();

        // 淡出到白色
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            yield return null;
        }

        fadeGroup.alpha = 1f;

        // 短暂延迟
        yield return new WaitForSecondsRealtime(0.1f);

        // 恢复时间缩放
        Time.timeScale = 1f;

        // 加载主菜单
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// 创建白色淡出遮罩
    /// </summary>
    GameObject CreateWhiteFadeOut()
    {
        // 创建 Canvas
        GameObject canvasObject = new GameObject("MenuFadeCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000; // 确保在最上层
        DontDestroyOnLoad(canvasObject);

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 创建白色遮罩
        GameObject fadePanel = new GameObject("WhiteFadeOut");
        fadePanel.transform.SetParent(canvasObject.transform, false);

        RectTransform rectTransform = fadePanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        Image image = fadePanel.AddComponent<Image>();
        image.color = fadeOutColor;

        CanvasGroup canvasGroup = fadePanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        // 添加自动淡出脚本，确保在新场景加载后自动淡出并销毁
        FadeOutAndDestroy fadeOutScript = canvasObject.AddComponent<FadeOutAndDestroy>();
        fadeOutScript.duration = fadeOutDuration;

        return canvasObject;
    }

    /// <summary>
    /// 在编辑器中显示触发器范围
    /// </summary>
    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.5f); // 青色半透明
            
            if (col is BoxCollider2D)
            {
                BoxCollider2D boxCol = col as BoxCollider2D;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.offset, boxCol.size);
            }
            else if (col is CircleCollider2D)
            {
                CircleCollider2D circleCol = col as CircleCollider2D;
                Gizmos.DrawSphere(transform.position + (Vector3)circleCol.offset, circleCol.radius);
            }
        }
    }
}

