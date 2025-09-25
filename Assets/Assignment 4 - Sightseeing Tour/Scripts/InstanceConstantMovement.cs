using UnityEngine;

public class InstanceConstantMovement : MonoBehaviour
{
    public GameObject prefab;     
    public int maxClones = 6;      
    public float spawnInterval = 1f; 

    private int cloneCount = 0;
    private float timer = 0f;
    private Vector3 spawnPosition;

    private void Start()
    {
        spawnPosition = transform.localPosition;
    }

    void Update()
    {
        if (cloneCount >= maxClones) return; 

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            GameObject clone = Instantiate(prefab, transform.parent);
            clone.transform.localPosition = spawnPosition;

            cloneCount++;
        }
    }
}