using UnityEngine;
using UnityEngine.UI;

namespace GreatAchievement.UI
{
    /// <summary>
    /// 暂停菜单
    /// 游戏中按ESC显示
    /// </summary>
    public class UIPauseMenu : UIScreen
    {
        [Header("暂停菜单")]
        public GameObject pauseMenuPanel;
        public GameObject optionsPanel;

        [Header("按钮")]
        public Button resumeButton;
        public Button optionsButton;
        public Button mainMenuButton;

        [Header("场景")]
        public string mainMenuScene = "Main";

        private bool isPaused = false;

        protected override void Awake()
        {
            base.Awake();

            // 初始隐藏
            if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(false);
            gameObject.SetActive(false);

            // 绑定按钮
            if (resumeButton) resumeButton.onClick.AddListener(OnResume);
            if (optionsButton) optionsButton.onClick.AddListener(OnOpenOptions);
            if (mainMenuButton) mainMenuButton.onClick.AddListener(OnReturnToMainMenu);
        }

        private void Start()
        {
            // 设置Options面板的父级菜单引用
            if (optionsPanel != null)
            {
                UIMenuSettings settings = optionsPanel.GetComponent<UIMenuSettings>();
                if (settings != null)
                {
                    settings.SetParentMenu(this);
                }
            }
        }

        private void Update()
        {
            // ESC键逻辑 - 简化处理
            if (!Input.GetKeyDown(KeyCode.Escape)) return;

            // 如果Options面板打开，让UIMenuSettings处理
            if (optionsPanel && optionsPanel.activeSelf)
            {
                return;
            }

            // 如果暂停菜单打开，ESC键继续游戏
            if (isPaused && pauseMenuPanel && pauseMenuPanel.activeSelf)
            {
                OnResume();
                return;
            }

            // 否则暂停游戏
            if (!isPaused)
            {
                OnPause();
            }
        }

        /// <summary>
        /// 暂停游戏（带过渡动画）
        /// </summary>
        public void OnPause()
        {
            if (isPaused) return; // 防止重复暂停
            
            isPaused = true;

            // 暂停游戏逻辑
            Time.timeScale = 0f;
            if (Systems.GameManager.Instance)
            {
                Systems.GameManager.Instance.PauseGame();
            }

            // 显示暂停菜单
            if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
            if (optionsPanel) optionsPanel.SetActive(false);

            // 激活Canvas并播放淡入动画
            Activate(false);

            PlayAudioFeedback("pointerClick");
        }

        /// <summary>
        /// 继续游戏（带过渡动画）
        /// </summary>
        public void OnResume()
        {
            if (!isPaused) return; // 防止重复继续
            
            isPaused = false;

            PlayAudioFeedback("pointerClick");

            // 隐藏所有面板
            if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(false);

            // 播放淡出动画
            Deactivate(fadeTime);

            // 恢复游戏
            Time.timeScale = 1f;
            if (Systems.GameManager.Instance)
            {
                Systems.GameManager.Instance.ResumeGame();
            }
        }

        /// <summary>
        /// 打开Options（带过渡动画）
        /// </summary>
        public void OnOpenOptions()
        {
            PlayAudioFeedback("pointerClick");
            
            // 隐藏暂停菜单
            if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
            
            // 激活并淡入Options
            if (optionsPanel)
            {
                UIMenuSettings settings = optionsPanel.GetComponent<UIMenuSettings>();
                if (settings != null)
                {
                    settings.Activate(false);
                }
                else
                {
                    optionsPanel.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 从Options返回暂停菜单
        /// </summary>
        public void OnBackToPause()
        {
            // Options会自己淡出，这里只显示暂停菜单
            if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
        }

        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void OnReturnToMainMenu()
        {
            PlayAudioFeedback("pointerClick");
            
            // 恢复时间
            Time.timeScale = 1f;
            isPaused = false;

            LoadScene(mainMenuScene, fadeTime);
        }
    }
}

