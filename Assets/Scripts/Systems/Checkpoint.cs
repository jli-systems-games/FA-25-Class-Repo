using UnityEngine;
using GreatAchievement.Systems;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("此检查点的唯一ID")]
    public string checkpointID;

    [Tooltip("是否触碰时回满血")]
    public bool healPlayer = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager instance not found! Please ensure GameManager exists in the scene.");
                return;
            }

            // 1. 记录存档信息
            GameManager.Instance.SetCheckpoint(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 
                checkpointID
            );
            Debug.Log($"检查点已激活: {checkpointID}");

            // 2. 回血逻辑 (类似空洞骑士坐长椅)
            if (healPlayer)
            {
                CharacterController2D player = other.GetComponent<CharacterController2D>();
                if (player != null)
                {
                    player.life = 10f; // 假设满血是10，或者你可以从PlayerStats读取最大血量
                    // 这里可以添加回血特效或音效
                }
            }
        }
    }
}

