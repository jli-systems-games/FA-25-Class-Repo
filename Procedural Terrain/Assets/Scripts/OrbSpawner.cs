using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public GameObject orbPrefab;
    public GameObject cat;
    public int orbCount = 5;
    public float minDistance = 3f;
    public float maxDistance = 15f;

   
    public void SpawnOrbs()
    {
       
        foreach (GameObject orb in GameObject.FindGameObjectsWithTag("Orb"))
            Destroy(orb);

        if (orbPrefab == null || cat == null) return;

        for (int i = 0; i < orbCount; i++)
        {
            Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(minDistance, maxDistance);
            Vector3 spawnPos = cat.transform.position + new Vector3(circle.x, 1.5f, circle.y);
            GameObject newOrb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);
            newOrb.tag = "Orb";
        }
    }

    
    public void SetCat(GameObject newCat)
    {
        cat = newCat;
    }
}