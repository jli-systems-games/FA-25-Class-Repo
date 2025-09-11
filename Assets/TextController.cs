using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextController : MonoBehaviour
{
    public string[] sentences;
    private int index = 0;

    public TMP_Text dialogueText;

    void Start()
    {
        if (sentences.Length > 0)
            dialogueText.text = sentences[0];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextSentence();
        }
    }

    void NextSentence()
    {
        index++;
        if (index < sentences.Length)
        {
            dialogueText.text = sentences[index];
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
    }
}
