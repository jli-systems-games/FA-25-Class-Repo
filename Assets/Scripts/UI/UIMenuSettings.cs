using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

namespace GreatAchievement.UI
{
    /// <summary>
    /// Options菜单设置
    /// 处理音量、画质、全屏等设置
    /// </summary>
    public class UIMenuSettings : UIScreen
    {
        [Header("音频设置")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private TextMeshProUGUI volumeText;

        [Header("画质设置")]
        [SerializeField] private TMP_Dropdown qualityDropdown;

        [Header("全屏设置")]
        [SerializeField] private Toggle fullscreenToggle;

        [Header("分辨率设置")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        private Resolution[] resolutions;

        [Header("父级菜单引用")]
        public UIScreen parentMenu; // 父级菜单（UIMainMenu或UIPauseMenu）

        [Header("PlayerPrefs键")]
        private const string VOLUME_KEY = "Volume";
        private const string QUALITY_KEY = "Quality";
        private const string FULLSCREEN_KEY = "Fullscreen";
        private const string RESOLUTION_KEY = "ResolutionIndex";

        private bool isClosing = false; // 防止重复关闭

        protected override void Awake()
        {
            base.Awake();
            InitializeSettings();
            LoadSettings();
        }

        private void Update()
        {
            // 只有当面板激活且没有正在关闭时才响应ESC
            if (gameObject.activeSelf && !isClosing && Input.GetKeyDown(KeyCode.Escape))
            {
                OnBack();
            }
        }
        
        /// <summary>
        /// 设置父级菜单（由UIMainMenu或UIPauseMenu调用）
        /// </summary>
        public void SetParentMenu(UIScreen parent)
        {
            parentMenu = parent;
            Debug.Log($"[UIMenuSettings] 父级菜单已设置: {parent.GetType().Name}");
        }
        
        /// <summary>
        /// 返回处理（ESC键） - 使用淡出动画返回到父级菜单
        /// </summary>
        public void OnBack()
        {
            if (isClosing) return; // 防止重复调用
            isClosing = true;

            PlayAudioFeedback("pointerClick");
            
            // 保存设置
            ApplySettings();
            
            // 使用淡出动画关闭
            Deactivate(fadeTime);
            
            // 通知父级菜单显示主界面
            if (parentMenu != null)
            {
                // 尝试调用 OnBackToMain 方法（使用反射或 SendMessage）
                parentMenu.SendMessage("OnBackToMain", SendMessageOptions.DontRequireReceiver);
                parentMenu.SendMessage("OnBackToPause", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Debug.LogWarning("[UIMenuSettings] 未设置父级菜单！");
            }
            
            // 重置关闭标志
            StartCoroutine(ResetClosingFlag());
        }
        
        /// <summary>
        /// 重置关闭标志
        /// </summary>
        private System.Collections.IEnumerator ResetClosingFlag()
        {
            yield return new WaitForSeconds(fadeTime + 0.1f);
            isClosing = false;
        }

        /// <summary>
        /// 初始化设置
        /// </summary>
        private void InitializeSettings()
        {
            // 初始化分辨率选项
            if (resolutionDropdown)
            {
                resolutions = Screen.resolutions;
                resolutionDropdown.ClearOptions();

                System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();
                int currentResolutionIndex = 0;

                for (int i = 0; i < resolutions.Length; i++)
                {
                    string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio + "Hz";
                    options.Add(option);

                    if (resolutions[i].width == Screen.currentResolution.width &&
                        resolutions[i].height == Screen.currentResolution.height)
                    {
                        currentResolutionIndex = i;
                    }
                }

                resolutionDropdown.AddOptions(options);
                resolutionDropdown.value = currentResolutionIndex;
                resolutionDropdown.RefreshShownValue();
            }

            // 初始化画质选项
            if (qualityDropdown)
            {
                qualityDropdown.ClearOptions();
                qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
                qualityDropdown.value = QualitySettings.GetQualityLevel();
                qualityDropdown.RefreshShownValue();
            }

            // 绑定事件
            if (volumeSlider) volumeSlider.onValueChanged.AddListener(SetVolume);
            if (qualityDropdown) qualityDropdown.onValueChanged.AddListener(SetQuality);
            if (fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(SetFullScreen);
            if (resolutionDropdown) resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        /// <summary>
        /// 加载保存的设置
        /// </summary>
        private void LoadSettings()
        {
            // 加载音量
            if (volumeSlider)
            {
                float volume = PlayerPrefs.GetFloat(VOLUME_KEY, 0f);
                volumeSlider.value = volume;
                SetVolume(volume);
            }

            // 加载画质
            if (qualityDropdown)
            {
                int quality = PlayerPrefs.GetInt(QUALITY_KEY, QualitySettings.GetQualityLevel());
                qualityDropdown.value = quality;
                SetQuality(quality);
            }

            // 加载全屏
            if (fullscreenToggle)
            {
                bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, Screen.fullScreen ? 1 : 0) == 1;
                fullscreenToggle.isOn = fullscreen;
                SetFullScreen(fullscreen);
            }

            // 加载分辨率
            if (resolutionDropdown)
            {
                int resolutionIndex = PlayerPrefs.GetInt(RESOLUTION_KEY, resolutionDropdown.value);
                if (resolutionIndex < resolutions.Length)
                {
                    resolutionDropdown.value = resolutionIndex;
                    SetResolution(resolutionIndex);
                }
            }
        }

        /// <summary>
        /// 设置音量
        /// </summary>
        public void SetVolume(float volume)
        {
            if (audioMixer)
            {
                // AudioMixer使用对数刻度
                float dbVolume = volume <= -40f ? -80f : volume;
                audioMixer.SetFloat("Volume", dbVolume);
            }

            // 更新文本显示
            if (volumeText)
            {
                volumeText.text = Mathf.RoundToInt((volume + 40f) / 40f * 100f) + "%";
            }

            PlayerPrefs.SetFloat(VOLUME_KEY, volume);
            PlayAudioFeedback("pointerClick");
        }

        /// <summary>
        /// 设置画质
        /// </summary>
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            PlayerPrefs.SetInt(QUALITY_KEY, qualityIndex);
            PlayAudioFeedback("pointerClick");
            
            Debug.Log($"画质设置为: {QualitySettings.names[qualityIndex]}");
        }

        /// <summary>
        /// 设置全屏
        /// </summary>
        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
            PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullScreen ? 1 : 0);
            PlayAudioFeedback("pointerClick");
            
            Debug.Log($"全屏: {isFullScreen}");
        }

        /// <summary>
        /// 设置分辨率
        /// </summary>
        public void SetResolution(int resolutionIndex)
        {
            if (resolutions == null || resolutionIndex >= resolutions.Length) return;

            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt(RESOLUTION_KEY, resolutionIndex);
            
            Debug.Log($"分辨率: {resolution.width}x{resolution.height}");
        }

        /// <summary>
        /// 应用所有设置
        /// </summary>
        public void ApplySettings()
        {
            PlayerPrefs.Save();
            PlayAudioFeedback("submit");
            Debug.Log("设置已保存");
        }

        /// <summary>
        /// 重置为默认设置
        /// </summary>
        public void ResetToDefault()
        {
            if (volumeSlider) volumeSlider.value = 0f;
            if (qualityDropdown) qualityDropdown.value = 2; // Medium
            if (fullscreenToggle) fullscreenToggle.isOn = true;
            
            PlayAudioFeedback("pointerClick");
            Debug.Log("已重置为默认设置");
        }
    }
}

