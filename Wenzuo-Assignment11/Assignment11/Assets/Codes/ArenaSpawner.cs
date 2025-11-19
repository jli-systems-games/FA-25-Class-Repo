using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;   // 不同车的 prefab 都拖进来
    public Transform[] spawnPoints;   // 所有重生点
    public int carsToSpawn = 8;       // 本局一共要生成多少辆车
    public float spawnInterval = 0.6f;

    IEnumerator Start()
    {
        // 把所有重生点放进一个 List，方便“用过就删”
        List<Transform> freeSpawns = new List<Transform>(spawnPoints);

        int spawned = 0;

        while (spawned < carsToSpawn && freeSpawns.Count > 0)
        {
            // 随机拿一个“还没用过的”重生点
            int spawnIndex = Random.Range(0, freeSpawns.Count);
            Transform sp = freeSpawns[spawnIndex];

            // 用过一次就从列表里移除，这样之后不会再用到它
            freeSpawns.RemoveAt(spawnIndex);

            // 随机选一辆车（如果你只用一种车，也可以写 carPrefabs[0]）
            GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];

            Instantiate(prefab, sp.position, sp.rotation);

            spawned++;

            if (spawnInterval > 0f)
                yield return new WaitForSeconds(spawnInterval);
            else
                yield return null;  // 立刻生成下一辆
        }
    }
}
