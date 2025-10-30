using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameControl : MonoBehaviour
{
    public int targetCount;
    public string nextSceneName;
    public float sceneChangeDelay = 2f;

    public TMP_Text scoreText;

    [HideInInspector] public int currentCount = 0;

    private void Awake()
    {
        if (scoreText == null) scoreText = Object.FindAnyObjectByType<TMP_Text>();
    }

    private void Start() => UpdateUI();

    public void AddCount()
    {
        currentCount++;
        UpdateUI();
        if (currentCount >= targetCount && !string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(LoadSceneWithDelay());
        }
    }
    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"{currentCount}/{targetCount}";
    }

    private System.Collections.IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(sceneChangeDelay);
        SceneManager.LoadScene(nextSceneName);
    }
}