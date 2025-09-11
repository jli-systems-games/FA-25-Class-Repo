using UnityEngine;
using System.Collections;
using DG.Tweening.Core.Easing;

public class SpottingGame : MonoBehaviour
{
    public GameManager gameManager;
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
                    Debug.Log("spotting game succeeded");

                    StopAllCoroutines();
                    GameManager.lastGameSuccess = true;
                    GameManager.finishedSignal = true;
                }
            }
        }
    }

    IEnumerator GameTimer(float gameSpeed)
    {
        float timer = baseTimer / gameSpeed;
        while (timer > 0f&& gameActive)
        {
            Debug.Log("current time: " + timer);

            timer -= Time.deltaTime;
            yield return null;
        }

        if (gameActive)
        {
            Debug.Log("spotting game failed");
            gameActive = false;
            mouse.SetActive(false);

            StopAllCoroutines();
            //GameManager.lastGameSuccess = false;
            gameManager.LoseLife();
            GameManager.finishedSignal = true;
        }
        else yield break;
    }
}
