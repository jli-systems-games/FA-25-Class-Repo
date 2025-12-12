using UnityEngine;
using GreatAchievement.Systems;

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("此生成点的唯一ID")]
    public string spawnPointID;

    private void Start()
    {
        // 检查是否是指定的目标生成点
        if (GameManager.Instance != null && GameManager.Instance.targetSpawnID == spawnPointID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = transform.position;
                Debug.Log($"玩家已生成在点: {spawnPointID}");
            }
        }
    }
    
    // 在编辑器中绘制图标以便查看
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}


