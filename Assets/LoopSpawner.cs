using UnityEngine;

public class LoopSpawner : MonoBehaviour
{
    [Header("Prefab 画廊")]
    public GameObject prefabA;   // 正常画廊
    public GameObject prefabB;   // 特殊的画廊
    public Vector3 spawnPosition = new Vector3(-57.6f, 16.21f, 0.0157f);

    [Header("触发和销毁的x")]
    public float triggerX = -2.8f;   // 触发生成的 X 
    public float destroyX = 47.8f;   // 销毁的 X 

    private bool hasSpawned = false;

    // 记录上一次生成的（这个是为了那个 不重复生成的逻辑）
    private static GameObject lastSpawnedPrefab = null;

    void Update()
    {
        float x = transform.position.x;

        // 生成的点，进行一个检测
        if (!hasSpawned && x >= triggerX)
        {
            SpawnNewPrefab();
            hasSpawned = true;
        }

        // 销毁的点进行一个检测
        if (x >= destroyX)
        {
            Destroy(gameObject);
        }
    }

    void SpawnNewPrefab()
    {
        int roll = Random.Range(0, 100);

        GameObject toSpawn;

        if (roll < 85)  //生成无特殊画框的几率
        {
            toSpawn = prefabA;
        }
        else            
        {
            toSpawn = prefabB;//反之
        }

        // 如果上次生成了特殊画框那么这次就不可以继续生产特殊画框了防止出现的过于频繁
        if (lastSpawnedPrefab == prefabB && toSpawn == prefabB)
        {
            toSpawn = prefabA;
        }

        // 生成并记录上一个生成的是什么，这个逻辑是为了服务之前那个防止重复生成我的prefabb
        Instantiate(toSpawn, spawnPosition, Quaternion.identity);
        lastSpawnedPrefab = toSpawn;
    }
}
