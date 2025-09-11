using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CountingGame : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject mousePrefab;
    private List<GameObject> spawnedMice = new List<GameObject>();

    public float spawnRangeX = 8.57f;
    public float spawnRangeY = 5.23f;

    private int targetClicks;
    private int currentClicks;
    private float timer;
    private float speed;
    private int counter=0;

    //public void StartCountingGame(float gameSpeed)
    public void StartCountingGame(float gameSpeed)
    {
        ClearSpawnedObjects();
        speed = gameSpeed;
        currentClicks = 0;
        //mousePrefab.SetActive(false);

        targetClicks = Mathf.RoundToInt(Random.Range(2,5) * speed);
        Debug.Log("target clicks: " + targetClicks);
         for (int i = 0; i < targetClicks; i++)
        {
            float x = Random.Range(-spawnRangeX, spawnRangeX);
            float y = Random.Range(-spawnRangeY, spawnRangeY);
            GameObject mouse = Instantiate(mousePrefab, new Vector3(x, y, 0), Quaternion.identity);
            //mouse.SetActive(true);
            spawnedMice.Add(mouse);
        }

        timer = 5f/speed/1.2f;
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (timer > 0)
        {
            Debug.Log("current time: " + timer);
            timer -= Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                currentClicks++;
            }

            yield return null;
        }

        if (currentClicks == targetClicks)
        {
            Debug.Log("counting game succeeded");

            StopAllCoroutines();
            GameManager.lastGameSuccess = true;
            GameManager.finishedSignal = true;
        }
        else
        {
            Debug.Log("counting game failed");

            StopAllCoroutines();
            //GameManager.lastGameSuccess = false;
            gameManager.LoseLife();
            GameManager.finishedSignal = true;
        }
    }
    public void ClearSpawnedObjects()
    {
        foreach (var obj in spawnedMice)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedMice.Clear();
    }
}