using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SeedDropper : MonoBehaviour
{
    public GameObject seedPrefab;
    public TMP_Text resultText;

    private int planted = 0;
    private int seedsLeft = 5;
    private bool finished = false;

    void Start()
    {
        resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (finished) return;

        if (Input.GetKeyDown(KeyCode.Space) && seedsLeft > 0)
        {
            DropSeed();
        }

        if (planted >= 3)
            Win();
        else if (seedsLeft <= 0 && planted < 3)
            Lose();
    }

    void DropSeed()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-3f, 3f), 5f, 0);
        Instantiate(seedPrefab, spawnPos, Quaternion.identity);
        seedsLeft--;
    }

    public void PlantSeed()
    {
        planted++;
    }

    void Win()
    {
        finished = true;
        resultText.gameObject.SetActive(true);
        resultText.text = $"{Data.Instance.data_playerName}, you mastered Earth!";
        Invoke(nameof(ReturnToTitle), 3f);
    }

    void Lose()
    {
        finished = true;
        resultText.gameObject.SetActive(true);
        resultText.text = "Your seeds failed to take root...";
        Invoke(nameof(ReturnToTitle), 3f);
    }

    void ReturnToTitle()
    {
        Data.Instance.ResetData();
        SceneManager.LoadScene("TitleScene");
    }
}
