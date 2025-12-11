using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public string nextSceneName;

    public float typeSpeed = 0.04f;

    private string[] lines =
    {
        "Good morning.",
        "A lovely morning starts with a good pee.",
        "Come on, let¡¯s go to the bathroom.",
        "And don¡¯t let me catch you doing anything else in there, okay?"
    };

    private int index = 0;
    private bool isTyping = false;
    private string currentLine;
    private Coroutine typingCoroutine;

    void Start()
    {
        ShowLine();
    }

    void ShowLine()
    {
        if (index < 0 || index >= lines.Length)
        {
            LoadNextScene();
            return;
        }

        currentLine = lines[index];

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in currentLine)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }
    public void OnDialogueClick()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueText.text = currentLine;
            isTyping = false;
            return;
        }

        index++;

        if (index >= lines.Length)
        {
            LoadNextScene();
        }
        else
        {
            ShowLine();
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
