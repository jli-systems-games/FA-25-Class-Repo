using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneDialogue : MonoBehaviour
{
    public TMP_Text dialogueText;
    [TextArea]
    public string[] lines;
    public string nextSceneName = "main";

    int _currentIndex = 0;
    bool _isFinished = false;

    void Start()
    {
        if (dialogueText != null && lines != null && lines.Length > 0)
        {
            _currentIndex = 0;
            dialogueText.text = lines[_currentIndex];
        }
    }

    void Update()
    {
        if (_isFinished) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShowNextLine();
        }
    }

    void ShowNextLine()
    {
        if (lines == null || lines.Length == 0 || dialogueText == null)
            return;

        _currentIndex++;

        if (_currentIndex < lines.Length)
        {
            dialogueText.text = lines[_currentIndex];
        }
        else
        {
            _isFinished = true;
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
