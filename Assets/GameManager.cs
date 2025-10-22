using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Stats")]
    public int desire = 5;
    public int pain = 3;
    public int ecstacy = 0;
    public int token = 0;

    [Header("UI")]
    public Slider desireSlider;
    public Slider painSlider;
    public Slider ecstacySlider;
    public TMP_Text tokenText;

    private int maxDesire = 10;
    private int maxPain = 10;
    private int maxEcstacy = 3;

    void Start()
    {
        desireSlider.maxValue = maxDesire;
        painSlider.maxValue = maxPain;
        ecstacySlider.maxValue = maxEcstacy;

        UpdateUI();
    }

    public void AddPoints()
    {
        token ++;
        UpdateUI();
    }

    public void Play()
    {
        if (token <= 0) return;
        desire = Mathf.Min(desire + 2, maxDesire);
        pain = Mathf.Min(pain + 1, maxPain);
        token--;
        CheckState();
    }

    public void Comfort()
    {
        if (token <= 0) return;
        desire = Mathf.Max(desire - 1, 0);
        pain = Mathf.Max(pain - 2, 0);
        token--;
        CheckState();
    }

    public void Kiss()
    {
        if (token <= 0) return;

        if (desire >= 7 && pain >= 4 && pain <= 8)
        {
            Debug.Log("can kiss now");
            ecstacy = Mathf.Min(ecstacy + 1, maxEcstacy);
            desire = Mathf.Max(desire - 3, 0);
            pain = Mathf.Max(pain +3, 0);
            token--;
        }
        CheckState();
    }

    void CheckState()
    {
        if (desire <= 0 || pain >= maxPain)
        {
            SceneManager.LoadScene("LoseScene");
        }
        if (ecstacy >= maxEcstacy)
        {
            SceneManager.LoadScene("WinScene");
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        desireSlider.value = desire;
        painSlider.value = pain;
        ecstacySlider.value = ecstacy;

        if (desire >= 7 && pain >= 4 && pain <= 8)
        {
            Debug.Log("can kiss now");

        }
            if (tokenText != null)
            tokenText.text = token.ToString();
    }
}
