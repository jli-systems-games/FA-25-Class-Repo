using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public float rangeX = 10f;
    public float rangeY = 7f;
    public TMP_Text movementText;
    public Button nextButton;

    public GameObject[] stories;
    public int score = 0;
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
        if (score == 3 && stories.Length > 0)
        {
            stories[0].gameObject.SetActive(true);
            patternLength++;
        }

        else if (score == 6 && stories.Length > 1)
        {
            stories[1].gameObject.SetActive(true);
            patternLength++;
        }
        else if (score == 9 && stories.Length > 2)
        {
            stories[2].gameObject.SetActive(true);
            patternLength++;
            StartCoroutine(ShowButton());
        }
    }

    private IEnumerator ShowButton()
    {
        yield return new WaitForSeconds(4f);
        nextButton.gameObject.SetActive(true);
    }
}
