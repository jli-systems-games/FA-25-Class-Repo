using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GreatAchievement.UI
{
    /// <summary>
    /// UI按钮音频组件
    /// 附加到Button上自动播放音效
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("音频反馈")]
        public UIAudioFeedbackConfig feedbackConfig;
        private AudioSource audioSource;

        [Header("事件音效")]
        public bool playOnClick = true;
        public bool playOnHover = true;
        public bool playOnDown = false;
        public bool playOnUp = false;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            
            // 查找AudioSource
            audioSource = GetComponentInParent<AudioSource>();
            if (!audioSource)
            {
                // 如果没有，尝试从Canvas查找
                Canvas canvas = GetComponentInParent<Canvas>();
                if (canvas) audioSource = canvas.GetComponent<AudioSource>();
            }

            // 绑定点击事件
            if (button && playOnClick)
            {
                button.onClick.AddListener(() => PlaySound("pointerClick"));
            }
        }

        /// <summary>
        /// 鼠标进入
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (playOnHover && button && button.interactable)
            {
                PlaySound("pointerEnter");
            }
        }

        /// <summary>
        /// 鼠标离开
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            // 通常不需要离开音效
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (playOnDown && button && button.interactable)
            {
                PlaySound("pointerDown");
            }
        }

        /// <summary>
        /// 鼠标松开
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (playOnUp && button && button.interactable)
            {
                PlaySound("pointerUp");
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        private void PlaySound(string type)
        {
            if (!audioSource || !feedbackConfig) return;

            AudioClip clip = feedbackConfig.GetSound(type);
            if (clip) audioSource.PlayOneShot(clip);
        }
    }
}

