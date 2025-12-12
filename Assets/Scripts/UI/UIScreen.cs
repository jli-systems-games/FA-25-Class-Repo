using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GreatAchievement.Systems;

namespace GreatAchievement.UI
{
    /// <summary>
    /// UI屏幕基类 - 处理淡入淡出、场景加载、音频反馈
    /// 参考自Metroidvania项目，适配《伟大成就》
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour
    {
        public enum Transition { none, fade, colorFade, colorFadeAndFade }
        
        [Header("场景入场动画")]
        public Transition sceneEntrance = Transition.fade;
        public float fadeTime = 1f;
        public Color fadeColor = Color.white; // 改为白色
        [Range(0, 1)] public float crossFade = 0.5f;
        public float updateFrequency = 0.05f;

        [System.Flags] 
        public enum TimeScaleMode { affectedByGlobalTimeScale = 1, affectedByPause = 2 }
        
        [Header("时间缩放模式")]
        public TimeScaleMode timeScaleMode = TimeScaleMode.affectedByPause;

        [Header("UI音频反馈")]
        public UIAudioFeedbackConfig feedbackConfig;
        
        protected CanvasGroup group;
        protected float originalAlpha = 1f;
        protected Coroutine currentAnimation;
        protected bool isAnimating = false; // 子类可访问
        protected AudioSource audioSource;

        public bool IsAnimating() { return isAnimating; }

        public const int COLOR_FADE_PRIORITY = 1000;

        protected virtual void Awake()
        {
            // 查找音频源
            audioSource = GetComponentInParent<AudioSource>();
            if (!audioSource && feedbackConfig)
            {
                Debug.LogWarning($"未找到AudioSource！{name}将没有音频反馈！");
            }

            group = GetComponent<CanvasGroup>();
            originalAlpha = group.alpha;

            // 根据设置播放入场动画
            switch (sceneEntrance)
            {
                case Transition.fade:
                    StartCoroutine(Fade(fadeTime, 1));
                    break;
                case Transition.colorFade:
                    StartCoroutine(FadeTo(fadeColor, -1, fadeTime));
                    break;
                case Transition.colorFadeAndFade:
                    StartCoroutine(ColorFadeAndFade(fadeTime, 1, fadeColor));
                    break;
            }
        }

        /// <summary>
        /// 获取当前时间缩放
        /// </summary>
        public float GetTimeScale()
        {
            if ((timeScaleMode & TimeScaleMode.affectedByPause) > 0)
            {
                if (Systems.GameManager.Instance && Systems.GameManager.Instance.isPaused) 
                    return 0f;
            }
            if ((timeScaleMode & TimeScaleMode.affectedByGlobalTimeScale) > 0) 
                return Time.timeScale;
            return 1f;
        }

        /// <summary>
        /// 播放音频反馈
        /// </summary>
        public void PlayAudioFeedback(string type)
        {
            if (!audioSource || !feedbackConfig) return;

            AudioClip sfx = feedbackConfig.GetSound(type);
            if (sfx) audioSource.PlayOneShot(sfx);
        }

        /// <summary>
        /// 停用所有其他UI屏幕
        /// </summary>
        public void DeactivateAll(float fadeDuration = -1)
        {
            UIScreen[] all = FindObjectsByType<UIScreen>(FindObjectsSortMode.None);
            foreach (UIScreen u in all)
            {
                if (u == this) continue;
                u.Deactivate(fadeDuration);
            }
        }

        /// <summary>
        /// 激活此UI屏幕
        /// </summary>
        public virtual void Activate(bool exclusive = false)
        {
            if (isAnimating) return;

            float activationDelay = 0;
            if (exclusive)
            {
                if (crossFade > 0)
                {
                    DeactivateAll(fadeTime * crossFade);
                    activationDelay = 0;
                }
                else
                {
                    activationDelay = fadeTime / 2;
                    DeactivateAll(activationDelay);
                }
            }

            gameObject.SetActive(true);
            if (group) group.alpha = 0;

            StartCoroutine(ActivateCoroutine(activationDelay));
        }

        /// <summary>
        /// 激活协程（带延迟）
        /// </summary>
        protected virtual IEnumerator ActivateCoroutine(float delay)
        {
            if (isAnimating) yield break;
            isAnimating = true;

            if (delay > 0)
            {
                WaitForSecondsRealtime w = new WaitForSecondsRealtime(updateFrequency);
                while (delay > 0)
                {
                    yield return w;
                    float timeScale = GetTimeScale();
                    if (timeScale <= 0) continue;
                    delay -= w.waitTime * timeScale;
                }
            }

            gameObject.SetActive(true);
            if (group)
            {
                group.alpha = 0;
                StartCoroutine(Fade(fadeTime, 1));
            }

            isAnimating = false;
        }

        /// <summary>
        /// 停用此UI屏幕
        /// </summary>
        public virtual void Deactivate(float fadeDuration = -1)
        {
            if (fadeDuration < 0) fadeDuration = fadeTime;
            if (group)
            {
                group.alpha = originalAlpha;
                StartCoroutine(Fade(fadeDuration, -1));
            }
        }

        /// <summary>
        /// 淡入淡出协程
        /// </summary>
        protected virtual IEnumerator Fade(float duration, int direction = 1)
        {
            WaitForSecondsRealtime w = new WaitForSecondsRealtime(updateFrequency);

            while (isAnimating) yield return w;

            isAnimating = true;
            float currentDuration = duration;

            if (group)
            {
                group.alpha = direction > 0 ? 0 : originalAlpha;
            }
            gameObject.SetActive(true);

            while (currentDuration > 0)
            {
                yield return w;
                float timeScale = GetTimeScale();
                if (timeScale <= 0) continue;
                currentDuration -= w.waitTime * timeScale;
                float ratio = currentDuration / duration;
                if (group) group.alpha = (direction > 0 ? 1f - ratio : ratio) * originalAlpha;
            }

            if (group)
            {
                group.alpha = direction > 0 ? originalAlpha : 0;
                if (direction < 0) gameObject.SetActive(false);
            }

            isAnimating = false;
        }

        /// <summary>
        /// 淡入到某个颜色
        /// </summary>
        public static IEnumerator FadeTo(Color color, int direction = 1, float duration = 0.5f, TimeScaleMode timeScaleMode = 0)
        {
            GameObject go = new GameObject("Fader (Temp)");
            RectTransform r = go.AddComponent<RectTransform>();
            Canvas c = go.AddComponent<Canvas>();
            Image img = go.AddComponent<Image>();
            CanvasGroup g = go.AddComponent<CanvasGroup>();
            UIScreen u = go.AddComponent<UIScreen>();
            u.timeScaleMode = timeScaleMode;

            img.color = color;
            g.alpha = direction > 0 ? 0 : 1;
            g.blocksRaycasts = g.interactable = false;
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.sortingOrder = COLOR_FADE_PRIORITY;

            yield return u.Fade(duration, direction);
            Destroy(go, u.updateFrequency);
        }

        /// <summary>
        /// 颜色淡入+淡入组合
        /// </summary>
        protected virtual IEnumerator ColorFadeAndFade(float fadeTime, int direction, Color color)
        {
            if (group) group.alpha = 0;
            yield return FadeTo(color, -direction, fadeTime);
            yield return Fade(fadeTime, direction);
        }

        // 单参数版本供Button使用
        public virtual void LoadScene(string sceneName) { LoadScene(sceneName, -1); }
        public virtual void LoadScene(int buildIndex) { LoadScene(buildIndex, -1); }

        /// <summary>
        /// 加载场景（带淡入淡出）
        /// </summary>
        public virtual void LoadScene(string sceneName, float fadeDuration = -1, float newTimeScale = 1f)
        {
            int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
            if (buildIndex < 0)
            {
                Debug.LogError($"场景 {sceneName} 未在Build Settings中！");
                return;
            }
            LoadScene(buildIndex, fadeDuration, newTimeScale);
        }

        public virtual void LoadScene(int buildIndex, float fadeDuration = -1, float newTimeScale = 1f)
        {
            StartCoroutine(LoadSceneCoroutine(buildIndex, LoadSceneMode.Single, fadeDuration, newTimeScale));
        }

        /// <summary>
        /// 加载场景协程
        /// </summary>
        protected virtual IEnumerator LoadSceneCoroutine(int buildIndex, LoadSceneMode loadSceneMode, float fadeDuration = -1, float newTimeScale = 1f)
        {
            if (fadeDuration < 0) fadeDuration = fadeTime;

            // 淡入到黑色
            yield return FadeTo(fadeColor, 1, fadeDuration, 0);

            Time.timeScale = Mathf.Max(0, newTimeScale);
            SceneManager.LoadScene(buildIndex, loadSceneMode);
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public virtual void Quit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}

