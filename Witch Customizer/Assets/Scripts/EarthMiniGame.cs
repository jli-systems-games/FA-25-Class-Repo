using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EarthMiniGame : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text resultText;

    [Header("Settings")]
    public float timeLimit = 20f;

    private float timer;
    private int grownCount = 0;
    private bool finished = false;

    void Start()
    {
        timer = timeLimit;
        resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Stop counting down once finished
        if (finished) return;

        timer -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.Ceil(timer);

        if (timer <= 0 && !finished && grownCount < 3)
            Lose();
    }

    public void PlantGrown()
    {
        if (finished) return; // Prevent further calls after game ends
        // Count each finished plant
        grownCount++;
        Debug.Log("PlantGrown called. Total = " + grownCount);

        // Trigger win only once
        if (grownCount >= 3)
            Win();
    }

    void Win()
    {
        finished = true; // Prevents Update() from running again
        resultText.gameObject.SetActive(true);
        resultText.text = $"You mastered Earth!";
        Debug.Log("Win triggered!");
        Invoke(nameof(ReturnToTitle), 3f);
    }

    void Lose()
    {
        finished = true;
        resultText.gameObject.SetActive(true);
        resultText.text = "The grove withers...";
        Debug.Log("Lose triggered!");
        Invoke(nameof(ReturnToTitle), 3f);
    }

    void ReturnToTitle()
    {
        Data.Instance.ResetData();
        SceneManager.LoadScene("Menu");
    }
}