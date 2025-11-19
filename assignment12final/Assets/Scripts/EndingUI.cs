using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingUI : MonoBehaviour
{
    public static EndingUI Instance;

    public GameObject endingPanel;
    public TextMeshProUGUI endingTitleText;
    public TextMeshProUGUI endingBodyText;
    public Button restartButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        endingPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowEnding(string title, string body)
    {
        endingTitleText.text = title;
        endingBodyText.text = body;
        endingPanel.SetActive(true);
    }

    void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
