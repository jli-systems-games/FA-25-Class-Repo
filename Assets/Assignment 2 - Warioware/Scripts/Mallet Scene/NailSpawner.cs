using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NailSpawner : MonoBehaviour
{
    public GameObject nailPrefab;
    private int nailCount;
    public float minRadius = 0.2f;
    public LayerMask nailLayer;
    private GameObject[] spawnedNails;
    private int targetNailCount = 3;
    [Space(10)]

    //Material
    public Material normalMaterial;
    public Material blueMaterial;
    public Material redMaterial;
    [Space(10)]

    //Spawn Area Bounds
    private float spawnAreaXMax = -6.436f;
    private float spawnAreaXMin = -7.164f;
    private float spawnAreaYMax = 0.128f;
    private float spawnAreaYMin = 0.056f;
    private float spawnAreaZMax = -1.59f;
    private float spawnAreaZMin = -1.96f;

    public GameManager gameManager;
    public Timer timer;
    public ParticleSystem confettiParticle;

    private int malletMode;

    private bool isNormalMode = false;
    private bool isColorMode = false;

    void Start()
    {
        confettiParticle.gameObject.SetActive(false);

        malletMode = Data.globalMalletMode;

        if (malletMode == 0)
        {
            isNormalMode = true;
            nailCount = 3;
        }
        else if (malletMode == 1)
        {
            isColorMode = true;
            nailCount = 6;
        }

        spawnedNails = new GameObject[nailCount];
        SpawnNails();
    }

    void SpawnNails()
    {
        int spawned = 0;
        int attempts = 0;

        //Create variables to make sure in color mode the targetnailcount gets spawned
        int targetNailsToSpawn = Mathf.Clamp(targetNailCount, 0, nailCount);
        int otherNailsToSpawn = nailCount - targetNailsToSpawn;

        while (spawned < nailCount && attempts < 500) //keep looping the spawn until 100 attempts so that it will instantiate until it finds a right position for the nails
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
            attempts++;
        }
        //Got code to check overlapping objects from https://www.youtube.com/watch?v=ENEtzLePZbQ&ab_channel=Rabidgremlin

        if (spawned < nailCount)
        {
            Debug.LogWarning("Nail spawning failed");
        }
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
        Data.globalConsecutiveRound += 1;
        confettiParticle.gameObject.SetActive(true);
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
