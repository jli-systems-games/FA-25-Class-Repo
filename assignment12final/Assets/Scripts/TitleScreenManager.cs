using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Root")]
    public GameObject titleCanvasRoot;

    [Header("Instructions UI")]
    public GameObject instructionsPanel;
    public TextMeshProUGUI instructionsText;

    [Header("Main Scene")]
    [SerializeField] private string mainSceneName = "MainScene";

    private void Start()
    {
        if (titleCanvasRoot != null) titleCanvasRoot.SetActive(true);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);

        if (instructionsText != null && string.IsNullOrEmpty(instructionsText.text))
        {
            instructionsText.text =
                "CONTROLS\n\n" +
                "- Move: WASD or Arrow Keys\n" +
                "- Interact / Talk: E (when icon appears)\n" +
                "- Advance dialogue: E / Space / Enter\n" +
                "- Skip typing effect: Space / Enter while text is appearing\n" +
                "- Choose options: Click buttons or press 1 / 2\n" +
                "- Side quests: walk up to marked people/objects and press E\n" +
                "- Intro slideshow: Space / Enter or Next button to continue, Esc to skip\n";
        }
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void OnClickShowInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }

    public void OnClickCloseInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }

    public void OnClickQuit()
    {
        Application.Quit();


#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
