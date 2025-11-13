using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCoreMAnager : MonoBehaviour
{
    public static SCoreMAnager Instance;

    public TMP_Text scoreText;
    public GameObject finishButton;
    public int totalNotes = 8;

    int score = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        score = 0;
        UpdateUI();
        finishButton.SetActive(false);
    }

    public void AddScore()
    {
        score++;
        UpdateUI();

        if (score >= totalNotes)
            finishButton.SetActive(true);
    }

    void UpdateUI()
    {
        scoreText.text = score + " / " + totalNotes;
    }
}
