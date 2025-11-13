using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Tilemap tilemap;
    public int enemyCount = 5;

    void Start()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3Int cellPos = new Vector3Int(Random.Range(1, 5), Random.Range(-3, 3), 0); // adjust range to right side
            Vector3 spawnPos = tilemap.GetCellCenterWorld(cellPos);
            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPos, Quaternion.identity);
        }
    }
}
