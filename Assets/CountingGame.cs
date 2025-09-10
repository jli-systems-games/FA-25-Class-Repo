using UnityEngine;
using System.Collections;

public class CountingGame : MonoBehaviour
{
    public GameObject mousePrefab;
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
            Debug.Log(counter++);
        }

        timer = 5f/speed;
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                currentClicks++;
            }

            yield return null;
        }

        if (currentClicks == targetClicks)
        {
            GameManager.speedManager.IncreaseSpeed();
            GameBridge.EndGame(true);
        }
        else
        {
            GameManager.liveManager.LoseLife();
            GameManager.speedManager.IncreaseSpeed();
            GameBridge.EndGame(true);
        }
    }
}