using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NailSpawner : MonoBehaviour
{
    public GameObject nailPrefab;
    public int nailCount = 10;
    public float minRadius = 0.1f;
    public LayerMask nailLayer;
    private GameObject[] spawnedNails;
    [Space(10)]

    //Spawn Area Bounds
    public float spawnAreaXMax = -6.4f;
    public float spawnAreaXMin = -7.2f;
    public float spawnAreaYMax = 0.128f;
    public float spawnAreaYMin = 0.056f;
    public float spawnAreaZMax = -1.57f;
    public float spawnAreaZMin = -1.97f;
    [Space(10)]

    public GameManager gameManager;

    void Start()
    {
        spawnedNails = new GameObject[nailCount];
        SpawnNails();
    }

    void SpawnNails()
    {
        int spawned = 0;

        while (spawned < nailCount) //keep looping the spawn so that it will  instantiate until it finds a right position for the nails
        {
            Vector3 randomPos = new Vector3(
                Random.Range(spawnAreaXMin, spawnAreaXMax),
                Random.Range(spawnAreaYMin, spawnAreaYMax),
                Random.Range(spawnAreaZMin, spawnAreaZMax)
            );

            if (!Physics.CheckSphere(randomPos, minRadius, nailLayer)) //Check Sphere: Creates a sphere and checks if it collides with any object in that layer
            {
                GameObject newNail = Instantiate(nailPrefab, randomPos, Quaternion.Euler(90f, 0f, 0f), transform);
                newNail.transform.localScale = Vector3.one * 17.664f;
                newNail.layer = LayerMask.NameToLayer("Nail");

                spawnedNails[spawned] = newNail;
                spawned++;
            }
        }
        //Got code to check overlapping objects from https://www.youtube.com/watch?v=ENEtzLePZbQ&ab_channel=Rabidgremlin
    }

    private void Update()
    {
        if (AreAllNailsDown())
        {
            Debug.Log("All nails are down.");

            StartCoroutine(CompletionDelay(2f));
        }
    }

    public bool AreAllNailsDown()
    {
        foreach (GameObject nail in spawnedNails)
        {
            if (nail.transform.position.y > -0.03f)
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerator CompletionDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameManager.LoadRandomGame();
    }
}
