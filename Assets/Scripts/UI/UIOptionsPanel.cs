using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

namespace GreatAchievement.UI
{
    /// <summary>
    /// Options面板 - 独立的UIScreen，可被主菜单和暂停菜单使用
    /// </summary>
    public class UIOptionsPanel : UIScreen
    {
        [Header("音频设置")]
        public AudioMixer audioMixer;
        public Slider volumeSlider;
        public TextMeshProUGUI volumeText;

        [Header("画质设置")]
        public TMP_Dropdown qualityDropdown;

        [Header("全屏设置")]
        public Toggle fullscreenToggle;

        [Header("分辨率设置")]
        public TMP_Dropdown resolutionDropdown;
        private Resolution[] resolutions;

        [Header("返回面板")]
        public UIScreen previousPanel; // 返回到哪个面板（会由调用者设置）

        private const string VOLUME_KEY = "Volume";
        private const string QUALITY_KEY = "Quality";
        private const string FULLSCREEN_KEY = "Fullscreen";
        private const string RESOLUTION_KEY = "ResolutionIndex";

        protected override void Awake()
        {
            base.Awake();
            InitializeSettings();
            LoadSettings();
        }

        private void Update()
        {
            // 按ESC返回
            if (gameObject.activeSelf && !isAnimating && Input.GetKeyDown(KeyCode.Escape))
            {
                OnBack();
            }
        }

        /// <summary>
        /// 返回上一个面板
        /// </summary>
        public void OnBack()
        {
            PlayAudioFeedback("pointerClick");
            
            // 保存设置
            ApplySettings();

            // 激活上一个面板（使用exclusive自动淡出当前面板）
            if (previousPanel)
            {
                previousPanel.Activate(true);
            }
            else
            {
                // 如果没有设置，就简单淡出
                Deactivate();
            }
        }

        /// <summary>
        /// 初始化设置
        /// </summary>
        private void InitializeSettings()
        {
            // 初始化分辨率
            if (resolutionDropdown)
            {
                resolutions = Screen.resolutions;
                resolutionDropdown.ClearOptions();

                var options = new System.Collections.Generic.List<string>();
                int currentResolutionIndex = 0;

                for (int i = 0; i < resolutions.Length; i++)
                {
                    string option = $"{resolutions[i].width} x {resolutions[i].height} @ {resolutions[i].refreshRateRatio}Hz";
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

            // 初始化画质
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

        private void LoadSettings()
        {
            if (volumeSlider)
            {
                float volume = PlayerPrefs.GetFloat(VOLUME_KEY, 0f);
                volumeSlider.value = volume;
                SetVolume(volume);
            }

            if (qualityDropdown)
            {
                int quality = PlayerPrefs.GetInt(QUALITY_KEY, QualitySettings.GetQualityLevel());
                qualityDropdown.value = quality;
                SetQuality(quality);
            }

            if (fullscreenToggle)
            {
                bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, Screen.fullScreen ? 1 : 0) == 1;
                fullscreenToggle.isOn = fullscreen;
                SetFullScreen(fullscreen);
            }

            if (resolutionDropdown && resolutions != null)
            {
                int resolutionIndex = PlayerPrefs.GetInt(RESOLUTION_KEY, resolutionDropdown.value);
                if (resolutionIndex < resolutions.Length)
                {
                    resolutionDropdown.value = resolutionIndex;
                    SetResolution(resolutionIndex);
                }
            }
        }

        public void SetVolume(float volume)
        {
            if (audioMixer)
            {
                float dbVolume = volume <= -40f ? -80f : volume;
                audioMixer.SetFloat("Volume", dbVolume);
            }

            if (volumeText)
            {
                volumeText.text = Mathf.RoundToInt((volume + 40f) / 40f * 100f) + "%";
            }

            PlayerPrefs.SetFloat(VOLUME_KEY, volume);
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            PlayerPrefs.SetInt(QUALITY_KEY, qualityIndex);
        }

        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
            PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullScreen ? 1 : 0);
        }

        public void SetResolution(int resolutionIndex)
        {
            if (resolutions == null || resolutionIndex >= resolutions.Length) return;

            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt(RESOLUTION_KEY, resolutionIndex);
        }

        public void ApplySettings()
        {
            PlayerPrefs.Save();
        }

        public void ResetToDefault()
        {
            if (volumeSlider) volumeSlider.value = 0f;
            if (qualityDropdown) qualityDropdown.value = 2;
            if (fullscreenToggle) fullscreenToggle.isOn = true;
        }
    }
}

