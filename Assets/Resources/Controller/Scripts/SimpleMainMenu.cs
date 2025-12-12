using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 简化版主菜单 - 合并了 UIScreen 的场景切换功能
/// 不依赖复杂的继承结构
/// </summary>
public class SimpleMainMenu : MonoBehaviour
{
    [Header("按钮设置")]
    public Button startButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("场景设置")]
    public string firstLevelScene = "Level1";

    [Header("面板设置")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    [Header("淡入淡出设置")]
    [Tooltip("淡出时间（秒）")]
    public float fadeOutDuration = 1f;
    
    [Tooltip("淡出颜色")]
    public Color fadeColor = Color.white;

    [Header("音效设置（可选）")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;

    private bool isTransitioning = false;

    void Awake()
    {
        // 确保初始状态
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (optionsPanel) optionsPanel.SetActive(false);

        // 绑定按钮事件
        if (startButton)
        {
            startButton.onClick.AddListener(OnStartGame);
        }
        if (optionsButton)
        {
            optionsButton.onClick.AddListener(OnOpenOptions);
        }
        if (quitButton)
        {
            quitButton.onClick.AddListener(OnQuit);
        }

        // 确保时间正常
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void OnStartGame()
    {
        if (isTransitioning) return;
        
        PlaySound(buttonClickSound);
        StartCoroutine(LoadSceneWithFade(firstLevelScene));
    }

    /// <summary>
    /// 打开设置
    /// </summary>
    public void OnOpenOptions()
    {
        PlaySound(buttonClickSound);
        
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void OnBackToMain()
    {
        PlaySound(buttonClickSound);
        
        if (optionsPanel) optionsPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void OnQuit()
    {
        PlaySound(buttonClickSound);
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    /// <summary>
    /// 按钮悬停音效
    /// </summary>
    public void OnButtonHover()
    {
        PlaySound(buttonHoverSound);
    }

    /// <summary>
    /// 加载场景（带淡出效果）
    /// </summary>
    IEnumerator LoadSceneWithFade(string sceneName)
    {
        isTransitioning = true;

        // 创建白色遮罩
        GameObject fadeCanvas = CreateFadeOverlay();
        CanvasGroup canvasGroup = fadeCanvas.GetComponentInChildren<CanvasGroup>();

        // 淡出到白色
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // 短暂延迟
        yield return new WaitForSecondsRealtime(0.1f);

        // 加载场景（遮罩会在下一帧被SceneFadeIn处理）
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 创建淡出遮罩
    /// </summary>
    GameObject CreateFadeOverlay()
    {
        // 创建 Canvas
        GameObject canvasObject = new GameObject("FadeCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        DontDestroyOnLoad(canvasObject);

        // 创建遮罩面板
        GameObject fadePanel = new GameObject("FadePanel");
        fadePanel.transform.SetParent(canvasObject.transform, false);

        RectTransform rectTransform = fadePanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        Image image = fadePanel.AddComponent<Image>();
        image.color = fadeColor;

        CanvasGroup canvasGroup = fadePanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        return canvasObject;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

