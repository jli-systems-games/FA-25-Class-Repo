using UnityEngine;

namespace GreatAchievement.UI
{
    /// <summary>
    /// UI音频反馈配置 - ScriptableObject
    /// 存储UI交互音效
    /// </summary>
    [CreateAssetMenu(fileName = "UI Audio Feedback Config", menuName = "Great Achievement/UI Audio Feedback Config", order = 1000)]
    public class UIAudioFeedbackConfig : ScriptableObject
    {
        [Header("指针事件")]
        public AudioClip pointerClick;   // 点击
        public AudioClip pointerEnter;   // 鼠标进入
        public AudioClip pointerExit;    // 鼠标离开
        public AudioClip pointerDown;    // 按下
        public AudioClip pointerUp;      // 松开

        [Header("选择事件")]
        public AudioClip select;         // 选择
        public AudioClip deselect;       // 取消选择
        public AudioClip submit;         // 提交

        /// <summary>
        /// 根据类型名称获取音效
        /// </summary>
        public AudioClip GetSound(string type)
        {
            System.Reflection.FieldInfo f = typeof(UIAudioFeedbackConfig).GetField(type);
            if (f != null && f.FieldType == typeof(AudioClip))
            {
                AudioClip result = (AudioClip)f.GetValue(this);
                if (result) return result;
            }

            return null;
        }
    }
}

