using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public int carCount = 10;
    public float spawnHeight = 40f;
    public float platformRadius = 8f;

    void Start()
    {
        StartCoroutine(SpawnCars());
    }

    IEnumerator SpawnCars()
    {
        for (int i = 0; i < carCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * platformRadius;
            Vector3 pos = new Vector3(offset.x, spawnHeight, offset.y);
            Instantiate(carPrefab, pos, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }
}