using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaterMiniGame : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text resultText;
    public Slider fillBar;

    [Header("Game Settings")]
    public GameObject dropPrefab;
    public float gameTime = 15f;
    public int goalScore = 10;
    public float spawnRate = 0.8f;

    private float timer;
    private int score = 0;
    private bool finished = false;

    void Start()
    {
        timer = gameTime;
        resultText.gameObject.SetActive(false);
        InvokeRepeating(nameof(SpawnDrop), 1f, spawnRate);
    }

    void Update()
    {
        if (finished) return;

        timer -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.Ceil(timer);
        scoreText.text = "Drops: " + score + "/" + goalScore;

        if (fillBar)
            fillBar.value = (float)score / goalScore;

        if (score >= goalScore)
            Win();
        else if (timer <= 0)
            Lose();
    }

    void SpawnDrop()
    {
        if (finished) return;

        float x = Random.Range(-300f, 300f); // adjust for Canvas size
        float y = Random.Range(100f, 200f);
        GameObject newDrop = Instantiate(dropPrefab, transform);
        newDrop.transform.localPosition = new Vector3(x, y, 0);
    }

    public void CollectDrop(GameObject drop)
    {
        if (finished) return;

        score++;
        Destroy(drop);
    }

    void Win()
    {
        finished = true;
        resultText.gameObject.SetActive(true);
        resultText.text = $"You mastered Water!";
        Invoke(nameof(ReturnToTitle), 3f);
    }

    void Lose()
    {
        finished = true;
        resultText.gameObject.SetActive(true);
        resultText.text = "The tide slips away...";
        Invoke(nameof(ReturnToTitle), 3f);
    }

    void ReturnToTitle()
    {
        Data.Instance.ResetData();
        SceneManager.LoadScene("Menu");
    }
}
