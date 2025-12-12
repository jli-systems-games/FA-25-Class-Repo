using UnityEngine;
using UnityEngine.UI;
using GreatAchievement.Systems;

namespace GreatAchievement.UI
{
    /// <summary>
    /// 暂停菜单面板 - 独立的UIScreen
    /// </summary>
    public class UIPauseMenuPanel : UIScreen
    {
        [Header("按钮")]
        public Button resumeButton;
        public Button optionsButton;
        public Button mainMenuButton;

        [Header("Options面板")]
        public UIOptionsPanel optionsPanel;

        [Header("场景")]
        public string mainMenuScene = "Main";

        private bool isPaused = false;

        protected override void Awake()
        {
            base.Awake();

            // 绑定按钮
            if (resumeButton) resumeButton.onClick.AddListener(OnResume);
            if (optionsButton) optionsButton.onClick.AddListener(OnOpenOptions);
            if (mainMenuButton) mainMenuButton.onClick.AddListener(OnReturnToMainMenu);

            // 初始隐藏
            gameObject.SetActive(false);
        }

        private void Update()
        {
            // ESC键切换暂停（只有在当前面板激活且没有Options时）
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 如果Options打开，不处理（由Options处理）
                if (optionsPanel && optionsPanel.gameObject.activeSelf)
                {
                    return;
                }

                // 切换暂停状态
                if (isPaused)
                {
                    OnResume();
                }
                else
                {
                    OnPause();
                }
            }
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void OnPause()
        {
            if (isPaused) return;

            isPaused = true;

            // 暂停游戏
            Time.timeScale = 0f;
            if (Systems.GameManager.Instance)
            {
                Systems.GameManager.Instance.PauseGame();
            }

            // 激活面板（带淡入动画）
            Activate(true); // exclusive = true

            PlayAudioFeedback("pointerClick");
        }

        /// <summary>
        /// 继续游戏
        /// </summary>
        public void OnResume()
        {
            if (!isPaused) return;

            isPaused = false;

            PlayAudioFeedback("pointerClick");

            // 淡出面板
            Deactivate();

            // 恢复游戏
            Time.timeScale = 1f;
            if (Systems.GameManager.Instance)
            {
                Systems.GameManager.Instance.ResumeGame();
            }
        }

        /// <summary>
        /// 打开Options
        /// </summary>
        public void OnOpenOptions()
        {
            PlayAudioFeedback("pointerClick");

            if (optionsPanel)
            {
                // 设置返回面板为当前面板
                optionsPanel.previousPanel = this;
                
                // 激活Options（会自动淡出当前面板）
                optionsPanel.Activate(true);
            }
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

            // 加载主菜单场景
            LoadScene(mainMenuScene);
        }
    }
}

