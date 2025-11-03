using System.Collections.Generic;
using UnityEngine;

public class HallwayInstance : MonoBehaviour
{
    public GameObject hallwayPrefab;
    public float hallwayLength = 10;
    public int numSections = 5;
    public Transform player;

    private List<GameObject> hallways =
        new List<GameObject>();

    private float lastSpawnZ;

    void Start()
    {
        for (int i = 0; i < numSections; i++)
        {
            Vector3 position = new Vector3(0, 0, i * hallwayLength);
            GameObject section = Instantiate(hallwayPrefab, position, Quaternion.identity);
            hallways.Add(section);
            lastSpawnZ = position.z;
        }
    }

    private void Update()
    {
        if (player.position.z + (hallwayLength * 2) > lastSpawnZ)
        {
            SpawnSection();
        }
    }

    void SpawnSection()
    {
        lastSpawnZ += hallwayLength;
        Vector3 position = new Vector3(0, 0, lastSpawnZ);
        GameObject section = Instantiate(hallwayPrefab, position, Quaternion.identity);
        hallways.Add(section);
    }
}
