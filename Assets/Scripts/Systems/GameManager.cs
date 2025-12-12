using UnityEngine;
using UnityEngine.SceneManagement;

namespace GreatAchievement.Systems
{
    /// <summary>
    /// 游戏主管理器 - 管理游戏状态和场景切换
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("游戏状态")]
        public bool isPaused = false;

        [Header("重生系统")]
        [Tooltip("游戏开始时的默认出生点ID")]
        public string initialSpawnPointID = "StartPoint"; 
        
        public string lastCheckpointScene;
        public string lastCheckpointID;
        public string targetSpawnID;

        [Header("Abilities Unlocked")]
        public bool hasUnlockedDoubleJump = false;
        public bool hasUnlockedDash = false;
        public bool hasUnlockedWallSlide = false;

        public void UnlockAbility(string abilityName)
        {
            switch (abilityName)
            {
                case "DoubleJump":
                    hasUnlockedDoubleJump = true;
                    break;
                case "Dash":
                    hasUnlockedDash = true;
                    break;
                case "WallSlide":
                    hasUnlockedWallSlide = true;
                    break;
            }
        }

        public void ResetAbilities()
        {
            hasUnlockedDoubleJump = false;
            hasUnlockedDash = false;
            hasUnlockedWallSlide = false;
            
            // 同时重置存档点，防止重生在错误的进度
            lastCheckpointScene = "";
            lastCheckpointID = "";
            targetSpawnID = initialSpawnPointID;
            
            Debug.Log("GameManager: 所有能力和进度已重置。");
        }

        private void Awake()
        {
            // 单例模式
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // 初始化目标生成点为默认出生点
                if (string.IsNullOrEmpty(targetSpawnID))
                {
                    targetSpawnID = initialSpawnPointID;
                }

                // 强制重置技能状态（用于测试，模拟首次进入游戏）
                hasUnlockedDoubleJump = false;
                hasUnlockedDash = false;
                hasUnlockedWallSlide = false;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // 当玩家到达检查点时调用此方法来保存进度
        public void SetCheckpoint(string sceneName, string checkpointID)
        {
            lastCheckpointScene = sceneName;
            lastCheckpointID = checkpointID;
            // 关键修复：如果你希望 Checkpoint 也可以作为传送目标
            // 那么 targetSpawnID 并不需要在这里立即设置，
            // 而是在 RespawnPlayer() 的时候才去读取 lastCheckpointID 并赋值给 targetSpawnID
            
            // 但是，有些设计是走到 Checkpoint 后，如果只是普通切换场景回来，不一定在 Checkpoint，
            // 除非 Checkpoint 本身也是一个 SpawnPoint。
            // 按照之前的逻辑，Checkpoint 脚本记录了 ID。
        }

        private void Start()
        {
            Time.timeScale = 1f;
        }

        /// <summary>
        /// 玩家重生
        /// </summary>
        public void RespawnPlayer()
        {
            Time.timeScale = 1f;
            if (!string.IsNullOrEmpty(lastCheckpointScene))
            {
                targetSpawnID = lastCheckpointID; // 重生时将目标生成点设置为上一个检查点ID
                SceneManager.LoadScene(lastCheckpointScene);
            }
            else
            {
                // 如果没有检查点，重置当前场景
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;
        }

        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        public void LoadScene(string sceneName)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
