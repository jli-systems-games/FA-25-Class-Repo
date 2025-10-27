using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public float spawnRangeX = 8f;
    public float spawnY = 5.5f;
    public float interval = 0.8f;
    public GameObject[] goodFoods;
    public GameObject[] badItems;
    [Range(0f, 1f)] public float badProbability = 0.1f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        bool hasGood = goodFoods != null && goodFoods.Length > 0;
        bool hasBad = badItems != null && badItems.Length > 0;
        
        if (!hasGood && !hasBad) return;

        bool spawnBad = hasBad && Random.value < badProbability;
        GameObject prefab = null;

        if (spawnBad) prefab = badItems[Random.Range(0, badItems.Length)];
        else prefab = hasGood ? goodFoods[Random.Range(0, goodFoods.Length)] : badItems[Random.Range(0, badItems.Length)];

        if (!prefab) return;

        float x = Random.Range(-spawnRangeX, spawnRangeX);
        Instantiate(prefab, new Vector3(x, spawnY, 0f), Quaternion.identity);
    }


}