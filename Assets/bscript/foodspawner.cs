using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject[] foodPrefabs;  
    public float spawnInterval = 5f;  
    public float fallSpeed = 2f;       
    public float pauseChance = 0.3f;   
    public Camera mainCamera;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        InvokeRepeating(nameof(SpawnFood), 0f, spawnInterval);
    }

    void SpawnFood()
    {
        if (foodPrefabs.Length == 0) return;

        
        GameObject prefab = foodPrefabs[Random.Range(0, foodPrefabs.Length)];

        
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

       
        float xPos = Random.Range(-camWidth / 2f, camWidth / 2f);
        float yPos = mainCamera.transform.position.y + camHeight / 2f + 1f; // slightly above screen

      
        Vector3 spawnPos = new Vector3(xPos, yPos, 0f);
        GameObject newFood = Instantiate(prefab, spawnPos, Quaternion.identity);

        
        FoodFall fallScript = newFood.AddComponent<FoodFall>();
        fallScript.fallSpeed = fallSpeed;
        fallScript.pauseChance = pauseChance;
    }
}
