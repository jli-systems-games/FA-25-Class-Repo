using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GreatAchievement.Systems;

public class SceneTransition : MonoBehaviour
{
    [Tooltip("要切换到的场景名称")]
    public string targetSceneName;
    
    [Tooltip("目标场景中的生成点ID")]
    public string targetSpawnPointID;

    [Header("Transition Settings")]
    [Tooltip("淡入淡出时间")]
    public float fadeDuration = 0.5f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(TransitionRoutine(other.gameObject));
        }
    }

    private IEnumerator TransitionRoutine(GameObject player)
    {
        // 1. 冻结游戏画面 / 玩家操作
        Time.timeScale = 0f; // 如果你想让游戏逻辑完全暂停（除了协程）
        // 或者只禁用玩家控制:
        // player.GetComponent<PlayerMovement>().enabled = false;
        // player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // 2. 创建淡出遮罩 (变白)
        GameObject fadeCanvas = CreateFadeCanvas();
        CanvasGroup canvasGroup = fadeCanvas.GetComponentInChildren<CanvasGroup>();
        
        float timer = 0f;
        while (timer < fadeDuration)
        {
            // 使用 unscaledDeltaTime 因为 timeScale 可能是 0
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // 3. 设置目标并加载场景
        GameManager.Instance.targetSpawnID = targetSpawnPointID;
        
        // 恢复时间流逝，以便新场景正常加载和运行
        Time.timeScale = 1f; 
        
        // 注意：GameManager LoadScene 是同步的，这里使用SceneManager.LoadSceneAsync可能更平滑
        // 这里直接调用 GameManager，让它去处理加载
        GameManager.Instance.LoadScene(targetSceneName);
    }

    private GameObject CreateFadeCanvas()
    {
        GameObject canvasObj = new GameObject("TransitionCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        DontDestroyOnLoad(canvasObj); // 保持到下一个场景以处理淡入

        GameObject panel = new GameObject("FadePanel");
        panel.transform.SetParent(canvasObj.transform, false);
        
        Image img = panel.AddComponent<Image>();
        img.color = Color.white; // 白色遮罩
        
        // 铺满屏幕
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        CanvasGroup cg = panel.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        
        // 确保新场景加载后自动销毁
        FadeOutAndDestroy script = canvasObj.AddComponent<FadeOutAndDestroy>();
        script.duration = fadeDuration;

        return canvasObj;
    }
}

// 辅助脚本：在新场景淡出并销毁
public class FadeOutAndDestroy : MonoBehaviour
{
    public float duration = 0.5f;

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        // 等待一帧，确保新场景完全初始化
        yield return null;

        CanvasGroup cg = GetComponentInChildren<CanvasGroup>();
        if (cg != null)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime; 
                cg.alpha = Mathf.Lerp(1f, 0f, timer / duration);
                yield return null;
            }
            cg.alpha = 0f;
        }
        
        // 销毁自身
        Destroy(gameObject);
    }
}

