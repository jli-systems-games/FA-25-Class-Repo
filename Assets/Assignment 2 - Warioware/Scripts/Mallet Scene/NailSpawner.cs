using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NailSpawner : MonoBehaviour
{
    public GameObject nailPrefab;
    public int nailCount = 10;
    public float minRadius = 0.1f;
    public LayerMask nailLayer;
    private GameObject[] spawnedNails;
    public int targetNailCount;
    [Space(10)]

    //Material
    public Material normalMaterial;
    public Material blueMaterial;
    public Material redMaterial;
    [Space(10)]

    //Spawn Area Bounds
    public float spawnAreaXMax = -6.45f;
    public float spawnAreaXMin = -7.15f;
    public float spawnAreaYMax = 0.128f;
    public float spawnAreaYMin = 0.056f;
    public float spawnAreaZMax = -1.59f;
    public float spawnAreaZMin = -1.9f;
    [Space(10)]

    public GameManager gameManager;
    public Timer timer;

    private int malletMode;

    private bool isNormalMode = false;
    private bool isColorMode = false;

    void Start()
    {
        malletMode = Data.globalMalletMode;

        if (malletMode == 0) isColorMode = true;
        else if (malletMode == 1) isColorMode = true;

        spawnedNails = new GameObject[nailCount];
        SpawnNails();
    }

    void SpawnNails()
    {
        int spawned = 0;

        //Create variables to make sure in color mode the targetnailcount gets spawned
        int targetNailsToSpawn = Mathf.Clamp(targetNailCount, 0, nailCount);
        int otherNailsToSpawn = nailCount - targetNailsToSpawn;

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

                Renderer nailRenderer = newNail.GetComponent<Renderer>();

                if (isNormalMode)
                {
                    nailRenderer.material = normalMaterial;
                }
                else if (isColorMode)
                {
                    //Make screw red or blue
                    if (targetNailsToSpawn > 0)
                    {
                        nailRenderer.material = redMaterial;
                        targetNailsToSpawn--;
                    }
                    else
                    {
                        nailRenderer.material = blueMaterial;
                    }
                }

                spawnedNails[spawned] = newNail;
                spawned++;
            }
        }
        //Got code to check overlapping objects from https://www.youtube.com/watch?v=ENEtzLePZbQ&ab_channel=Rabidgremlin
    }

    private void Update()
    {
        if (isNormalMode)
        {
            if (AreAllNailsDown())
            {
                Completed();
            }
        }
        else if (isColorMode)
        {
            if (AreOnlyCertainNailsDown())
            {
                Completed();
            }
        }
    }

    void Completed()
    {
        isNormalMode = false;
        isColorMode = false;
        timer.isTimerRunning = false;
        StartCoroutine(CompletionDelay(2f));
    }
    public bool AreOnlyCertainNailsDown()
    {
        foreach (GameObject nail in spawnedNails)
        {
            Renderer nailRenderer = nail.GetComponent<Renderer>();

            if (nailRenderer.material.name.Contains(redMaterial.name))
            {
                if (nail.transform.position.y > -0.03f)
                {
                    return false;
                }
            }
            
            if (nailRenderer.material.name.Contains(blueMaterial.name))
            {
                if (nail.transform.position.y < 0.05f)
                {
                    return false;
                }
            }
        }

        return true;
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
