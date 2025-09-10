using UnityEngine;
using System.Collections;

public class SpottingGame : MonoBehaviour
{
    public GameObject mouse;

    public float spawnRangeX = 8.57f;
    public float spawnRangeY = 5.23f;

    public float baseTimer = 5f;
    private bool gameActive = false;

    public void StartSpottingGame(float gameSpeed)
    {
        SpawnMouse();
        gameActive = true;
        StartCoroutine(GameTimer(gameSpeed));
    }

    void SpawnMouse()
    {
        Vector3 pos = new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY),
            0f
        );
        mouse.transform.position = pos;
        mouse.SetActive(true);
    }

    void Update()
    {
        if (!gameActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == mouse)
                {
                    gameActive = false;
                    mouse.SetActive(false);
                    GameManager.speedManager.IncreaseSpeed();
                }
            }
        }
    }

    IEnumerator GameTimer(float gameSpeed)
    {
        while (gameSpeed > 0f)
        {
            gameSpeed -= Time.deltaTime;
            yield return null;
        }

        if (gameActive)
        {
            GameManager.speedManager.IncreaseSpeed();
            GameManager.liveManager.LoseLife();

            gameActive = false;
            mouse.SetActive(false);
        }
    }
}
