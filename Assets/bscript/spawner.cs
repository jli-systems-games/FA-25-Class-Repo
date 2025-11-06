using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;   
    public Transform player;          
    public float spawnRadius = 10f;   
    public int spawnCount = 1;

    private void Start()
    {
        SpawnRandom();
    }
    public void SpawnRandom()
    {
        if (prefabToSpawn == null || player == null) return;

        for (int i = 0; i < spawnCount; i++)
        {
            
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(spawnRadius * 0.8f, spawnRadius);
            Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        }
    }
}

