using UnityEngine;
using System.Collections;

public class RandomOilSpawner : MonoBehaviour
{
    public GameObject oilPrefab;       
    public Transform[] spawnPoints;   
    public float spawnInterval = 1f;
    public Transform parentContainer;

    public GameObject completionChecker;

    public int maxSpawns = 20;
    private int spawnCount = 0;

    void Start()
    {
        StartCoroutine(SpawnOilRoutine());
    }

    IEnumerator SpawnOilRoutine()
    {
        while (spawnCount < maxSpawns)
        {
            SpawnOilSet();
            spawnCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
        SpawnSeaweed();

    }

    void SpawnOilSet()
    {
        GameObject oilLine = new GameObject("Oil Line");
        oilLine.transform.parent = parentContainer;

        int emptyIndex = Random.Range(0, spawnPoints.Length);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (i == emptyIndex) continue;

            GameObject newOil = Instantiate(oilPrefab, spawnPoints[i].position, Quaternion.identity);
            newOil.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            newOil.transform.parent = oilLine.transform;
        }
    }

    void SpawnSeaweed()
    {
        Instantiate(completionChecker, spawnPoints[1].position, Quaternion.identity, parentContainer);
    }
}
