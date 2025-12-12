using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GreatAchievement.Systems;

/// <summary>
/// 死亡区域 - 玩家触碰后暂停、变白、重生
/// </summary>
public class KillZone : MonoBehaviour
{
    [Header("死亡效果设置")]
    [Tooltip("变白持续时间（秒）")]
    public float fadeToWhiteDuration = 0.5f;
    
    [Tooltip("暂停时间（秒）")]
    public float pauseDuration = 0.3f;
    
    [Tooltip("是否禁用玩家控制")]
    public bool disablePlayerControl = true;

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayerDeathSequence(col.gameObject));
        }
        else if (col.gameObject.tag != "Player")
        {
            Destroy(col.gameObject);
        }
    }

    /// <summary>
    /// 玩家死亡序列
    /// </summary>
    IEnumerator PlayerDeathSequence(GameObject player)
    {
        // 1. 禁用玩家控制
        if (disablePlayerControl)
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
                rb.simulated = false; // 禁用物理模拟
            }
        }

        // 2. 短暂暂停
        yield return new WaitForSecondsRealtime(pauseDuration);

        // 3. 创建白色淡入遮罩
        GameObject fadeObject = CreateWhiteFade();
        CanvasGroup fadeGroup = fadeObject.GetComponent<CanvasGroup>();

        // 4. 淡入到白色
        float elapsed = 0f;
        while (elapsed < fadeToWhiteDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeToWhiteDuration);
            yield return null;
        }

        fadeGroup.alpha = 1f;

        // 5. 短暂延迟
        yield return new WaitForSecondsRealtime(0.1f);

        // 6. 重新加载场景（使用 GameManager 逻辑）
        Time.timeScale = 1f; // 确保时间恢复正常
        
        // 销毁遮罩，因为 GameManager 可能会有自己的遮罩处理，或者在加载新场景后遮罩需要处理
        // 注意：DontDestroyOnLoad的物体需要手动销毁，或者让它在一段时间后自动销毁
        Destroy(fadeObject, 2f); // 2秒后销毁遮罩，给新场景加载留点时间

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RespawnPlayer();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /// <summary>
    /// 创建白色淡入遮罩
    /// </summary>
    GameObject CreateWhiteFade()
    {
        // 创建 Canvas
        GameObject canvasObject = new GameObject("DeathFadeCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // 确保在最上层
        DontDestroyOnLoad(canvasObject);

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 创建白色遮罩
        GameObject fadePanel = new GameObject("WhiteFade");
        fadePanel.transform.SetParent(canvasObject.transform, false);

        RectTransform rectTransform = fadePanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        Image image = fadePanel.AddComponent<Image>();
        image.color = Color.white;

        CanvasGroup canvasGroup = fadePanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        return fadePanel;
    }
}

