using UnityEngine;
using System.Collections;

public class SpottingGame : MonoBehaviour
{
    public GameObject mouse;
    private GameObject currentMouse;

    public float spawnRangeX = 8.57f;
    public float spawnRangeY = 5.23f;

    public float baseTimer = 5f;
    private bool gameActive = false;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        SpawnMouse();
        gameActive = true;
        StartCoroutine(GameTimer());
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
                }
            }
        }
    }

    IEnumerator GameTimer()
    {
        float t = baseTimer*GameManager.speedManager.GetSpeed();
        while (t > 0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }

        if (gameActive)
        {
            Debug.Log("Time's up! You lose.");
            gameActive = false;
            mouse.SetActive(false);
        }
    }
}
