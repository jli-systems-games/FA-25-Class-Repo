using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingUI : MonoBehaviour
{
    public static EndingUI Instance;

    [Header("UI")]
    public GameObject endingPanel;
    public TextMeshProUGUI endingTitleText;
    public TextMeshProUGUI endingBodyText;
    public Button restartButton;

    [Header("Scene Names")]
    [SerializeField] private string titleSceneName = "TitleScene";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (endingPanel != null)
            endingPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartToTitle);
    }

    public void ShowEnding(string title, string body)
    {
        if (endingTitleText != null)
            endingTitleText.text = title;

        if (endingBodyText != null)
            endingBodyText.text = body;

        if (endingPanel != null)
            endingPanel.SetActive(true);
    }

    void RestartToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }
}
