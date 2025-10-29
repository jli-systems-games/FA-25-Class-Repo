using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public float rangeX = 10f;
    public float rangeY = 7f;
    public TMP_Text movementText;

    public TMP_Text[] unlockTexts;
    private int score = 0;
    private int patternLength = 4;

    private GameObject currentItem;

    private void Start()
    {
        SpawnNextItem();
    }

    public void SpawnNextItem()
    {
        if (currentItem != null) Destroy(currentItem);

        int index = Random.Range(0, itemPrefabs.Length);

        Vector2 spawnPoint = new Vector2(Random.Range(-rangeX, rangeX), Random.Range(-rangeY, rangeY));

        currentItem = Instantiate(itemPrefabs[index], spawnPoint, Quaternion.identity);

        var item = currentItem.GetComponent<ItemController>();
        item.manager = this;
        item.movementTextUI = movementText;
        item.patternLength = patternLength;
    }
    public void AddScore()
    {
        score++;
        patternLength++;
        if (score == 3 && unlockTexts.Length > 0)
            unlockTexts[0].gameObject.SetActive(true);
        else if (score == 6 && unlockTexts.Length > 1)
            unlockTexts[1].gameObject.SetActive(true);
        else if (score == 9 && unlockTexts.Length > 2)
            unlockTexts[2].gameObject.SetActive(true);
    }
}
