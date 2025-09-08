using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public Slider timerSlider;
    public TMP_Text timerText;
    public float gameTime = 15f;   // 총 시간(초)
    public string failScene = "Fail";

    float remaining;

    void Start()
    {
        remaining = gameTime;
        if (timerSlider != null)
        {
            timerSlider.maxValue = gameTime;
            timerSlider.value = gameTime;
        }
    }

    void Update()
    {
        remaining -= Time.deltaTime;
        if (remaining <= 0f)
        {
            remaining = 0f;
            UpdateUI();
            SceneManager.LoadScene(failScene);
            enabled = false;
            return;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);
        if (timerText != null) timerText.text = $"{minutes:0}:{seconds:00}";
        if (timerSlider != null) timerSlider.value = remaining;
    }
}