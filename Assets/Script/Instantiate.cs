using System.Collections.Generic;
using UnityEngine;

public class RandomPrefabSpawner3D : MonoBehaviour
{
    public List<GameObject> prefabPool = new List<GameObject>();
    public int spawnAmount = 20;
    public Vector2 areaSize = new Vector2(10f, 10f);
    public float minHeight = 0f;
    public float maxHeight = 5f;
    public bool spawnOnStart = true;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnAll();
        }
    }

    /// <summary>
    /// spawn objects in a range
    /// </summary>
    public void SpawnAll()
    {
        if (prefabPool == null || prefabPool.Count == 0)
        {
            return;
        }



        for (int i = 0; i < spawnAmount; i++)
        {
         
            float offsetX = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
            float offsetZ = Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);

          
            float offsetY = Random.Range(minHeight, maxHeight);

            Vector3 spawnPos = transform.position + new Vector3(offsetX, offsetY, offsetZ);

          
            int index = Random.Range(0, prefabPool.Count);
            GameObject prefab = prefabPool[index];

            GameObject newObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            spawnedObjects.Add(newObj);
        }
    }

}